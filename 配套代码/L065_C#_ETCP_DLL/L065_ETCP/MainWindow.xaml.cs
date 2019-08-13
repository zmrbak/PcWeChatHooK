using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace L065_ETCP
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

        public delegate void tcp_fun(IntPtr Server, IntPtr so, int type, StringBuilder buf, int len, int count);
        public delegate void tcp_fun_client(IntPtr Client, IntPtr so, int type, StringBuilder buf, int len);

        tcp_fun 服务端回调引用 = 服务端;
        tcp_fun_client 客户端回调引用 = 客户端;

        public static void 服务端(IntPtr Server, IntPtr so, int type, StringBuilder buf, int len, int count)
        {
            switch(type)
            {
                case 1:
                    break;
                case 2:
                    String data = buf.ToString();
                    //MyMonitor.InFunction(data);

                    StringBuilder stringBuilder = new StringBuilder(data);
                    stringBuilder.Append("A");

                    etcp_tcp_send(Server, so, stringBuilder, stringBuilder.Length);

                    break;
                case 3:
                    break;
                default:
                    break;

            }
        }

        public static void 客户端(IntPtr Client, IntPtr so, int type, StringBuilder buf, int len)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyMonitor.PartEvent += MyMonitor_PartEvent;
            ETCP初始化(服务端回调引用, 客户端回调引用, 8192);
            服务端创建("0.0.0.0", 8421, 1);
        }

        private void MyMonitor_PartEvent(object sender, MessageArgs e)
        {
            this.Dispatcher.Invoke(new Action(()=> {
                textBox.Text = e.TextMessage;
            }));            
        }

        int ETCP初始化(tcp_fun 服务端回调引用, tcp_fun_client 客户端回调引用, int 内置缓冲)
        {
            return etcp_vip_number(服务端回调引用, 客户端回调引用, 内置缓冲);
        }

        int 服务端创建(String  绑定地址, int 绑定端口, int 配套模式)
        {
            return etcp_tcp_server(new StringBuilder( 绑定地址), 绑定端口, 配套模式);
        }

        [DllImport("etcp60.dll",CallingConvention = CallingConvention.StdCall,EntryPoint ="#14")]
        public static extern int etcp_vip_number(tcp_fun nFun, tcp_fun_client cFun, int buflen);

        [DllImport("etcp60.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int etcp_tcp_server(StringBuilder host, int port, int nIs);

        [DllImport("etcp60.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int etcp_tcp_send(IntPtr sso, IntPtr so, StringBuilder buf, int len);

    }
}
