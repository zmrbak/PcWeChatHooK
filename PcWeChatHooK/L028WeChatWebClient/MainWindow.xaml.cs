using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WxWebClient;

namespace L028WeChatWebClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //实例化Http客户端
        HttpClient httpClient = new HttpClient();
        //上次选择的文件名
        String lastSelectedFile = "";
        //服务器返回的二进制数据内容
        byte[] serverReturnData;
        //发送的参数计数
        int paramIndex = 0;
        //动态数据集合
        private ObservableCollection<ParamNameValue> observableCollection = new ObservableCollection<ParamNameValue>();
        public ObservableCollection<ParamNameValue> ObservableCollection
        {
            get { return observableCollection; }
            set { observableCollection = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void HttpClient_OnException(Exception e)
        {
            button.IsEnabled = true;
            MessageBox.Show(e.Message + e.StackTrace);
        }

        private void HttpClient_OnDataReturn(byte[] returnData)
        {
            button.IsEnabled = true;

            serverReturnData = returnData;
            ViewReturnData();
        }

        //PC微信探秘学习
        private void Bt_Help_Click(object sender, RoutedEventArgs e)
        {
            String weChatStudyUrl = "https://study.163.com/course/introduction/1209042813.htm?utm_source=qq&utm_medium=webShare&utm_campaign=share&utm_content=courseIntro&share=2&shareId=480000001858469";
            Process.Start(weChatStudyUrl);
        }

        //Blend学习
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String blendStudyUrl = "https://study.163.com/course/introduction/1208988814.htm?utm_source=qq&utm_medium=webShare&utm_campaign=share&utm_content=courseIntro&share=2&shareId=480000001858469";
            Process.Start(blendStudyUrl);
        }

        /// <summary>
        /// 显示返回的数据
        /// </summary>
        private void ViewReturnData()
        {
            //以文本形式显示
            if (radioButton2.IsChecked == true)
            {
                this.textBox3.Text = Encoding.UTF8.GetString(serverReturnData) + Environment.NewLine;
            }
            else
            {
                //以16进制的方式显示
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte item in serverReturnData)
                {
                    stringBuilder.Append(item.ToString("X2") + " ");
                }
                this.textBox3.Text = stringBuilder.ToString() + Environment.NewLine;
            }
        }

        /// <summary>
        /// 返回数据中，点击字符串选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rb_String_Click(object sender, RoutedEventArgs e)
        {
            if (serverReturnData == null) return;

            this.textBox3.Text = Encoding.UTF8.GetString(serverReturnData);
        }

        /// <summary>
        /// 返回数据中，点击16进制选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rb_Hex_Click(object sender, RoutedEventArgs e)
        {
            if (serverReturnData == null) return;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte item in serverReturnData)
            {
                stringBuilder.Append(item.ToString("X2") + " ");
            }
            textBox3.Text = stringBuilder.ToString().TrimEnd();
        }

        /// <summary>
        /// POST GET切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cb_Method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = comboBoxPostGetChoice.SelectedItem as ComboBoxItem;
            if (comboBoxItem == null) return;

            if (dockPanel_Get == null) return;
            if (stackPanel_Get_CheckBoxes == null) return;

            if (comboBoxItem.Content.ToString() == "GET")
            {
                stackPanel_Get_CheckBoxes.Visibility = Visibility.Visible;
                dockPanel_Get.Visibility = Visibility.Visible;

                stackPanel_Post_CheckBoxes.Visibility = Visibility.Collapsed;
                dockPanel_Post.Visibility = Visibility.Collapsed;
            }
            else
            {
                stackPanel_Get_CheckBoxes.Visibility = Visibility.Collapsed;
                dockPanel_Get.Visibility = Visibility.Collapsed;

                stackPanel_Post_CheckBoxes.Visibility = Visibility.Visible;
                dockPanel_Post.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 目标URL修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tb_Url_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                httpClient.Url = new Uri(this.tb_SendUrl.Text);
            }
            catch { }

            //刷新显示
            UpdateListView();
        }


        /// <summary>
        /// 添加数据的项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StackPanelAdd_Click(object sender, RoutedEventArgs e)
        {
            //被点击的Button
            Button button = sender as Button;
            if (button == null) return;

            //父控件
            StackPanel stackPanel = VisualTreeHelper.GetParent(button) as StackPanel;
            if (stackPanel == null) return;

            //祖父控件
            DockPanel dockPanel = VisualTreeHelper.GetParent(stackPanel) as DockPanel;
            if (dockPanel == null) return;

            //叔父控件
            StackPanel stackPanel_1 = dockPanel.Children[1] as StackPanel;
            if (stackPanel_1 == null) return;

            //堂兄弟控件
            DockPanel dockPanel_1 = stackPanel_1.Children[0] as DockPanel;
            if (dockPanel_1 == null) return;

            //复制堂兄弟控件
            String radioButton1String = XamlWriter.Save(dockPanel_1);
            StringReader stringReader = new StringReader(radioButton1String);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            UIElement clonedChild = (UIElement)XamlReader.Load(xmlReader);
            DockPanel dockPanel_2 = clonedChild as DockPanel;

            if (dockPanel_2 == null) return;

            //POST 文件
            Button bt_FileSelect = dockPanel_2.Children[1] as Button;
            if (bt_FileSelect != null)
            {
                if (bt_FileSelect.Content.ToString() == "文件...")
                {
                    bt_FileSelect.Click += Bt_SelectFile_Click;
                    TextBox tb_FileSelect = dockPanel_2.Children[2] as TextBox;
                    if (tb_FileSelect != null)
                    {
                        tb_FileSelect.Text = "";
                    }
                }
            }

            //GET 数据
            TextBox textBox = dockPanel_2.Children[2] as TextBox;
            if (textBox != null)
            {
                textBox.Text = "";
            }

            //POST 数据
            StackPanel stackPanelData = dockPanel_2.Children[0] as StackPanel;
            if (stackPanelData != null)
            {
                RadioButton radioButtonData = stackPanelData.Children[0] as RadioButton;
                if (radioButtonData != null)
                {
                    radioButtonData.IsChecked = true;

                    StackPanel stackPanel_12 = dockPanel_2.Children[2] as StackPanel;
                    if (stackPanel_12 != null)
                    {
                        //字符串
                        TextBox textBoxData0 = stackPanel_12.Children[0] as TextBox;
                        if (textBoxData0 != null)
                        {
                            textBoxData0.Text = "";
                        }

                        //16进制
                        TextBox textBoxData1 = stackPanel_12.Children[1] as TextBox;
                        if (textBoxData1 != null)
                        {
                            textBoxData1.Text = "";
                            textBoxData1.KeyUp += TextBoxData1_KeyUp;
                            textBoxData1.PreviewMouseLeftButtonUp += TextBox_PreviewMouseLeftButtonUp;
                        }
                    }
                }
                stackPanelData.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(StackPanelClick_RadioButton), true);
            }

            //POST GET 字段
            if (dockPanel_2.Children.Count >= 4)
            {
                TextBox textBoxParam1 = dockPanel_2.Children[0] as TextBox;
                TextBox textBoxParam2 = dockPanel_2.Children[3] as TextBox;
                if (textBoxParam1 != null && textBoxParam2 != null)
                {
                    paramIndex++;
                    textBoxParam1.Text = "p" + paramIndex.ToString("d4");
                    textBoxParam2.Text = "";
                }
            }

            dockPanel_2.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(StackPanelClick_ChildRemove), true);
            stackPanel_1.Children.Add(dockPanel_2);
        }

        /// <summary>
        /// 16进制文本框，改变内容后，校验内容，重新排版
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxData1_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            //记录光标位置
            int selectionStart = textBox.SelectionStart;

            StringBuilder stringBuilder = new StringBuilder();
            String HexString = textBox.Text.Replace(" ", "").ToUpper();

            //剔除不正常的字符
            foreach (Char item in HexString)
            {
                if (item <= 'F' && item >= 'A')
                {
                    stringBuilder.Append(item.ToString());
                }
                if (item <= '9' && item >= '0')
                {
                    stringBuilder.Append(item.ToString());
                }
            }

            //重新排版
            HexString = stringBuilder.ToString();
            stringBuilder.Clear();
            for (int i = 0; i < HexString.Length / 2; i++)
            {
                stringBuilder.Append(HexString.Substring(i * 2, 2) + " ");
            }

            if (HexString.Length % 2 == 1)
            {
                stringBuilder.Append(HexString.Substring(HexString.Length - 1, 1));
            }

            textBox.Text = stringBuilder.ToString();
            if (selectionStart < HexString.Length)
            {
                textBox.SelectionStart = selectionStart - 1;
            }
            else
            {
                textBox.SelectionStart = selectionStart + 1;
            }

            //刷新显示
            UpdateListView();
        }

        /// <summary>
        /// 删除元素事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StackPanelClick_ChildRemove(object sender, RoutedEventArgs e)
        {
            DockPanel dockPanel = sender as DockPanel;
            if (dockPanel == null) return;

            StackPanel stackPanel = VisualTreeHelper.GetParent(dockPanel) as StackPanel;
            if (stackPanel == null) return;

            Button button = e.OriginalSource as Button;
            if (button == null) return;
            if (button.Content.ToString() == "文件...") return;

            stackPanel.Children.Remove(dockPanel);
        }

        /// <summary>
        /// 点击 GET 数据 控制复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_GetData_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;

            if (checkBox.IsChecked == true)
            {
                groupBox_GetData.Visibility = Visibility.Visible;
            }
            else
            {
                groupBox_GetData.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 点击 GET 字段 控制复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_GetParam_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;

            if (checkBox.IsChecked == true)
            {
                groupBox_GetParam.Visibility = Visibility.Visible;
            }
            else
            {
                groupBox_GetParam.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 点击 POST 数据 控制复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_PostData_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;

            if (checkBox.IsChecked == true)
            {
                groupBox_PostData.Visibility = Visibility.Visible;
            }
            else
            {
                groupBox_PostData.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 点击 POST 字段 控制复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_PostParam_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;

            if (checkBox.IsChecked == true)
            {
                groupBox_PostParam.Visibility = Visibility.Visible;
            }
            else
            {
                groupBox_PostParam.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 点击 POST 文件 控制复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_PostFile_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;

            if (checkBox.IsChecked == true)
            {
                groupBox_PostFile.Visibility = Visibility.Visible;
            }
            else
            {
                groupBox_PostFile.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Get
            groupBox_GetData.Visibility = Visibility.Collapsed;
            groupBox_GetParam.Visibility = Visibility.Collapsed;

            //Post
            groupBox_PostData.Visibility = Visibility.Collapsed;
            groupBox_PostParam.Visibility = Visibility.Collapsed;
            groupBox_PostFile.Visibility = Visibility.Collapsed;

            //折叠Post的添加项
            stackPanel_Post_CheckBoxes.Visibility = Visibility.Collapsed;
            dockPanel_Post.Visibility = Visibility.Visible;

            httpClient.Url = new Uri("http://127.0.0.1:8421");
            httpClient.OnDataReturn += HttpClient_OnDataReturn;
            httpClient.OnException += HttpClient_OnException;

            this.DataContext = this;

            //启动Web服务器
            HttpServer httpServer = new HttpServer();
            httpServer.OnPostDataReceived += HttpServer_OnPostDataReceived;
            httpServer.OnGetDataReceived += HttpServer_OnGetDataReceived;
            httpServer.Start(8421);

        }

        private void HttpServer_OnGetDataReceived(System.Collections.Generic.List<ParamNameValue> paramList, System.Net.HttpListenerResponse response)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + "收到GET信息：" + Environment.NewLine);
            foreach (ParamNameValue item in paramList)
            {
                //Console.WriteLine(item.ParamName + "=" + item.ParamValue);
                stringBuilder.Append(item.ParamName + "=" + item.ParamValue + Environment.NewLine);
            }

            //回应客户端
            Stream output = response.OutputStream;
            byte[] buffer = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private void HttpServer_OnPostDataReceived(System.Collections.Generic.List<ChunkData> chunkDataList, System.Net.HttpListenerResponse response)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + "收到POST信息：" + Environment.NewLine);

            foreach (ChunkData item in chunkDataList)
            {
                switch (item.DataType)
                {
                    case DataTypeEnum.Str:
                        //Console.WriteLine(item.ContentType);
                        //Console.WriteLine(item.ParamValue);
                        stringBuilder.Append("字符串：\t" + item.ParamValue + Environment.NewLine);
                        break;
                    case DataTypeEnum.Hex:
                        //Console.WriteLine(item.ContentType);
                        StringBuilder hexString = new StringBuilder();
                        foreach (byte hexdata in item.HexData)
                        {
                            //Console.Write(((int)hexdata).ToString("X2") + " ");
                            hexString.Append(((int)hexdata).ToString("X2") + " ");
                        }
                        //Console.WriteLine();
                        stringBuilder.Append("16进制字串：\t" + hexString.ToString() + Environment.NewLine);
                        break;
                    case DataTypeEnum.File:
                        //Console.WriteLine(item.FileName + "," + item.HexData.Length);
                        stringBuilder.Append("文件：\t" + item.FileName + "(" + item.HexData.Length + ")" + Environment.NewLine);
                        break;
                    case DataTypeEnum.Param:
                        //Console.WriteLine(item.ParamName + "=" + item.ParamValue);
                        stringBuilder.Append("文本内容：\t" + item.ParamName + "=" + item.ParamValue + Environment.NewLine);
                        break;
                    default:
                        break;
                }
            }

            //回应客户端
            Stream output = response.OutputStream;
            byte[] buffer = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private void Bt_DenyDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("首行数据为内置数据，不可删除！", "错误", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK);
        }

        private void Bt_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件|*.*";
            if (lastSelectedFile != "")
            {
                openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(lastSelectedFile);
            }
            if (openFileDialog.ShowDialog() != true) return;
            lastSelectedFile = openFileDialog.FileName;

            //找到父节点
            //被点击的Button
            Button button = sender as Button;
            if (button == null) return;

            //父控件
            DockPanel dockPanel = VisualTreeHelper.GetParent(button) as DockPanel;
            if (dockPanel == null) return;

            //兄弟控件
            TextBox textBox = dockPanel.Children[2] as TextBox;
            textBox.Text = lastSelectedFile;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateListView();
        }
        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            UpdateListView();
        }
        private void ComboBoxPostGetChoice_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateListView();
        }
        private void UpdateListView()
        {
            if (listViewSend == null) return;

            listViewSend.ItemsSource = null;
            Uri serverUri = null;
            try
            {
                serverUri = new Uri(this.tb_SendUrl.Text);
            }
            catch
            {
                return;
            }
            observableCollection.Clear();
            observableCollection.Add(new ParamNameValue("提交方式", comboBoxPostGetChoice.Text));
            observableCollection.Add(new ParamNameValue("目标主机", serverUri.Host));
            observableCollection.Add(new ParamNameValue("目标端口", serverUri.Port.ToString()));
            observableCollection.Add(new ParamNameValue("绝对Uri", serverUri.AbsoluteUri));
            observableCollection.Add(new ParamNameValue("原始字符串", serverUri.OriginalString));

            if (comboBoxPostGetChoice.Text == "POST")
            {
                //数据
                if (checkBox_PostData.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_PostData.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    int i = 0;
                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;

                        StackPanel stackPanel_11 = dockPanel_1.Children[0] as StackPanel;
                        if (stackPanel_11 == null) continue;


                        StackPanel stackPanel_12 = dockPanel_1.Children[2] as StackPanel;
                        if (stackPanel_12 == null) continue;


                        RadioButton radioButton = stackPanel_11.Children[0] as RadioButton;
                        if (radioButton == null) continue;

                        // Content="字符串"
                        i++;
                        if (radioButton.IsChecked == true)
                        {
                            TextBox textBox = stackPanel_12.Children[0] as TextBox;
                            if (textBox == null) continue;
                            observableCollection.Add(new ParamNameValue("字符串" + i, textBox.Text));
                        }
                        else
                        {
                            TextBox textBox = stackPanel_12.Children[1] as TextBox;
                            if (textBox == null) continue;
                            observableCollection.Add(new ParamNameValue("16进制" + i, textBox.Text));
                        }
                    }
                }
                //字段
                if (checkBox_PostParam.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_PostParam.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;


                        TextBox textBox0 = dockPanel_1.Children[0] as TextBox;
                        if (textBox0 == null) continue;
                        if (textBox0.Text == "") continue;

                        TextBox textBox3 = dockPanel_1.Children[3] as TextBox;
                        if (textBox3 == null) continue;

                        observableCollection.Add(new ParamNameValue("提交字段", textBox0.Text + "=" + textBox3.Text));
                    }
                }
                //文件
                if (checkBox_PostFile.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_PostFile.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;


                        TextBox textBox2 = dockPanel_1.Children[2] as TextBox;
                        if (textBox2 == null) continue;
                        if (textBox2.Text == "") continue;

                        observableCollection.Add(new ParamNameValue("提交文件", textBox2.Text));
                    }
                }
            }
            else
            {
                //数据
                if (checkBox_GetData.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_GetData.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    int i = 0;
                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;

                        TextBox textBox = dockPanel_1.Children[2] as TextBox;
                        if (textBox == null) continue;

                        i++;
                        observableCollection.Add(new ParamNameValue("GET字符串" + i.ToString("d2"), textBox.Text));
                    }
                }
                //字段
                if (checkBox_GetParam.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_GetParam.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;


                        TextBox textBox0 = dockPanel_1.Children[0] as TextBox;
                        if (textBox0 == null) continue;
                        if (textBox0.Text == "") continue;

                        TextBox textBox3 = dockPanel_1.Children[3] as TextBox;
                        if (textBox3 == null) continue;

                        observableCollection.Add(new ParamNameValue("GET字段", textBox0.Text + "=" + textBox3.Text));
                    }
                }
            }

            listViewSend.ItemsSource = observableCollection;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt_SendData_Click(object sender, RoutedEventArgs e)
        {
            Uri serverUri = null;
            try
            {
                serverUri = new Uri(this.tb_SendUrl.Text);
            }
            catch
            {
                MessageBox.Show("目标主机不正确,无法发送数据！", "错误", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            if (comboBoxPostGetChoice.Text == "POST")
            {
                httpClient.MethodInit(Methods.POST);

                //数据
                if (checkBox_PostData.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_PostData.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;

                        StackPanel stackPanel_11 = dockPanel_1.Children[0] as StackPanel;
                        if (stackPanel_11 == null) continue;


                        StackPanel stackPanel_12 = dockPanel_1.Children[2] as StackPanel;
                        if (stackPanel_12 == null) continue;


                        RadioButton radioButton = stackPanel_11.Children[0] as RadioButton;
                        if (radioButton == null) continue;

                        // Content="字符串"
                        if (radioButton.IsChecked == true)
                        {
                            TextBox textBox = stackPanel_12.Children[0] as TextBox;
                            if (textBox == null) continue;
                            httpClient.AddString(textBox.Text);
                        }
                        else
                        {
                            TextBox textBox = stackPanel_12.Children[1] as TextBox;
                            if (textBox == null) continue;
                            httpClient.AddHex(textBox.Text);
                        }
                    }
                }
                //字段
                if (checkBox_PostParam.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_PostParam.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;


                        TextBox textBox0 = dockPanel_1.Children[0] as TextBox;
                        if (textBox0 == null) continue;

                        TextBox textBox3 = dockPanel_1.Children[3] as TextBox;

                        httpClient.AddParam(textBox0.Text, textBox3.Text);
                    }
                }
                //文件
                if (checkBox_PostFile.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_PostFile.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;

                        TextBox textBox2 = dockPanel_1.Children[2] as TextBox;
                        if (textBox2 == null) continue;

                        httpClient.AddFile(textBox2.Text);
                    }
                }
            }
            else
            {
                httpClient.MethodInit(Methods.GET);

                //数据
                if (checkBox_GetData.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_GetData.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;

                        TextBox textBox = dockPanel_1.Children[2] as TextBox;
                        if (textBox == null) continue;

                        httpClient.AddString(textBox.Text);
                    }
                }
                //字段
                if (checkBox_GetParam.IsChecked == true)
                {
                    DockPanel dockPanel = groupBox_GetParam.Content as DockPanel;
                    if (dockPanel == null) return;

                    StackPanel stackPanel = dockPanel.Children[1] as StackPanel;
                    if (stackPanel == null) return;

                    foreach (var item in stackPanel.Children)
                    {
                        DockPanel dockPanel_1 = item as DockPanel;
                        if (dockPanel_1 == null) continue;


                        TextBox textBox0 = dockPanel_1.Children[0] as TextBox;
                        if (textBox0 == null) continue;

                        TextBox textBox3 = dockPanel_1.Children[3] as TextBox;
                        if (textBox3 == null) continue;

                        httpClient.AddParam(textBox0.Text, textBox3.Text);
                    }
                }
            }
            httpClient.Start();
        }

        /// <summary>
        /// 十六进制文本框，关闭输入法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            // 关闭输入法
            if (textBox.IsInputMethodEnabled == true)
            {
                InputMethod.SetIsInputMethodEnabled(textBox, false);
            }
        }

        private void StackPanelClick_RadioButton(object sender, RoutedEventArgs e)
        {
            SetTextBoxForStringOrHex(e);
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SetTextBoxForStringOrHex(e);
        }
        private static void SetTextBoxForStringOrHex(RoutedEventArgs e)
        {
            RadioButton radioButton = e.OriginalSource as RadioButton;
            if (radioButton == null) return;

            //父控件
            StackPanel stackPanel = VisualTreeHelper.GetParent(radioButton) as StackPanel;
            if (stackPanel == null) return;

            //祖父控件
            DockPanel dockPanel = VisualTreeHelper.GetParent(stackPanel) as DockPanel;
            if (dockPanel == null) return;

            //叔父控件
            if (dockPanel.Children.Count < 3) return;

            StackPanel stackPanel_1 = dockPanel.Children[2] as StackPanel;
            if (stackPanel_1 == null) return;

            //堂兄弟控件
            TextBox textBox1 = stackPanel_1.Children[0] as TextBox;
            if (textBox1 == null) return;

            TextBox textBox2 = stackPanel_1.Children[1] as TextBox;
            if (textBox2 == null) return;

            if (radioButton.Content.ToString() == "字符串")
            {
                textBox1.Visibility = Visibility.Visible;
                textBox2.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBox1.Visibility = Visibility.Collapsed;
                textBox2.Visibility = Visibility.Visible;
            }
        }


    }
}
