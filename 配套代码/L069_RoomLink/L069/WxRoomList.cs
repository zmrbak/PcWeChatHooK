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
    public class WxRoomList
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

            int linkPointer = GetWxMemoryInt(wxProcess.Handle, weChatBaseAdress + roomLinkOffset) + 0x678 + 0x64;
            Console.WriteLine("链表指针：\t0x" + linkPointer.ToString("X8"));
            Console.WriteLine("#######################################################");

            GetLinkData(linkPointer);
        }

        private void GetLinkData(int LinkHeader)
        {
            //群链表地址
            int headerAddress = GetWxMemoryInt(wxProcess.Handle, LinkHeader);
            Console.WriteLine("头地址：\t0x" + headerAddress.ToString("X8"));
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

            //$ ==> 0C5F5360
            int header1 = GetWxMemoryInt(wxProcess.Handle, roomAddress);
            //$+4 > 10D3C4A8
            int header2 = GetWxMemoryInt(wxProcess.Handle, roomAddress + 4);
            //$+8 > 0C5F5360
            int header3 = GetWxMemoryInt(wxProcess.Handle, roomAddress + 8);
            //$+C > 00000001
            //$+10 > 10D5E720  UNICODE "10048068352@chatroom"
            //$+14 > 00000014
            //$+18 > 00000020
            String roomWxId = GetWxMemoryUnicodeString(
                wxProcess.Handle,
                GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x10),
                GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x14)
                );
            Console.WriteLine("群wxid：\t" + roomWxId);
            //$+1C > 00000000
            //$+20 > 00000000
            //$+24 > 0000400C
            //$+28 > 10D5E7C0 UNICODE "10048068352@chatroom"
            //$+2C > 00000014
            //$+30 > 00000020
            //$+34 > 00000000
            //$+38 > 00000000
            //$+3C > 10DBEA70 UNICODE "^Gzhaoqingming1929^Gwxid_juxacm8a5ysr22^Gwxid_2aii"
            //$+40 > 00000D54
            //$+44 > 00001000
            //$+48 > 00000000
            //$+4C > 00000000
            //$+50 > 10D5E680  UNICODE "^G^G陈家英13071209807耶格^G陈^G"
            //$+54 > 00000019
            //$+58 > 00000020
            //$+5C > 00000000
            //$+60 > 00000000
            //$+64 > 00000009
            //$+68 > 10D57D78  UNICODE "zhaoqingming1929"
            //$+6C > 00000010
            //$+70 > 00000010
            String roomMaster = GetWxMemoryUnicodeString(
              wxProcess.Handle,
              GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x68),
              GetWxMemoryInt(wxProcess.Handle, roomAddress + 0x6c)
              );
            Console.WriteLine("群主wxid：\t" + roomMaster);

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

                Process.Start(wxPath);
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
