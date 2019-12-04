using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace L000WeChatDllInjector
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            //只运行一个实例
            mutex = new Mutex(true, "WeChatDllInjector", out bool isNewInstance);
            if (isNewInstance != true)
            {
                IntPtr intPtr = FindWindowW(null, "微信DLL注入器");
                if (intPtr != IntPtr.Zero)
                {
                    ShowWindowAsync(intPtr, 9);
                    SetForegroundWindow(intPtr);
                }

                Shutdown();
            }
        }

        [DllImport("User32", CharSet = CharSet.Unicode)]
        static extern IntPtr ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("User32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowW(String lpClassName, String lpWindowName);

        [DllImport("User32", CharSet = CharSet.Unicode)]
        static extern Boolean SetForegroundWindow(IntPtr hWnd);
    }
}
