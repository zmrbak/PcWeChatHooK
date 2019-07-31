using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiSdk;


namespace L045HttpApiDEMO
{
    class Program
    {
        static List<Tuple<Object, int, ENUM_MSG_TYPE>> msgList = new List<Tuple<object, int, ENUM_MSG_TYPE>>();
        //发送消息的机器人
        static String RobotWxid = "wxid_tkdqfcgq5ldr22";
        static Boolean RobotFound = false;

        //要准备发送消息的群
        static String RoomWxid = "4637525423@chatroom";
        static Boolean RoomFound = false;

        //要准备发送给消息的人
        static String ContractWxid = "wxid_k2d9oduqc9lc22";
        static Boolean ContractFound = false;

        static void Main(string[] args)
        {
            //获取机器人列表，取第一个
            CheckRobot();

            //检查联系人
            CheckContract();

            //获取群列表
            CheckRoom();

            //给个人发送消息
            SendContractMsg();

            //发送群消息
            SendRoomMsg();

            Console.ReadLine();
        }

        private static void CheckContract()
        {
            Console.WriteLine("检测指定的联系人......");
            var contactList = AirCatApi.GetContactListData(RobotWxid, "", "");
            foreach (var contact in contactList.Members)
            {
                Console.WriteLine(contact.Wxid + "\t" + contact.WxName);
                if (ContractWxid == contact.Wxid)
                {
                    Console.WriteLine("OK!");
                    ContractFound = true;
                    //break;
                }
            }
            if (ContractFound == false)
            {
                Console.WriteLine("指定的联系人未找到，程序无法继续执行！");
                Console.ReadLine();
                return;
            }
        }

        private static void SendContractMsg()
        {
            Console.WriteLine("加载要发送的消息......");
            LoadMsg();

            Console.WriteLine("开始发送消息......");
            foreach (var item in msgList)
            {
                //发送消息之前，先延时（秒）；
                Console.WriteLine("延时:" + item.Item2);

                int currentLineCursor = Console.CursorTop;

                //发送的时间
                DateTime sendTime = DateTime.Now.AddSeconds(item.Item2);
                while (sendTime > DateTime.Now)
                {
                    Console.Write((sendTime - DateTime.Now).Seconds.ToString());
                    Thread.Sleep(100);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, currentLineCursor);
                }
                String result = "";
                switch (item.Item3)
                {
                    case ENUM_MSG_TYPE.TEXT:
                        Console.WriteLine("发送文本消息：" + item.Item1.ToString());
                        AirCatApi.SendContractText(RobotWxid, ContractWxid, item.Item1.ToString());
                        break;
                    case ENUM_MSG_TYPE.IMAG:
                        Console.WriteLine("发送图片消息：" + item.Item1.ToString());
                        AirCatApi.SendContractImage(RobotWxid, ContractWxid, item.Item1.ToString());
                        break;
                    case ENUM_MSG_TYPE.LINK:
                        Console.WriteLine("发送链接消息：" + item.Item1.ToString());
                        JObject jObject = (JObject)(item.Item1);
                        String title = jObject["Title"].ToString();
                        String text = jObject["Text"].ToString();
                        String targetUrl = jObject["TargetUrl"].ToString();
                        String imgUrl = jObject["ImgUrl"].ToString();
                        String icoUrl = jObject["IcoUrl"].ToString();
                        AirCatApi.SendContractLink(RobotWxid, ContractWxid, title, text, targetUrl, imgUrl, icoUrl);
                        break;
                    case ENUM_MSG_TYPE.FILE:
                        Console.WriteLine("发送文件：" + item.Item1.ToString());
                        AirCatApi.SendContractFile(RobotWxid, ContractWxid, item.Item1.ToString());
                        break;
                    case ENUM_MSG_TYPE.EMOJI:
                        Console.WriteLine("发送文件：" + item.Item1.ToString());
                        AirCatApi.SendContractEmoji(RobotWxid, ContractWxid, item.Item1.ToString());
                        break;
                    case ENUM_MSG_TYPE.VEDIO:
                        Console.WriteLine("发送文件：" + item.Item1.ToString());
                        result = AirCatApi.SendContractVideo(RobotWxid, ContractWxid, item.Item1.ToString());
                        break;
                    default:
                        break;
                }
                Console.WriteLine(result);
            }

            Console.WriteLine("发送完毕!");
        }

