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

namespace L022CSMultiWeChat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int i = 0;
        private void Button1_Click(object sender, EventArgs e)
        {
            int a = WeChatMultiOpen();
            if (a == 0)
            {
                i++;
                this.textBox1.AppendText("成功打开了"+i+"个微信！" + Environment.NewLine);
            }
        }

        [DllImport("L022CMultiWeChat.dll")]
        public static extern int WeChatMultiOpen();

        private void Button2_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/zmrbak/PcWeChatHooK");
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Process.Start("http://t.cn/EXUbebQ");
        }
    }
}
