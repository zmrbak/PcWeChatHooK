using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        private void Button1_Click(object sender, EventArgs e)
        {
            int a = WeChatMultiOpen();
            this.textBox1.AppendText(a + Environment.NewLine);
        }

        [DllImport("L022CMultiWeChat.dll")]
        public static extern int WeChatMultiOpen();
    }
}
