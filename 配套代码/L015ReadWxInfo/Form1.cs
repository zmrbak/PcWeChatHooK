using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L015ReadWxInfo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            this.textBox1.Clear();

            //微信进程
            Process WxProcess = null;
            //WeChatWin.dll基址
            IntPtr WeChatWinBaseAddress = IntPtr.Zero;
            //微信版本
            String WeChatVersion = "";
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName == "WeChat")
                {
                    WxProcess = process;
                    this.textBox1.AppendText("微信已找到！" + Environment.NewLine);
                    this.textBox1.AppendText("微信句柄:\t" + "0x" + ((int)(process.Handle)).ToString("X8") + Environment.NewLine);
                    foreach (ProcessModule processModule in process.Modules)
                    {
                        if (processModule.ModuleName == "WeChatWin.dll")
                        {
                            WeChatWinBaseAddress = processModule.BaseAddress;
                            this.textBox1.AppendText("微信基址:\t" + "0x" + ((int)(processModule.BaseAddress)).ToString("X8") + Environment.NewLine);

                            WeChatVersion = processModule.FileVersionInfo.FileVersion;
                            this.textBox1.AppendText("微信版本:\t" + processModule.FileVersionInfo.FileVersion + Environment.NewLine);

                            break;
                        }
                    }
                    break;
                }
            }

            if (WxProcess == null)
            {
                this.textBox1.AppendText("微信没有找到！");
                return;
            }

            //版本号是否匹配
            if (WeChatVersion != "2.6.6.28")
            {
                MessageBox.Show("当前微信版本：" + WeChatVersion + "\n版本不匹配，请使用2.6.6.28版本，其它版本无效！");
                return;
            }

            //微信号
            int WxNameAddress = (int)WeChatWinBaseAddress + 0x1131B90;
            this.textBox1.AppendText("微信号地址:\t" + "0x" + ((int)(WxNameAddress)).ToString("X8") + Environment.NewLine);
            this.textBox1.AppendText("微信号:\t" + GetString(WxProcess.Handle, (IntPtr)WxNameAddress) + Environment.NewLine);

            //微信昵称
            int WxNickNameAddress = (int)WeChatWinBaseAddress + 0x1131C64;
            this.textBox1.AppendText("微信昵称地址:\t" + "0x" + ((int)(WxNickNameAddress)).ToString("X8") + Environment.NewLine);
            this.textBox1.AppendText("微信昵称:\t" + GetString(WxProcess.Handle, (IntPtr)WxNickNameAddress) + Environment.NewLine);

            //微信ID wxid
            int WxIdAddress = (int)WeChatWinBaseAddress + 0x1131B78;
            this.textBox1.AppendText("微信ID地址:\t" + "0x" + ((int)(WxIdAddress)).ToString("X8") + Environment.NewLine);
            this.textBox1.AppendText("微信ID:\t" + GetString(WxProcess.Handle, (IntPtr)(GetAddress(WxProcess.Handle, (IntPtr)WxIdAddress))) + Environment.NewLine);


        }

        String GetString(IntPtr hProcess, IntPtr lpBaseAddress, int nSize = 100)
        {
            byte[] data = new byte[nSize];
            if (ReadProcessMemory(hProcess, lpBaseAddress, data, nSize, 0) == 0)
            {
                //读取内存失败！
                return "";
            }
            String result = "";
            String TempString = Encoding.ASCII.GetString(data);
            // \0
            foreach (char item in TempString)
            {
                if (item == '\0')
                {
                    break;
                }
                result += item.ToString();
            }
            return result;
        }

        int GetAddress(IntPtr hProcess, IntPtr lpBaseAddress)
        {
            byte[] data = new byte[4];
            if (ReadProcessMemory(hProcess, lpBaseAddress, data, 4, 0) == 0)
            {
                //读取内存失败！
                return 0;
            }

            String Hex = data[3].ToString("x2") +
                data[2].ToString("x2") +
                data[1].ToString("x2") +
                data[0].ToString("x2");
            return int.Parse(Hex, System.Globalization.NumberStyles.HexNumber);
        }


        [DllImport("Kernel32.dll")]
        //BOOL ReadProcessMemory(
        //  HANDLE hProcess,
        //  LPCVOID lpBaseAddress,
        //  LPVOID lpBuffer,
        //  SIZE_T nSize,
        //  SIZE_T* lpNumberOfBytesRead
        //);
        public static extern int ReadProcessMemory(
              IntPtr hProcess,  //正在读取内存的进程句柄。句柄必须具有PROCESS_VM_READ访问权限。
              IntPtr lpBaseAddress,    //指向要从中读取的指定进程中的基址的指针。在发生任何数据传输之前，系统会验证基本地址和指定大小的内存中的所有数据是否都可以进行读访问，如果无法访问，则该函数将失败。
              byte[] lpBuffer,  //指向缓冲区的指针，该缓冲区从指定进程的地址空间接收内容。
              int nSize,    //要从指定进程读取的字节数。
              int lpNumberOfBytesRead //指向变量的指针，该变量接收传输到指定缓冲区的字节数。如果lpNumberOfBytesRead为NULL，则忽略该参数。
            );
    }
}
