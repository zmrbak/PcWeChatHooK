using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L069
{
    class Program
    {
        static void Main(string[] args)
        {
            WxSessionList wxSessionList = new WxSessionList();
            wxSessionList.GetData();

            Console.ReadLine();
            //Process.Start("https://github.com/zmrbak/PcWeChatHooK");
        }
    }
}
