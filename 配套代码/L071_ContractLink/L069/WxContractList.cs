using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L069
{
    public class WxContractList
    {
        Process wxProcess = null;
        int weChatBaseAdress = 0;
        int roomLinkOffset = 0x126E0A0;
        List<int> nodeAddressList = new List<int>();
        int index = 0;
        public void GetData()
        {
            WeChatStart();
            WeChatCheck();

            int linkPointer = GetWxMemoryInt(wxProcess.Handle, weChatBaseAdress + roomLinkOffset) + 0x24 + 0x68;
            Console.WriteLine("链表指针：\t0x" + linkPointer.ToString("X8"));
            Console.WriteLine("#######################################################");

            GetLinkData(linkPointer);
        }

        private void GetLinkData(int LinkHeader)
        {
            //联系人链表地址
            int headerAddress = GetWxMemoryInt(wxProcess.Handle, LinkHeader);
            Console.WriteLine("头地址：\t0x" + headerAddress.ToString("X8"));
            if (headerAddress == 0)
            {
                Console.WriteLine("请先登录微信");
                return;
            }

            //群数量
            int contractCount = GetWxMemoryInt(wxProcess.Handle, LinkHeader + 4);
            Console.WriteLine("节点数量：\t0x" + contractCount.ToString("X8"));
            nodeAddressList.Add(headerAddress);

            Console.WriteLine("#######################################################");
            int header1 = GetWxMemoryInt(wxProcess.Handle, headerAddress);
            int header2 = GetWxMemoryInt(wxProcess.Handle, headerAddress + 4);
            int header3 = GetWxMemoryInt(wxProcess.Handle, headerAddress + 8);

            Console.WriteLine("分支1：\t\t0x" + header1.ToString("X8"));
            Console.WriteLine("分支2：\t\t0x" + header2.ToString("X8"));
            Console.WriteLine("分支3：\t\t0x" + header3.ToString("X8"));
            Console.WriteLine("#######################################################");

            GetRoomInfo(header1, "0x" + headerAddress.ToString("X8"));
            GetRoomInfo(header2, "0x" + headerAddress.ToString("X8"));
            GetRoomInfo(header3, "0x" + headerAddress.ToString("X8"));
        }

        private void GetRoomInfo(int roomAddress, string log)
        {
            foreach (var item in nodeAddressList)
            {
                if (item == roomAddress) return;
            }
            nodeAddressList.Add(roomAddress);
            
            index++;
            Console.WriteLine("---------------" + index + "------------------");
            log = log + "->" + "0x" + roomAddress.ToString("X8");
            Console.WriteLine(log);

//$ ==>    > 07E0C958
//$+4 > 07D18478
//$+8 > 03F6C278
            int header1 = GetWxMemoryInt(wxProcess.Handle, roomAddress);
            int header2 = GetWxMemoryInt(wxProcess.Handle, roomAddress + 4);
            int header3 = GetWxMemoryInt(wxProcess.Handle, roomAddress + 8);

//$+10 > 07D50890  UNICODE "wxid_e3hyk98lir0t21"
//$+14 > 00000013
//$+18 > 00000020

            String contractWxId = GetWxMemoryUnicodeString(
                wxProcess.Handle,
                GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x10),
                GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x14)
                );
            Console.WriteLine("联系人wxid：\t" + contractWxId);

            //$+44 > 07AFAFE0 UNICODE "qq694950743"
            //$+48 > 0000000B
            //$+4C > 00000010
            String contractWxName = GetWxMemoryUnicodeString(
                           wxProcess.Handle,
                           GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x44),
                           GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x48)
                           );
            Console.WriteLine("联系人账号：\t" + contractWxName);

            //$+70 > 00000003
            int sex = GetWxMemoryInt(wxProcess.Handle,
                GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x70)
                );
            Console.WriteLine("联系人性别：\t" + sex);

            //$+8C > 0408D1C8 UNICODE "叶静"
            //$+90 > 00000002
            //$+94 > 00000002
            String contractNickName = GetWxMemoryUnicodeString(
                                      wxProcess.Handle,
                                      GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x8c),
                                      GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x90)
                                      );
            Console.WriteLine("联系人昵称：\t" + contractNickName);

            GetRoomInfo(header1, log);
            GetRoomInfo(header2, log);
            GetRoomInfo(header3, log);
        }

        private void WeChatCheck()
        {
            if (wxProcess == null) return;
            Console.WriteLine("进程PID：\t" + wxProcess.Id);
            Console.WriteLine("窗口标题：\t" + wxProcess.MainWindowTitle);
            Console.WriteLine("启动时间：\t" + wxProcess.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("微信目录：\t" + System.IO.Path.GetDirectoryName(wxProcess.MainModule.FileName));

            var modules = (from ProcessModule module in wxProcess.Modules
                           where module.ModuleName.ToLower() == "WeChatWin.dll".ToLower()
                           select module).ToList();
            if (modules == null || modules.Count() == 0)
            {
                return;
            }
            weChatBaseAdress = (int)modules[0].BaseAddress;
            Console.WriteLine("微信基址：\t" + "0x" + weChatBaseAdress.ToString("X8"));

            String wxVersion = modules[0].FileVersionInfo.FileVersion;
            Console.WriteLine("微信版本：\t" + wxVersion);

            if (wxVersion != "2.6.8.65")
            {
                Console.WriteLine("当前微信版本不支持！");
                Console.WriteLine("请使用版本为2.6.8.65的微信！");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private void WeChatStart()
        {
            var processes = Process.GetProcessesByName("WeChat");
            //微信未启动
            if (processes == null || processes.Length == 0)
            {
                //启动微信
                RegistryKey registryKey = Registry.CurrentUser;
                //算机\HKEY_CURRENT_USER\Software\Tencent\WeChat
                RegistryKey software = registryKey.OpenSubKey("Software\\Tencent\\WeChat");
                object InstallPath = software.GetValue("InstallPath");
                String wxPath = InstallPath.ToString() + "\\WeChat.exe";
                registryKey.Close();

                wxProcess=Process.Start(wxPath);
                Thread.Sleep(500);
            }
            else
            {
                wxProcess = processes[0];
            }
        }

        public static String GetWxMemoryUnicodeString(IntPtr hProcess, int lpBaseAddress, int nSize = 100)
        {
            byte[] data = new byte[nSize * 2];
            if (ReadProcessMemory(hProcess, (IntPtr)lpBaseAddress, data, nSize * 2, 0) == 0)
            {
                return "";
            }
            return Encoding.Unicode.GetString(data);
        }


        public static int GetWxMemoryInt(IntPtr hProcess, int lpBaseAddress)
        {
            byte[] data = new byte[4];
            if (ReadProcessMemory(hProcess, (IntPtr)lpBaseAddress, data, 4, 0) == 0)
            {
                return 0;
            }
            return BitConverter.ToInt32(data, 0);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int ReadProcessMemory(
          IntPtr hProcess,
          IntPtr lpBaseAddress,
          [Out] byte[] lpBuffer,
          int dwSize,
          int lpNumberOfBytesRead);
    }
}
