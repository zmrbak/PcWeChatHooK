using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace L000WeChatDllInjector
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// 刷新信息
        /// </summary>
        public void Refresh()
        {
            int WxId = 0;
            Process[] processes = Process.GetProcessesByName("WeChat");

            StringBuilder wxInfo = new StringBuilder();
            wxInfo.Append("刷新时间：\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
            wxInfo.Append("DLL位置：\t" + Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Environment.NewLine);

            foreach (Process process in processes)
            {
                WxId = process.Id;
                wxInfo.Append("进程PID：\t" + process.Id + Environment.NewLine);
                wxInfo.Append("窗口标题：\t" + process.MainWindowTitle + Environment.NewLine);
                wxInfo.Append("启动时间：\t" + process.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);

                //确定微信版本
                foreach (ProcessModule item in process.Modules)
                {
                    if (item.ModuleName.ToLower() != "WeChatWin.dll".ToLower()) continue;

                    wxInfo.Append("微信目录：\t" + System.IO.Path.GetDirectoryName(process.MainModule.FileName) + Environment.NewLine);
                    wxInfo.Append("微信版本：\t" + item.FileVersionInfo.FileVersion + Environment.NewLine);
                    wxInfo.Append("微信基址：\t" + "0x" + item.BaseAddress.ToString("X8") + Environment.NewLine);

                    break;
                }
                break;
            }
            tb_WxInfo.Text = wxInfo.ToString();

            //遍历当前文件目录下的DLL
            List<String> fileList = new List<string>();
            this.cb_dllLists.ItemsSource = null;
            foreach (String item in Directory.GetFiles(".", "*.dll"))
            {
                fileList.Add(System.IO.Path.GetFileName(item));
            }
            this.cb_dllLists.ItemsSource = fileList;
            this.cb_dllLists.SelectedIndex = 0;

            if (WxId == 0)
            {
                tb_WxInfo.Text = "错误信息：注入前请先启动微信！";
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_Inject_Click(object sender, RoutedEventArgs e)
        {
            //1) 遍历系统中的进程，找到微信进程（CreateToolhelp32Snapshot、Process32Next）
            Process[] processes = Process.GetProcesses();
            Process WxProcess = null;
            foreach (Process process in processes)
            {
                if (process.ProcessName.ToLower() == "WeChat".ToLower())
                {
                    WxProcess = process;
                    foreach (ProcessModule processModule in WxProcess.Modules)
                    {
                        if (processModule.ModuleName == cb_dllLists.Text)
                        {
                            MessageBox.Show("DLL文件“" + cb_dllLists.Text + "”之前已注入!\n\n若要重新注入，请先重启微信!", "警告", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                    break;
                }
            }

            if (WxProcess == null)
            {
                MessageBox.Show("注入前请先启动微信！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //2) 打开微信进程，获得HANDLE（OpenProcess）。

            //3) 在微信进程中为DLL文件路径字符串申请内存空间（VirtualAllocEx）。
            if (this.cb_dllLists.Items.Count == 0)
            {
                MessageBox.Show("没找到被注入的DLL文件！\n请把被注入的DLL文件放在本程序所在目录下。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //默认选择第一项
            if (this.cb_dllLists.SelectedIndex == -1)
            {
                this.cb_dllLists.SelectedIndex = 0;
            }

            if (this.cb_dllLists.Text == null || this.cb_dllLists.Text == "")
            {
                MessageBox.Show("没找到被注入的DLL文件！\n请把被注入的DLL文件放在本程序所在目录下。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            String DLlPath = System.IO.Path.GetFullPath(this.cb_dllLists.Text); //\0
            if (File.Exists(DLlPath) == false)
            {
                MessageBox.Show("被注入的DLL文件(" + DLlPath + ")不存在！\n请把被注入的DLL文件放在本程序所在目录下。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            int DllPathSize = DLlPath.Length * 2 + 1;
            int MEM_COMMIT = 0x00001000;
            int PAGE_READWRITE = 0x04;
            int DllAddress = VirtualAllocEx((int)WxProcess.Handle, 0, DllPathSize, MEM_COMMIT, PAGE_READWRITE);
            if (DllAddress == 0)
            {
                MessageBox.Show("内存分配失败！");
                return;
            }
            tb_WxInfo.AppendText("内存地址:\t" + "0x" + DllAddress.ToString("X8") + Environment.NewLine);

            //4) 把DLL文件路径字符串写入到申请的内存中（WriteProcessMemory）
            if (WriteProcessMemory((int)WxProcess.Handle, DllAddress, DLlPath, DllPathSize, 0) == false)
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
            if (CreateRemoteThread((int)WxProcess.Handle, 0, 0, LoadLibraryAddress, DllAddress, 0, 0) == 0)
            {
                MessageBox.Show("执行远程线程失败！");
                return;
            }

            tb_WxInfo.AppendText("成功注入:\t" + cb_dllLists.Text + Environment.NewLine);
        }

        #region  WinApi
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


        [DllImport("Kernel32.dll")]
        //BOOL VirtualFreeEx(
        //  HANDLE hProcess,
        //  LPVOID lpAddress,
        //  SIZE_T dwSize,
        //  DWORD dwFreeType
        //);
        public static extern Boolean VirtualFreeEx(int hProcess, int lpAddress, int dwSize, int dwFreeType);
        #endregion

        /// <summary>
        /// 打开视频帮助
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_Help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://t.cn/EXUbebQ");
        }

        /// <summary>
        /// 重启微信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_WxRestart_Click(object sender, RoutedEventArgs e)
        {
            //如果当前系统中，微信在运行，则重启微信
            String WxPath = "";
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName.ToLower() == "WeChat".ToLower())
                {
                    if (WxPath == "")
                    {
                        WxPath = process.MainModule.FileName;
                    }
                    process.Kill();
                }
            }

            //启动微信
            if (WxPath == "")
            {
                //在注册表中查找微信
                //计算机\HKEY_CURRENT_USER\Software\Tencent\WeChat
                //InstallPath
                try
                {
                    RegistryKey registryKey = Registry.CurrentUser;
                    RegistryKey software = registryKey.OpenSubKey("Software\\Tencent\\WeChat");
                    object InstallPath = software.GetValue("InstallPath");
                    WxPath = InstallPath.ToString() + "\\WeChat.exe";
                    registryKey.Close();
                }
                catch
                {
                    WxPath = "";
                }
            }

            if (WxPath != "")
            {
                Process.Start(WxPath);
                Thread.Sleep(500);
                Refresh();
            }
            else
            {
                MessageBox.Show("在系统中未找到微信，请手动启动微信", "错误", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        /// <summary>
        /// 打开github代码仓库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_GitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/zmrbak/PcWeChatHooK");
        }
    }
}
