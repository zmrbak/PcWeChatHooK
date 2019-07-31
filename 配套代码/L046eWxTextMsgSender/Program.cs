using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiSdk;

namespace L046eWxTextMsgSender
{

    class Program
    {
        //要准备发送消息的群
        static String RoomWxid = "5031368048@chatroom";

        //要准备发送给消息的人
        static String ContractWxid = "wxid_k2d9oduqc9lc22";

        //测试信息
        static String DemoTextMessage = "时间:" + DateTime.Now.ToString();

        static void Main(string[] args)
        {
            //启动本地Web服务器
            AirCatApi airCatApi = new AirCatApi();
            airCatApi.OnDebug += AirCatApi_OnDebug;
            airCatApi.ServiceStart();


            DemoGetServerStatus();
            Thread.Sleep(2000);

            DemoSendContractText();
            Thread.Sleep(2000);

            DemoSendRoomText();
            Thread.Sleep(2000);

            Console.ReadLine();
        }

        /// <summary>
        /// 调试信息输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AirCatApi_OnDebug(object sender, DebugEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        /// <summary>
        /// 获取微信端信息
        /// </summary>
        private static void DemoGetServerStatus()
        {
            try
            {
                var Result = AirCatApi.GetServerStatusData();
                Console.WriteLine(Result.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        private static void DemoSendContractText()
        {
            try
            {
                var Result = AirCatApi.SendContractTextData("", ContractWxid, DemoTextMessage);
                Console.WriteLine(Result.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void DemoSendRoomText()
        {
            try
            {
                var Result = AirCatApi.SendRoomText("", RoomWxid, DemoTextMessage);
                Console.WriteLine(Result.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
