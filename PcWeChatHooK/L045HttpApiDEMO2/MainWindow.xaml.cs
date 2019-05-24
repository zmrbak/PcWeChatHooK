using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
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
using WebApiSdk;

namespace L045HttpApiDEMO2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //判断配置是否已变动
        static Boolean IsDirty1 = false;
        static Boolean IsDirty2 = false;

        //默认机器人微信号
        static String DefaultRobotWxid = ConfigurationManager.AppSettings["DefaultRobotWxid"];
        //默认群聊微信号
        static String DefaultRoomWxid = ConfigurationManager.AppSettings["DefaultRoomWxid"];
        //默认好友微信号
        static String DefaultContractWxid = ConfigurationManager.AppSettings["DefaultContractWxid"];
        //可爱猫WebApi的IP地址
        static String AirWebApiIP = ConfigurationManager.AppSettings["AirWebApiIP"];
        //可爱猫WebApi的Web端口
        static int AirWebApiPort = int.Parse(ConfigurationManager.AppSettings["AirWebApiPort"]);
        //本地管理端的Web端口
        static int LocalManagerPort = int.Parse(ConfigurationManager.AppSettings["ManagerPort"]);
        //共享的通讯密钥
        static String ShareToken = ConfigurationManager.AppSettings["Token"];

        //机器人列表
        List<String> robotWxidList = new List<string>();
        string selectedRobotWxid = "";

        //聊天室列表
        Dictionary<String, List<String>> roomList = new Dictionary<string, List<string>>();


        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AirCatApi airCatApi = new AirCatApi();
            airCatApi.AirWebApiIP = AirWebApiIP;
            airCatApi.AirWebApiPort = AirWebApiPort;
            airCatApi.ServicePort = LocalManagerPort;
            airCatApi.AirWebApiToken = ShareToken;


            Type type = airCatApi.GetType();
            MethodInfo[] methods = type.GetMethods();

            List<String> CommandList = new List<string>();
            foreach (MethodInfo method in methods)
            {
                if (method.IsStatic == false) continue;
                if (method.ReturnParameter.ParameterType.Name != "String") continue;
                CommandList.Add(method.Name);

            }
            CommandList.Sort();
            this.CobmoxCommands.ItemsSource = CommandList;


            airCatApi.OnDebug += AirCatApi_OnDebug;
            airCatApi.ServiceStart();

            ServerIP.Text = AirWebApiIP;
            ServerPort.Text = AirWebApiPort.ToString();
            ManagerIP.Text = "127.0.0.1";
            ManagerPort.Text = LocalManagerPort.ToString();
            Token.Text = ShareToken;
            TestRobotWxid.Text = DefaultRobotWxid;
            TestRoomWxid.Text = DefaultRoomWxid;
            TestContractWxid.Text = DefaultContractWxid;


            new Thread(() =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    CobmoxCommands.SelectedIndex = CobmoxCommands.Items.IndexOf("GetRobotList");
                }));
            }).Start();

        }

        void WriteLog(String json)
        {
            String message = "";
            if (json.Substring(0, 1) == "[")
            {
                message = JArray.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            else if (json.Substring(0, 1) == "{")
            {
                message = JObject.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            else
            {
                message = json;
            }


            new Thread(() =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.serverMessage2.AppendText(message + Environment.NewLine);
                }));
            }).Start();
        }

        private void AirCatApi_OnDebug(object sender, DebugEventArgs e)
        {
            WriteLog(e.DebugMessage);
        }

        private void CobmoxCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CommandParams.Children.Clear();
            CommandParams.Orientation = Orientation.Vertical;
            ComboBox comboBox = (ComboBox)sender;

            //检查参数
            AirCatApi airCatApi = new AirCatApi();
            Type type = airCatApi.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.IsStatic == false) continue;
                if (method.ReturnParameter.ParameterType.Name != "String") continue;


                //if (method.ReturnParameter.) continue;
                if (method.Name == comboBox.SelectedValue.ToString())
                {
                    Grid grid = new Grid();
                    // grid.ShowGridLines = true;
                    ColumnDefinition columnDefinition1 = new ColumnDefinition();
                    columnDefinition1.Width = GridLength.Auto;
                    ColumnDefinition columnDefinition2 = new ColumnDefinition();
                    grid.ColumnDefinitions.Add(columnDefinition1);
                    grid.ColumnDefinitions.Add(columnDefinition2);

                    TextBlock_ApiComments.Text = ("名称：" + method.Name + ((ApiCommentAttribute)method.GetCustomAttribute(typeof(ApiCommentAttribute))).ApiComment).Trim();
                    TextBlock_ApiComments.Visibility = Visibility.Visible;

                    ParameterInfo[] parameterInfos = method.GetParameters();
                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        grid.RowDefinitions.Add(rowDefinition);
                    }

                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = parameterInfos[i].Name;
                        textBlock.Padding = new Thickness(3);
                        textBlock.Margin = new Thickness(3);
                        grid.Children.Add(textBlock);
                        Grid.SetRow(textBlock, i);
                        Grid.SetColumn(textBlock, 0);


                        ComboBox comboBox2 = this.FindName(parameterInfos[i].Name) as ComboBox;
                        if (comboBox2 != null)
                        {
                            CommandParams.UnregisterName(parameterInfos[i].Name);
                        }

                        TextBox textBox2 = this.FindName(parameterInfos[i].Name) as TextBox;
                        if (textBox2 != null)
                        {
                            CommandParams.UnregisterName(parameterInfos[i].Name);
                        }

                        if (parameterInfos[i].Name == "FileName")
                        {
                            StackPanel stackPanel = new StackPanel();
                            TextBox textBox = new TextBox();
                            textBox.Name = parameterInfos[i].Name;
                            textBox.Padding = new Thickness(3);
                            textBox.Margin = new Thickness(3);
                            textBox.IsReadOnly = true;
                            CommandParams.RegisterName(parameterInfos[i].Name, textBox);
                            stackPanel.Children.Add(textBox);

                            Button button = new Button();
                            button.Content = "浏览";
                            button.Click += Button_Click1;
                            button.Tag = parameterInfos[i].Name;
                            stackPanel.Children.Add(button);

                            grid.Children.Add(stackPanel);
                            Grid.SetRow(stackPanel, i);
                            Grid.SetColumn(stackPanel, 1);
                        }
                        else
                        {
                            if (parameterInfos[i].Name == "RobotWxid")
                            {
                                //登陆的用户列表
                                ComboBox comboBox1 = new ComboBox();
                                comboBox1.Name = parameterInfos[i].Name;
                                comboBox1.Padding = new Thickness(3);
                                comboBox1.Margin = new Thickness(3);
                                comboBox1.SelectionChanged += ComboBox1_SelectionChanged;
                                comboBox1.SelectedIndex = 0;

                                foreach (String item in robotWxidList)
                                {
                                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                                    comboBoxItem.Content = item;

                                    if (selectedRobotWxid == item)
                                    {
                                        comboBoxItem.IsSelected = true;
                                    }

                                    comboBox1.Items.Add(comboBoxItem);
                                }

                                grid.Children.Add(comboBox1);
                                Grid.SetRow(comboBox1, i);
                                Grid.SetColumn(comboBox1, 1);
                                CommandParams.RegisterName(parameterInfos[i].Name, comboBox1);
                            }
                            //else if (parameterInfos[i].Name == "GetRoomList")
                            //{

                            //}
                            else
                            {
                                TextBox textBox = new TextBox();
                                textBox.Name = parameterInfos[i].Name;
                                textBox.Padding = new Thickness(3);
                                textBox.Margin = new Thickness(3);

                                if (parameterInfos[i].Name == "RoomWxid")
                                {
                                    textBox.Text = DefaultRoomWxid;
                                }
                                else if (parameterInfos[i].Name == "Wxid")
                                {
                                    textBox.Text = DefaultContractWxid;
                                }
                                else if (parameterInfos[i].Name == "AtWxid")
                                {
                                    textBox.Text = DefaultContractWxid;
                                }
                                grid.Children.Add(textBox);
                                Grid.SetRow(textBox, i);
                                Grid.SetColumn(textBox, 1);
                                CommandParams.RegisterName(parameterInfos[i].Name, textBox);
                            }
                        }
                    }
                    CommandParams.Children.Add(grid);
                }
            }
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null) return;

            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.IsSelected == true)
                {
                    selectedRobotWxid = item.Content.ToString();
                    break;
                }
            }

        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                TextBox textBox2 = this.FindName(((Button)sender).Tag.ToString()) as TextBox;
                if (textBox2 != null)
                {
                    textBox2.Text = openFileDialog.FileName;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CobmoxCommands.SelectedValue == null) return;
            CommandRun(false);
        }

        private void CommandRun(Boolean ClassData)
        {
            serverMessage.Text = "";

            AirCatApi airCatApi = new AirCatApi();
            Type type = airCatApi.GetType();
            MethodInfo[] methods = type.GetMethods();

            String CommnadRun = CobmoxCommands.SelectedValue.ToString();
            if (ClassData == true)
            {
                CommnadRun += "Data";
            }

            List<String> CommandList = new List<string>();
            foreach (MethodInfo method in methods)
            {
                if (method.IsStatic == false) continue;
                //if (method.ReturnParameter.ParameterType.Name != "String") continue;

                if (method.Name == CommnadRun)
                {
                    ParameterInfo[] parameterInfos = method.GetParameters();
                    object[] obj = new object[parameterInfos.Length];
                    for (int j = 0; j < parameterInfos.Length; j++)
                    {
                        TextBox textBox = this.FindName(parameterInfos[j].Name) as TextBox;
                        if (textBox != null)
                        {
                            obj[j] = textBox.Text;
                        }
                        else
                        {
                            ComboBox comboBox = this.FindName(parameterInfos[j].Name) as ComboBox;
                            if (comboBox != null)
                            {
                                foreach (ComboBoxItem item in comboBox.Items)
                                {
                                    if (item.IsSelected == true)
                                    {
                                        obj[j] = item.Content.ToString();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    object message = "";
                    try
                    {
                        message = method.Invoke(airCatApi, obj);
                        String json = message.ToString();
                        if (json == "") return;
                        if (json.Substring(0, 1) == "[")
                        {
                            serverMessage.Text = JArray.Parse(message.ToString()).ToString(Newtonsoft.Json.Formatting.Indented);
                        }
                        else
                        {
                            serverMessage.Text = JObject.Parse(message.ToString()).ToString(Newtonsoft.Json.Formatting.Indented);
                        }

                        //获取登陆微信列表
                        if (CommnadRun == "GetRobotList")
                        {
                            robotWxidList.Clear();
                            JObject jObject = JObject.Parse(message.ToString());
                            JArray jArray = JArray.Parse(jObject["Robots"].ToString());
                            foreach (var item in jArray)
                            {
                                JObject j = JObject.Parse(item.ToString());
                                robotWxidList.Add(j["RobotWxid"].ToString());
                            }
                        }

                        //获取群列表
                        if (CommnadRun == "GetRoomList")
                        {

                            JObject jObject = JObject.Parse(message.ToString());
                            string robotWxid = jObject["RobotWxid"].ToString();

                            //查找群列表中的数据
                            List<String> rooms = new List<string>();
                            foreach (var item in roomList)
                            {
                                if (item.Key == robotWxid)
                                {
                                    rooms = item.Value;
                                    break;
                                }
                            }

                            JArray jArray = JArray.Parse(jObject["Rooms"].ToString());
                            foreach (var item in jArray)
                            {
                                JObject j = JObject.Parse(item.ToString());
                                string roomWxid = j["RoomWxid"].ToString();

                                Boolean roomExist = false;
                                foreach (var room in rooms)
                                {
                                    if (room == roomWxid)
                                    {
                                        roomExist = true;
                                        break;
                                    }
                                }
                                if (roomExist == false)
                                {
                                    rooms.Add(roomWxid);
                                }
                            }
                            roomList[robotWxid] = rooms;
                        }
                    }
                    catch (Exception ex)
                    {
                        serverMessage.Text = message.ToString();
                        MessageBox.Show(ex.Message + ex.StackTrace);
                    }
                    return;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (CobmoxCommands.SelectedValue == null) return;
            CommandRun(true);
        }

        private void ServerIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ServerIP.Text != AirWebApiIP)
            {
                IsDirty1 = true;
            }
            SetSaveButtonStatus();

        }

        private void ServerPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ServerPort.Text != AirWebApiPort.ToString())
            {
                IsDirty1 = true;
            }
            SetSaveButtonStatus();

        }

        private void ManagerPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ManagerPort.Text != LocalManagerPort.ToString())
            {
                IsDirty1 = true;
            }
            SetSaveButtonStatus();
        }

        private void Token_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Token.Text != ShareToken)
            {
                IsDirty1 = true;
            }
            SetSaveButtonStatus();
        }

        private void SetSaveButtonStatus()
        {
            if (IsDirty1 == false)
            {
                SaveConfig.IsEnabled = false;
            }
            else
            {
                SaveConfig.IsEnabled = true;
            }
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            AirWebApiIP = ServerIP.Text;
            AirWebApiPort = int.Parse(ServerPort.Text);
            LocalManagerPort = int.Parse(ManagerPort.Text);
            ShareToken = Token.Text;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (IsKeyExist("AirWebApiIP") == true)
            {
                config.AppSettings.Settings["AirWebApiIP"].Value = AirWebApiIP;
            }
            else
            {
                config.AppSettings.Settings.Add("AirWebApiIP", AirWebApiIP);
            }

            if (IsKeyExist("AirWebApiPort") == true)
            {
                config.AppSettings.Settings["AirWebApiPort"].Value = AirWebApiPort.ToString();
            }
            else
            {
                config.AppSettings.Settings.Add("AirWebApiPort", AirWebApiPort.ToString());
            }

            if (IsKeyExist("ManagerPort") == true)
            {
                config.AppSettings.Settings["ManagerPort"].Value = LocalManagerPort.ToString();
            }
            else
            {
                config.AppSettings.Settings.Add("ManagerPort", LocalManagerPort.ToString());
            }

            if (IsKeyExist("Token") == true)
            {
                config.AppSettings.Settings["Token"].Value = ShareToken;
            }
            else
            {
                config.AppSettings.Settings.Add("Token", ShareToken);
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            MessageBox.Show("配置已经更新！");
        }

        private Boolean IsKeyExist(string Key)
        {
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == Key)
                {
                    return true;
                }
            }
            return false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            serverMessage.Text = "";

            JObject jObject = new JObject();
            jObject.Add("Cmd", "GetServerStatus");

            new Thread(() =>
            {
                RemoteHttpTest(jObject);
            }).Start();
            new Thread(() =>
            {
                LocalHttpTest(jObject);
            }).Start();


        }

        private void LocalHttpTest(JObject jObject)
        {
            //检测WebApi端
            String myServerIP = "";
            String myServerPort = "";
            String myToken = "";

            this.Dispatcher.Invoke(new Action(() =>
            {
                myServerIP = "127.0.0.1";
                myServerPort = ManagerPort.Text;
                myToken = Token.Text;
            }));

            String testMessage = HttpPost(myServerIP, int.Parse(myServerPort), myToken, jObject.ToString(Newtonsoft.Json.Formatting.None));
            if (testMessage != "")
            {
                new Thread(() =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        serverMessage.AppendText("---------------------------------管理端---------------------------------" + Environment.NewLine);
                        serverMessage.AppendText(testMessage + Environment.NewLine);
                    }));
                }).Start();

            }
        }

        private void RemoteHttpTest(JObject jObject)
        {
            //检测WebApi端
            String myServerIP = "";
            String myServerPort = "";
            String myToken = "";

            this.Dispatcher.Invoke(new Action(() =>
            {
                myServerIP = ServerIP.Text;
                myServerPort = ServerPort.Text;
                myToken = Token.Text;
            }));

            String testMessage = HttpPost(myServerIP, int.Parse(myServerPort), myToken, jObject.ToString(Newtonsoft.Json.Formatting.None));
            if (testMessage != "")
            {
                new Thread(() =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        serverMessage.AppendText("--------------------------------WebApi--------------------------------" + Environment.NewLine);
                        serverMessage.AppendText(testMessage + Environment.NewLine);
                    }));
                }).Start();
            }
        }

        private static string HttpPost(String SercverIP, int ServerPort, String ShareToken, String JsonString)
        {
            try
            {
                //拼接URL
                String urlPath = "http://" + SercverIP + ":" + ServerPort + "/?token=" + ShareToken;
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(urlPath);

                //post请求
                myRequest.Method = "POST";

                //utf-8编码
                byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(JsonString);

                myRequest.ContentLength = buf.Length;
                myRequest.Timeout = 5000;

                //指定为json否则会出错
                myRequest.ContentType = "application/json";
                myRequest.MaximumAutomaticRedirections = 1;
                myRequest.AllowAutoRedirect = true;
                Stream newStream = myRequest.GetRequestStream();
                newStream.Write(buf, 0, buf.Length);
                newStream.Close();

                //获得接口返回值
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string ReqResult = reader.ReadToEnd();
                reader.Close();
                myResponse.Close();

                //写入日志
                return ReqResult;
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            serverMessage.Clear();
            serverMessage2.Clear();
        }

        private void TestRobotWxid_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TestRobotWxid.Text != DefaultRobotWxid)
            {
                IsDirty2 = true;
            }
            SetSaveDefaultButton();
        }

        private void TestRoomWxid_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TestRoomWxid.Text != DefaultRoomWxid)
            {
                IsDirty2 = true;
            }
            SetSaveDefaultButton();
        }

        private void TestContractWxid_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TestContractWxid.Text != DefaultContractWxid)
            {
                IsDirty2 = true;
            }
            SetSaveDefaultButton();
        }


        private void SetSaveDefaultButton()
        {
            if (IsDirty2 == false)
            {
                SaveDefaultButton.IsEnabled = false;
            }
            else
            {
                SaveDefaultButton.IsEnabled = true;
            }
        }


        private void SaveDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultRobotWxid = TestRobotWxid.Text;
            DefaultRoomWxid = TestRoomWxid.Text;
            DefaultContractWxid = TestContractWxid.Text;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (IsKeyExist("DefaultRobotWxid") == true)
            {
                config.AppSettings.Settings["DefaultRobotWxid"].Value = DefaultRobotWxid;
            }
            else
            {
                config.AppSettings.Settings.Add("DefaultRobotWxid", DefaultRobotWxid);
            }

            if (IsKeyExist("DefaultRoomWxid") == true)
            {
                config.AppSettings.Settings["DefaultRoomWxid"].Value = DefaultRoomWxid;
            }
            else
            {
                config.AppSettings.Settings.Add("DefaultRoomWxid", DefaultRoomWxid);
            }

            if (IsKeyExist("DefaultContractWxid") == true)
            {
                config.AppSettings.Settings["DefaultContractWxid"].Value = DefaultContractWxid;
            }
            else
            {
                config.AppSettings.Settings.Add("DefaultContractWxid", DefaultContractWxid);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            MessageBox.Show("配置已经更新！");
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CobmoxCommands.Width = ButtonRun.ActualWidth;
        }
    }
}
