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

namespace L009CsInjector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int WxId = 0;
            int WxHandle = 0;

            //1) 遍历系统中的进程，找到微信进程（CreateToolhelp32Snapshot、Process32Next）
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == "WeChat")
                {
                    WxId = process.Id;
                    WxHandle = (int)(process.Handle);
                    break;
                }
            }
            if (WxId == 0)
            {
                MessageBox.Show("未找到微信！");
                return;
            }

            //2) 打开微信进程，获得HANDLE（OpenProcess）。

            //3) 在微信进程中为DLL文件路径字符串申请内存空间（VirtualAllocEx）。
            String DLlPath = @"C:\Users\Visual Studio 2019\Desktop\L004CHookDll.dll"; //\0
            int DllPathSize = DLlPath.Length + 1;
            int MEM_COMMIT = 0x00001000;
            int PAGE_READWRITE = 0x04;
            int DllAddress = VirtualAllocEx(WxHandle, 0, DllPathSize, MEM_COMMIT, PAGE_READWRITE);
            if (DllAddress == 0)
            {
                MessageBox.Show("内存分配失败！");
                return;
            }

            //4) 把DLL文件路径字符串写入到申请的内存中（WriteProcessMemory）
            if (WriteProcessMemory(WxHandle, DllAddress, DLlPath, DllPathSize, 0) == false)
            {
                MessageBox.Show("内存写入失败！");
                return;
            };


            //5) 从Kernel32.dll中获取LoadLibraryA的函数地址（GetModuleHandle、GetProcAddress）
            int module = GetModuleHandleA("Kernel32.dll");
            int LoadLibraryAddress = GetProcAddress(module, "LoadLibraryA");
            if (LoadLibraryAddress == 0)
            {
                MessageBox.Show("查找LoadLibraryA地址失败！");
                return;
            }

            //6) 在微信中启动内存中指定了文件名路径的DLL（CreateRemoteThread）。
            if (CreateRemoteThread(WxHandle, 0, 0, LoadLibraryAddress, DllAddress, 0, 0) == 0)
            {
                MessageBox.Show("执行远程线程失败！");
                return;
            }

            MessageBox.Show("Okay OK！");
        }
        [DllImport("Kernel32.dll")]
        //LPVOID VirtualAllocEx(
        //  HANDLE hProcess,
        //  LPVOID lpAddress,
        //  SIZE_T dwSize,
        //  DWORD flAllocationType,
        //  DWORD flProtect
        //);
        public static extern int VirtualAllocEx(int hProcess, int lpAddress, int dwSize, int flAllocationType, int flProtect);

        [DllImport("Kernel32.dll")]
        //BOOL WriteProcessMemory(
        //  HANDLE hProcess,
        //  LPVOID lpBaseAddress,
        //  LPCVOID lpBuffer,
        //  SIZE_T nSize,
        //  SIZE_T* lpNumberOfBytesWritten
        //);
        public static extern Boolean WriteProcessMemory(int hProcess, int lpBaseAddress, String lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("Kernel32.dll")]
        //HMODULE GetModuleHandleA(
        //  LPCSTR lpModuleName
        //);
        public static extern int GetModuleHandleA(String lpModuleName);

        [DllImport("Kernel32.dll")]
        //FARPROC GetProcAddress(
        //  HMODULE hModule,
        //  LPCSTR lpProcName
        //);
        public static extern int GetProcAddress(int hModule, String lpProcName);

        [DllImport("Kernel32.dll")]
        //HANDLE CreateRemoteThread(
        //  HANDLE hProcess,
        //  LPSECURITY_ATTRIBUTES lpThreadAttributes,
        //  SIZE_T dwStackSize,
        //  LPTHREAD_START_ROUTINE lpStartAddress,
        //  LPVOID lpParameter,
        //  DWORD dwCreationFlags,
        //  LPDWORD lpThreadId
        //);
        public static extern int CreateRemoteThread(int hProcess, int lpThreadAttributes, int dwStackSize, int lpStartAddress, int lpParameter, int dwCreationFlags, int lpThreadId);
    }
}