        private static void SendRoomMsg()
        {
            Console.WriteLine("加载要发送的消息......");
            LoadMsg();

            Console.WriteLine("开始发送消息......");
            foreach (var item in msgList)
            {
                //发送消息之前，先延时（秒）；
                Console.WriteLine("延时:" + item.Item2);

                int currentLineCursor = Console.CursorTop;

                //发送的时间
                DateTime sendTime = DateTime.Now.AddSeconds(item.Item2);
                while (sendTime > DateTime.Now)
                {
                    Console.Write((sendTime - DateTime.Now).Seconds.ToString());
                    Thread.Sleep(100);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, currentLineCursor);
                }
                String result = "";
                switch (item.Item3)
                {
                    case ENUM_MSG_TYPE.TEXT:
                        Console.WriteLine("发送文本消息：" + item.Item1.ToString());
                        AirCatApi.SendRoomText(RobotWxid, RoomWxid, item.Item1.ToString());
                        break;
                    case ENUM_MSG_TYPE.IMAG:
                        Console.WriteLine("发送图片消息：" + item.Item1.ToString());
                        AirCatApi.SendRoomImage(RobotWxid, RoomWxid, item.Item1.ToString());
                        break;
                    case ENUM_MSG_TYPE.LINK:
                        Console.WriteLine("发送链接消息：" + item.Item1.ToString());
                        JObject jObject = (JObject)(item.Item1);
                        String title = jObject["Title"].ToString();
                        String text = jObject["Text"].ToString();
                        String targetUrl = jObject["TargetUrl"].ToString();
                        String imgUrl = jObject["ImgUrl"].ToString();
                        String icoUrl = jObject["IcoUrl"].ToString();
                        AirCatApi.SendRoomLink(RobotWxid, RoomWxid, title, text, targetUrl, imgUrl, icoUrl);
                        break;
                    case ENUM_MSG_TYPE.FILE:
                        Console.WriteLine("发送文件：" + item.Item1.ToString());
                        AirCatApi.SendRoomFile(RobotWxid, RoomWxid, item.Item1.ToString());
                        break;
                    case ENUM_MSG_TYPE.EMOJI:
                        Console.WriteLine("发送文件：" + item.Item1.ToString());
                        AirCatApi.SendRoomEmoji(RobotWxid, RoomWxid, item.Item1.ToString());
                        break;
                    case ENUM_MSG_TYPE.VEDIO:
                        Console.WriteLine("发送文件：" + item.Item1.ToString());
                        result = AirCatApi.SendRoomVideo(RobotWxid, RoomWxid, item.Item1.ToString());

                        break;
                    default:
                        break;
                }
                Console.WriteLine(result);
            }

            Console.WriteLine("发送完毕!");
        }

        private static void CheckRoom()
        {
            Console.WriteLine("检测指定的群......");
            var roomList = AirCatApi.GetRoomListData(RobotWxid, "", "");
            foreach (var room in roomList.Rooms)
            {
                Console.WriteLine(room.RoomWxid + "\t" + room.RoomName);
                if (RoomWxid == room.RoomWxid)
                {
                    Console.WriteLine("OK!");
                    RoomFound = true;
                    //break;
                }
            }
            if (RoomFound == false)
            {
                Console.WriteLine("指定的群未找到，程序无法继续执行！");
                Console.ReadLine();
                return;
            }
        }

        public static void AddMsg(Object msg, int delaySeconds, ENUM_MSG_TYPE mSG_TYPE)
        {
            msgList.Add(new Tuple<object, int, ENUM_MSG_TYPE>(msg, delaySeconds, mSG_TYPE));
        }

        public static void LoadMsg()
        {
            msgList.Clear();

            AddMsg("大家好!", 0, ENUM_MSG_TYPE.TEXT);
            AddMsg(@"bd_logo1.png", 5, ENUM_MSG_TYPE.IMAG);

            JObject jObject = new JObject();
            jObject.Add("Title", "测试Title");
            jObject.Add("Text", "测试Text");
            jObject.Add("TargetUrl", "https://www.so.com/");
            jObject.Add("ImgUrl", "https://p0.ssl.qhimgs4.com/dmtfd/178_100_/t01b059b09ac74ce297.webp?size=500x532");
            jObject.Add("IcoUrl", "https://p.ssl.qhimg.com/t01d1f1a2ae31e3c3e4.png");
            AddMsg(jObject, 10, ENUM_MSG_TYPE.LINK);

            AddMsg(@"bd_logo1.png", 6, ENUM_MSG_TYPE.FILE);
            AddMsg(@"qd.gif", 7, ENUM_MSG_TYPE.EMOJI);

            //发视频很慢，但可以成功
            AddMsg(@"vedio.mp4", 4, ENUM_MSG_TYPE.VEDIO);

        }

        static void CheckRobot()
        {
            Console.WriteLine("检测在线机器人......");
            var RobotList = AirCatApi.GetRobotListData();
            foreach (var item in RobotList.Robots)
            {
                //Console.WriteLine(item.RobotWxid + "\t" + item.LoginTime + "\t" + item.RobotWxName);
                if (RobotWxid == item.RobotWxid)
                {
                    Console.WriteLine("OK!");
                    RobotFound = true;
                    break;
                }
            }
            if (RobotFound == false)
            {
                Console.WriteLine("指定的机器人未找到，程序无法继续执行！");
                Console.ReadLine();
                return;
            }
        }
    }

    public enum ENUM_MSG_TYPE
    {
        TEXT = 0,
        IMAG = 1,
        LINK = 2,
        FILE = 3,
        EMOJI = 4,
        VEDIO = 5
    }
}
