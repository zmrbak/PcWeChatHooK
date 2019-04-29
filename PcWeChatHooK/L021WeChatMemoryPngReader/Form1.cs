using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L021WeChatMemoryPngReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //当前微信进程
        Process WxProcess = null;
        //微信WeChatWin基址
        IntPtr WeChatWinBaseAddress = IntPtr.Zero;
        //Image Byte
        byte[] ImageByte = null;

        /// <summary>
        /// 在操作微信之前，确保微信已经在系统中运行
        /// </summary>
        /// <returns></returns>
        bool IsWxRunning()
        {
            WxProcess = null;
            //遍历所有进程
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                //如果进程名为“WeChat”，系统中有微信正在运行
                if (process.ProcessName == "WeChat")
                {
                    //微信进程
                    WxProcess = process;
                    //在微信进程加载的模块中寻找“WeChatWin.dll”
                    foreach (ProcessModule processModule in process.Modules)
                    {
                        if (processModule.ModuleName == "WeChatWin.dll")
                        {
                            //找到WeChatWin的基址
                            WeChatWinBaseAddress = processModule.BaseAddress;
                            break;
                        }
                    }
                    break;
                }
            }

            //如果系统中未找到运行的微信，则返回false
            if (WxProcess == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 向文本框填入二进制文本数据
        /// </summary>
        /// <param name="pngBytes"></param>
        private void SetTextBoxByBytes(byte[] pngBytes)
        {
            ImageByte = pngBytes;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte item in pngBytes)
            {
                stringBuilder.Append(item.ToString("X2") + " ");
            }
            this.textBox1.Text = stringBuilder.ToString().TrimEnd();
            this.textBox4.AppendText("数据长度：" + pngBytes.Length + Environment.NewLine);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
            this.textBox4.Clear();
            this.pictureBox1.Image = null;

            String pngPath = "PngTest.png";
            this.pictureBox1.ImageLocation = pngPath;

            byte[] pngBytes = File.ReadAllBytes(pngPath);
            SetTextBoxByBytes(pngBytes);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
            this.textBox4.Clear();
            this.pictureBox1.Image = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.png|*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                String pngPath = openFileDialog.FileName;
                this.pictureBox1.ImageLocation = pngPath;

                byte[] pngBytes = File.ReadAllBytes(pngPath);
                SetTextBoxByBytes(pngBytes);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
            {
                this.textBox4.AppendText("暂无数据，无法保存！");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.png|*.png";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog.FileName, ImageByte);
            }
        }

      
        /// <summary>
        /// 从微信内存中读取PNG图片数据，并显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtReadWxMemoryPng_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
            this.textBox4.Clear();
            this.pictureBox1.Image = null;

            if (IsWxRunning() == false)
            {
                this.textBox4.AppendText("微信没有找到！" + Environment.NewLine);
                return;
            }

            //如果“基址”选择框被选择，文本框中的数字是图片的开始地址的地址
            if (this.cbBassAddress.Checked == true)
            {
                //指针地址
                int pointerAddress = int.Parse(
                    this.textBox2.Text.Replace("0x", "").Replace("0X", ""),
                    System.Globalization.NumberStyles.HexNumber
                    );

                //数据真实地址
                Byte[] readStartAddress = GetWxDataBytes((IntPtr)(pointerAddress), 4);
                if (readStartAddress == null)
                {
                    this.textBox4.AppendText("地址数据读取失败！" + Environment.NewLine);
                    return;
                }

                int imageAddress = (int)(readStartAddress[0]) +
                    (int)(readStartAddress[1] << 8) +
                    (int)(readStartAddress[2] << 16) +
                    (int)(readStartAddress[3] << 24);

                //内存地址未4字节对齐
                if (imageAddress % 4 != 0)
                {
                    this.textBox4.AppendText("指针指向的内存地址有误，内存地址必须为4的倍数！" + Environment.NewLine);
                    this.textBox3.Text = "0x00000000";
                    return;
                }


                //数据长度
                int dataLength = int.Parse(
                    this.textBox3.Text.Replace("0x", "").Replace("0X", ""),
                    System.Globalization.NumberStyles.HexNumber
                    );

                //读取数据
                Byte[] myBytes = GetWxDataBytes((IntPtr)(imageAddress), dataLength);

                if (myBytes == null)
                {
                    this.textBox4.AppendText("未读取到数据！" + Environment.NewLine);
                    return;
                }

                if (myBytes.Length < 8)
                {
                    this.textBox4.AppendText("读取的内容不足8位！" + Environment.NewLine);
                    return;
                }
                if ((myBytes[0] != (byte)0x89) ||
                    (myBytes[1] != (byte)0x50) ||
                    (myBytes[2] != (byte)0x4E) ||
                    (myBytes[3] != (byte)0x47) ||
                    (myBytes[4] != (byte)0x0D) ||
                    (myBytes[5] != (byte)0x0A) ||
                    (myBytes[6] != (byte)0x1A) ||
                    (myBytes[7] != (byte)0x0A))
                {
                    this.textBox4.AppendText("非PNG图片！" + Environment.NewLine);
                    return;
                }

                //把数据显示到图片框
                try
                {
                    MemoryStream memoryStream = new MemoryStream();
                    memoryStream.Write(myBytes, 0, myBytes.Length);

                    Image image = Image.FromStream(memoryStream);
                    this.pictureBox1.Image = image;
                }
                catch
                {
                    this.textBox4.AppendText("图片无效！" + Environment.NewLine);
                }
                SetTextBoxByBytes(myBytes);
            }
            else
            {
                //设置每次读取的字节数
                int readSize = 1024 * 10;
                //每次读取到的内存数据，加入到列表中
                List<byte[]> ReadMemoryList = new List<byte[]>();
                //第几次读取内存
                int readCount = 0;
                //最大的读取次数
                int readMaxCount = 1024;
                //读取地址
                int readStartAddress = int.Parse(this.textBox2.Text.Replace("0x", "").Replace("0X", ""), System.Globalization.NumberStyles.HexNumber);
                //最近读取的数据中，有效数据数量
                int lastByteLength = 0;
                //末尾字符串
                String iEndString = "0000000049454E44AE426082";
                String tempIEND = "";
                //循环读取内存
                do
                {
                    //本次读取内存的地址
                    int readAddress = readStartAddress + readSize * (readCount++);
                    //判断是否达到做大的读取次数
                    if (readCount >= readMaxCount)
                    {
                        this.textBox4.AppendText("已经达到最大读取次数，不再读取内存..." + Environment.NewLine);
                        break;
                    }

                    //读取内存
                    Byte[] myBytes = GetWxDataBytes((IntPtr)readAddress, readSize);

                    //判断第一次读取是否有效
                    if (readCount == 1)
                    {
                        //判断是否读取成功
                        if (myBytes == null)
                        {
                            this.textBox4.AppendText("没有读取内容！" + Environment.NewLine);
                            return;
                        }

                        //判断读取的数据是否有效
                        if (myBytes.Length < 8)
                        {
                            this.textBox4.AppendText("读取的内容不足8位！" + Environment.NewLine);
                            return;
                        }
                        if ((myBytes[0] != (byte)0x89) ||
                            (myBytes[1] != (byte)0x50) ||
                            (myBytes[2] != (byte)0x4E) ||
                            (myBytes[3] != (byte)0x47) ||
                            (myBytes[4] != (byte)0x0D) ||
                            (myBytes[5] != (byte)0x0A) ||
                            (myBytes[6] != (byte)0x1A) ||
                            (myBytes[7] != (byte)0x0A))
                        {
                            this.textBox4.AppendText("非PNG图片！" + Environment.NewLine);
                            return;
                        }
                    }

                    //从第二次开始
                    //检查读取内存数据是否成功
                    if (myBytes == null)
                    {
                        this.textBox4.AppendText("PNG图片读取结束！" + Environment.NewLine);
                        break;
                    }
                    //读取成功，添加到列表中
                    ReadMemoryList.Add(myBytes);

                    //判断本次是否读取到PNG图片的末尾数据IEND
                    Boolean finished = false;
                    for (int i = 0; i < myBytes.Length; i++)
                    {
                        lastByteLength = i;
                        if (myBytes[i] == (byte)0)
                        {
                            tempIEND = "";
                        }
                        else
                        {
                            tempIEND += myBytes[i].ToString("X2");
                            //找到PNG图片的末尾
                            if ("00000000" + tempIEND == iEndString)
                            {
                                finished = true;
                                this.textBox4.AppendText("到达PNG图片末尾！" + Environment.NewLine);
                                break;
                            }
                        }
                    }
                    //找到PNG图片的末尾
                    if (finished == true) break;
                } while (true);
                //图片数据大小
                int bytelength = readSize * (ReadMemoryList.Count - 1) + lastByteLength;

                //重新整理图片数据
                byte[] data = new byte[bytelength];
                //定义一个可变字符串
                StringBuilder stringBuilder = new StringBuilder(bytelength * 3);

                //把ReadMemoryList中的数据拼接到一个数组中
                for (int i = 0; i < ReadMemoryList.Count - 1; i++)
                {
                    Array.ConstrainedCopy(ReadMemoryList[i], 0, data, readSize * i, readSize);
                }
                //最后一个数组
                Array.ConstrainedCopy(ReadMemoryList[ReadMemoryList.Count - 1], 0, data, readSize * (ReadMemoryList.Count - 1), lastByteLength);

                //把数据显示到图片框
                try
                {
                    MemoryStream memoryStream = new MemoryStream();
                    memoryStream.Write(data, 0, data.Length);
                    Image image = Image.FromStream(memoryStream);
                    this.pictureBox1.Image = image;
                }
                catch
                {
                    this.textBox4.AppendText("图片无效！" + Environment.NewLine);
                }
                SetTextBoxByBytes(data);
            }
        }

        /// <summary>
        /// 读取内存数据
        /// </summary>
        /// <param name="lpBaseAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        Byte[] GetWxDataBytes(IntPtr lpBaseAddress, int length = 1024)
        {
            byte[] data = new byte[length];
            if (ReadProcessMemory(WxProcess.Handle, lpBaseAddress, data, length, 0) == 0)
            {
                //读取内存失败！
                return null;
            }
            return data;
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
           IntPtr hProcess, //正在读取内存的进程句柄。句柄必须具有PROCESS_VM_READ访问权限。
           IntPtr lpBaseAddress,    //指向要从中读取的指定进程中的基址的指针。在发生任何数据传输之前，系统会验证基本地址和指定大小的内存中的所有数据是否都可以进行读访问，如果无法访问，则该函数将失败。
           byte[] lpBuffer,  //指向缓冲区的指针，该缓冲区从指定进程的地址空间接收内容。
           int nSize,    //要从指定进程读取的字节数。
           int lpNumberOfBytesRead //指向变量的指针，该变量接收传输到指定缓冲区的字节数。如果lpNumberOfBytesRead为NULL，则忽略该参数。
         );

        /// <summary>
        /// 点击复选框，变动数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataLength();
        }
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateDataLength();
        }
        private void UpdateDataLength()
        {
            this.textBox1.Clear();
            this.textBox4.Clear();
            this.textBox5.Text = "0x" + 0.ToString("X8");

            //this.pictureBox1.Image = null;

            //确保微信运行中
            if (IsWxRunning() == false)
            {
                this.textBox4.AppendText("微信没有找到！" + Environment.NewLine);
                return;

            }

            int pointerAddress;
            if (int.TryParse(
                this.textBox2.Text.Replace("0x", "").Replace("0X", ""),
                System.Globalization.NumberStyles.HexNumber,
                NumberFormatInfo.InvariantInfo,
                out pointerAddress)
                == false)
            {
                this.textBox4.AppendText("内存地址非数字！" + Environment.NewLine);
                return;
            }

            //内存地址未4字节对齐
            if (pointerAddress % 4 != 0)
            {
                this.textBox4.AppendText("内存地址必须为4的倍数！" + Environment.NewLine);
                this.textBox3.Text = "0x00000000";
                return;
            }

            //检查复选框是否选择
            if (cbBassAddress.Checked == false)
            {
                this.textBox3.Text = "0x00000000";
                this.textBox5.Text = this.textBox2.Text;
                return;
            }

            //图片数存放的地址
            Byte[] readStartAddress = GetWxDataBytes((IntPtr)(pointerAddress), 4);
            if (readStartAddress == null)
            {
                this.textBox4.AppendText("地址数据读取失败！" + Environment.NewLine);
                return;
            }

            int imageAddress = (int)(readStartAddress[0]) +
                (int)(readStartAddress[1] << 8) +
                (int)(readStartAddress[2] << 16) +
                (int)(readStartAddress[3] << 24);

            //内存地址未4字节对齐
            if (imageAddress % 4 != 0)
            {
                this.textBox4.AppendText("指针指向的内存地址有误，内存地址必须为4的倍数！" + Environment.NewLine);
                this.textBox3.Text = "0x00000000";
                return;
            }

            this.textBox5.Text = "0x" + imageAddress.ToString("X8");

            //指针下一个Int，保存的是图片的长度
            int lengthAddress = pointerAddress + 4;

            //读出图片的长度
            Byte[] byteLength = GetWxDataBytes((IntPtr)(lengthAddress), 4);
            if (byteLength == null)
            {
                this.textBox3.Text = "0x00000000";
                return;
            }

            //整理数据
            int length = (int)(byteLength[0]) +
                (int)(byteLength[1] << 8) +
                (int)(byteLength[2] << 16) +
                (int)(byteLength[3] << 24);

            //使用Windows的mspaint画出的最小的Png图片(1像素)，大小为1052字节
            //PNG头 IDHR长度0xF
            //PMG末尾IEND长度0xC
            if (length < 0xF + 0xC)
            {
                this.textBox4.AppendText("内存中图片长度无效（" + length + "）！" + Environment.NewLine);
                this.textBox3.Text = "0x00000000";
                return;
            }


            //将长度数据填写到文本框
            this.textBox3.Text = "0x" + length.ToString("X8");
        }
    }
}
