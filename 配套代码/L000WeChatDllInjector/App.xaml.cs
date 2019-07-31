using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace L000WeChatDllInjector
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //只运行一个实例
            //把已经启动的实例，全部结束
            Process thisProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(thisProcess.ProcessName);

            foreach (Process item in processes)
            {
                if (item.Id != thisProcess.Id)
                {
                    item.Kill();
                }
            }

            base.OnStartup(e);
        }
    }
}
