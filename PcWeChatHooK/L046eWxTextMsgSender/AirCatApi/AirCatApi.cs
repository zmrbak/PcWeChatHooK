using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiSdk
{
    public partial class AirCatApi
    {
        /// <summary>
        /// 直接拉好友进群
        /// </summary>
        /// <param name="ClusterID"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    Wxid：指定联系人的微信号
功能：直接拉好友进群（可爱猫2.0内测版暂不支持）
")]
        public static String AddContractToRoom(String RobotWxid, String RoomWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("Wxid", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static AddContractToRoomClassData AddContractToRoomData(String RobotWxid, String RoomWxid, String Wxid)
        {
            String json = AddContractToRoom(RobotWxid, RoomWxid, Wxid);
            return JsonConvert.DeserializeObject<AddContractToRoomClassData>(json);
        }
        /// <summary>
        /// 同意某人的好友请求
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    VerifyMsg：好友请求信息
功能：同意某人的好友请求
")]
        public static String AgreeFriend(String RobotWxid, String Msg)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static AgreeFriendClassData AgreeFriendData(String RobotWxid, String Msg)
        {
            String json = AgreeFriend(RobotWxid, Msg);
            return JsonConvert.DeserializeObject<AgreeFriendClassData>(json);
        }
        /// <summary>
        /// 接收好友转账信息
        /// </summary>
        /// <param name="PayId"></param>
        /// <param name="Wxid"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    PayId：支付的ID
功能：接收好友转账信息
")]
        public static String ConfirmPay(String RobotWxid, String Wxid, String Msg)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static ConfirmPayClassData ConfirmPayData(String RobotWxid, String Wxid, String Msg)
        {
            String json = ConfirmPay(RobotWxid, Wxid, Msg);
            return JsonConvert.DeserializeObject<ConfirmPayClassData>(json);
        }
        /// <summary>
        /// 添加好友（可爱猫2.0内测版暂不支持）
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    Msg：发请求的消息内容
功能：添加好友（可爱猫2.0内测版暂不支持）
")]
        public static String AddUserToContract(String RobotWxid, String Wxid, String Msg)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("Wxid", Wxid);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static AddUserToContractClassData AddUserToContractData(String RobotWxid, String Wxid, String Msg)
        {
            String json = AddUserToContract(RobotWxid, Wxid, Msg);
            return JsonConvert.DeserializeObject<AddUserToContractClassData>(json);
        }
        /// <summary>
        /// 服务器检查
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：无
功能：获取WebApi服务器状态。
")]
        public static String GetServerStatus()
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetServerStatusClassData GetServerStatusData()
        {
            String json = GetServerStatus();
            return JsonConvert.DeserializeObject<GetServerStatusClassData>(json);
        }
        /// <summary>
        /// 检查WebApiServer上登陆的所有机器人微信号
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：无
功能：检查WebApiServer上登陆的所有机器人微信号。
")]
        public static String GetRobotList()
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetRobotListClassData GetRobotListData()
        {
            String json = GetRobotList();
            return JsonConvert.DeserializeObject<GetRobotListClassData>(json);
        }
        /// <summary>
        /// 获取所有群列表
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Start：从列表中第几个群开始返回，默认1
    Number：返回少个群信息，默认50
功能：检查WebApiServer上,指定机器人微信号的群列表。
")]
        public static String GetRoomList(String RobotWxid, String Start, String Number)
        {
            if (Start == "") Start = "1";
            if (Number == "") Number = "50";

            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Start", Start);
            jObject.Add("Number", Number);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetRoomListClassData GetRoomListData(String RobotWxid, String Start, String Number)
        {
            String json = GetRoomList(RobotWxid, Start, Number);
            return JsonConvert.DeserializeObject<GetRoomListClassData>(json);
        }
        /// <summary>
        /// 获取指定群的群信息
        /// </summary>
        /// <param name="wx_id"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
功能：获取指定群的群信息。
")]
        public static String GetRoomInfo(String RobotWxid, String RoomWxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetRoomInfoClassData GetRoomInfoData(String RobotWxid, String RoomWxid)
        {
            String json = GetRoomInfo(RobotWxid, RoomWxid);
            return JsonConvert.DeserializeObject<GetRoomInfoClassData>(json);
        }
        /// <summary>
        /// 通过群号查找群名
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
功能：通过群号查找群名。
")]
        public static String GetRoomName(String RobotWxid, String RoomWxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetRoomNameClassData GetRoomNameData(String RobotWxid, String RoomWxid)
        {
            String json = GetRoomName(RobotWxid, RoomWxid);
            return JsonConvert.DeserializeObject<GetRoomNameClassData>(json);
        }
        /// <summary>
        /// 通过微信群的群名找到群微信号（可能有多个重名的微信群，但群微信号不一样）。
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomName：微信群的群名
功能：通过微信群的群名找到群微信号（可能有多个重名的微信群，但群微信号不一样）。
")]
        public static String GetRoomWxids(String RobotWxid, String RoomName)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomName", RoomName);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetRoomWxidsClassData GetRoomWxidsData(String RobotWxid, String RoomName)
        {
            String json = GetRoomWxids(RobotWxid, RoomName);
            return JsonConvert.DeserializeObject<GetRoomWxidsClassData>(json);
        }
        /// <summary>
        /// 向群中发送文本消息
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    Msg：发送的文本消息
功能：向群中发送文本消息。
")]
        public static String SendRoomText(String RobotWxid, String RoomWxid, String Msg)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomClassData SendRoomTextData(String RobotWxid, String RoomWxid, String Msg)
        {
            String json = SendRoomText(RobotWxid, RoomWxid, Msg);
            return JsonConvert.DeserializeObject<SendRoomClassData>(json);
        }
        /// <summary>
        /// 向群中发送文本消息，并且@他
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="AtWxid"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    AtWxid：发消息时@某人
    Msg：发送的文本消息
功能：向群中发送文本消息，并且@某人
")]
        public static String SendRoomTextAt(String RobotWxid, String RoomWxid, String AtWxid, String Msg)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("AtWxid", AtWxid);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomTextAtClassData SendRoomTextAtData(String RobotWxid, String RoomWxid, String AtWxid, String Msg)
        {
            String json = SendRoomTextAt(RobotWxid, RoomWxid, AtWxid, Msg);
            return JsonConvert.DeserializeObject<SendRoomTextAtClassData>(json);
        }
        /// <summary>
        /// 向群中发送文本消息，并且@他
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="AtWxid"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    AtWxid：发消息时@某人
    Msg：发送的文本消息
功能：向群中发送文本消息，并且@某人
")]
        public static String SendRoomTextAt2(String RobotWxid, String RoomWxid, String AtWxid,String AtName, String Msg)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("AtWxid", AtWxid);
            jObject.Add("AtName", AtName);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomTextAtClassData SendRoomTextAt2Data(String RobotWxid, String RoomWxid, String AtWxid, String AtName,String Msg)
        {
            String json = SendRoomTextAt2(RobotWxid, RoomWxid, AtWxid, AtName, Msg);
            return JsonConvert.DeserializeObject<SendRoomTextAtClassData>(json);
        }
        /// <summary>
        /// 修改群名
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="NewName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    NewName：新微信群名
功能：修改指定群的群名
")]
        public static String SendRoomRename(String RobotWxid, String RoomWxid, String NewName)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("RoomName", NewName);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomClassData SendRoomRenameData(String RobotWxid, String RoomWxid, String NewName)
        {
            String json = SendRoomRename(RobotWxid, RoomWxid, NewName);
            return JsonConvert.DeserializeObject<SendRoomClassData>(json);
        }
        /// <summary>
        /// 向群中发送分享链接
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="Title"></param>
        /// <param name="Text"></param>
        /// <param name="TargetUrl"></param>
        /// <param name="ImgUrl"></param>
        /// <param name="IconUrl"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    Title：链接大标题
    Text：链接内容摘要
    TargetUrl：跳转到的链接地址
    ImgUrl：显示的图片地址
    IconUrl：小图标地址
功能：向群中发送分享链接
")]
        public static String SendRoomLink(String RobotWxid, String RoomWxid, String Title, String Text, String TargetUrl, String ImgUrl, String IconUrl)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("Title", Title);
            jObject.Add("Text", Text);
            jObject.Add("TargetUrl", TargetUrl);
            jObject.Add("ImgUrl", ImgUrl);
            jObject.Add("IconUrl", IconUrl);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomClassData SendRoomLinkData(String RobotWxid, String RoomWxid, String Title, String Text, String TargetUrl, String ImgUrl, String IconUrl)
        {
            String json = SendRoomLink(RobotWxid, RoomWxid, Title, Text, TargetUrl, ImgUrl, IconUrl);
            return JsonConvert.DeserializeObject<SendRoomClassData>(json);
        }
        /// <summary>
        /// 从群中把某用户踢出
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="Wxid"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    Wxid：被踢出去的微信号
功能：把某用户从指定的群中踢出
")]
        public static String SendRoomKickUser(String RobotWxid, String RoomWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("Wxid", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomClassData SendRoomKickUserData(String RobotWxid, String RoomWxid, String Wxid)
        {
            String json = SendRoomKickUser(RobotWxid, RoomWxid, Wxid);
            return JsonConvert.DeserializeObject<SendRoomClassData>(json);
        }
        /// <summary>
        /// 向群中发送一个图片
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    ImageFile：图片的本地地址
功能：向群中发送一个图片
")]
        public static String SendRoomImage(String RobotWxid, String RoomWxid, String FileName)
        {
            ////需要对文件名进行限制
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("FileName", Path.GetFileName(FileName));
            jObject.Add("FileBase64", FileBase64(FileName));
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomClassData SendRoomImageData(String RobotWxid, String RoomWxid, String FileName)
        {
            String json = SendRoomImage(RobotWxid, RoomWxid, FileName);
            return JsonConvert.DeserializeObject<SendRoomClassData>(json);
        }
        /// <summary>
        /// 向群中发送一个文件
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    FileName：图片的本地地址
功能：向群中发送一个文件
")]
        public static String SendRoomFile(String RobotWxid, String RoomWxid, String FileName)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("FileName", Path.GetFileName(FileName));
            jObject.Add("FileBase64", FileBase64(FileName));
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomClassData SendRoomFileData(String RobotWxid, String RoomWxid, String FileName)
        {
            String json = SendRoomFile(RobotWxid, RoomWxid, FileName);
            return JsonConvert.DeserializeObject<SendRoomClassData>(json);
        }
        /// <summary>
        /// 向群中发送表情图标
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    EmojiFile：表情的本地地址
功能：向群中发送一个表情
")]
        public static String SendRoomEmoji(String RobotWxid, String RoomWxid, String FileName)
        {
            //需要对文件名进行限制

            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("FileBase64", FileBase64(FileName));
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomClassData SendRoomEmojiData(String RobotWxid, String RoomWxid, String FileName)
        {
            String json = SendRoomEmoji(RobotWxid, RoomWxid, FileName);
            return JsonConvert.DeserializeObject<SendRoomClassData>(json);
        }
        /// <summary>
        /// 向群中发送小视频
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="RoomWxid"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    VideoFile：小视频的本地地址
功能：向群中发送小视频
")]
        public static String SendRoomVideo(String RobotWxid, String RoomWxid, String FileName)
        {
            ////需要对文件名进行限制
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("FileName", FileName);
            jObject.Add("FileBase64", FileBase64(FileName));
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendRoomClassData SendRoomVideoData(String RobotWxid, String RoomWxid, String FileName)
        {
            String json = SendRoomVideo(RobotWxid, RoomWxid, FileName);
            return JsonConvert.DeserializeObject<SendRoomClassData>(json);
        }
        /// <summary>
        /// 为某联系人发送一个短视频
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <param name="VideoFile"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    VideoFile：小视频的本地地址
功能：为某联系人发送一个短视频
")]
        public static String SendContractVideo(String RobotWxid, String Wxid, String FileName)
        {
            ////需要对文件名进行限制
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            jObject.Add("FileName", Path.GetFileName(FileName));
            jObject.Add("FileBase64", FileBase64(FileName));
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendContractClassData SendContractVideoData(String RobotWxid, String RoomWxid, String FileName)
        {
            String json = SendContractVideo(RobotWxid, RoomWxid, FileName);
            return JsonConvert.DeserializeObject<SendContractClassData>(json);
        }
        /// <summary>
        /// 给联系人发送一条文本消息
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    Msg：发送和的文本消息
功能：给联系人发送一条文本消息
")]
        public static String SendContractText(String RobotWxid, String Wxid, String Msg)
        {
            ////需要对文件名进行限制
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendContractClassData SendContractTextData(String RobotWxid, String Wxid, String Msg)
        {
            String json = SendContractText(RobotWxid, Wxid, Msg);
            return JsonConvert.DeserializeObject<SendContractClassData>(json);
        }
        /// <summary>
        /// 对某联系人发送链接
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <param name="Title"></param>
        /// <param name="Text"></param>
        /// <param name="TargetUrl"></param>
        /// <param name="ImgUrl"></param>
        /// <param name="IconUrl"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    Title：链接大标题
    Text：链接内容摘要
    TargetUrl：跳转到的链接地址
    ImgUrl：显示的图片地址
    IconUrl：小图标地址
功能：向群中发送分享链接
")]
        public static String SendContractLink(String RobotWxid, String Wxid, String Title, String Text, String TargetUrl, String ImgUrl, String IconUrl)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            jObject.Add("Title", Title);
            jObject.Add("Text", Text);
            jObject.Add("TargetUrl", TargetUrl);
            jObject.Add("ImgUrl", ImgUrl);
            jObject.Add("IconUrl", IconUrl);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendContractClassData SendContractLinkData(String RobotWxid, String Wxid, String Title, String Text, String TargetUrl, String ImgUrl, String IconUrl)
        {
            String json = SendContractLink(RobotWxid, Wxid, Title, Text, TargetUrl, ImgUrl, IconUrl);
            return JsonConvert.DeserializeObject<SendContractClassData>(json);
        }
        /// <summary>
        /// 对某联系人发送图片
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    FileName：图片的本地地址
功能：向指定联系人发送一个图片
")]
        public static String SendContractImage(String RobotWxid, String Wxid, String FileName)
        {
            ////需要对文件名进行限制
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            jObject.Add("FileBase64", FileBase64(FileName));
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendContractClassData SendContractImageData(String RobotWxid, String Wxid, String FileName)
        {
            String json = SendContractImage(RobotWxid, Wxid, FileName);
            return JsonConvert.DeserializeObject<SendContractClassData>(json);
        }
        /// <summary>
        /// 向指定联系人发送一个文件
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    FileName：图片的本地地址
功能：向指定联系人发送一个文件
")]
        public static String SendContractFile(String RobotWxid, String Wxid, String FileName)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            jObject.Add("FileName", Path.GetFileName(FileName));
            jObject.Add("FileBase64", FileBase64(FileName));
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendContractClassData SendContractFileData(String RobotWxid, String Wxid, String FileName)
        {
            String json = SendContractFile(RobotWxid, Wxid, FileName);
            return JsonConvert.DeserializeObject<SendContractClassData>(json);
        }
        /// <summary>
        /// 为联系人发送一个表情
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    FileName：表情的本地地址
功能：为联系人发送一个表情
")]
        public static String SendContractEmoji(String RobotWxid, String Wxid, String FileName)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            jObject.Add("FileBase64", FileBase64(FileName));
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendContractClassData SendContractEmojiData(String RobotWxid, String Wxid, String FileName)
        {
            String json = SendContractEmoji(RobotWxid, Wxid, FileName);
            return JsonConvert.DeserializeObject<SendContractClassData>(json);
        }
        /// <summary>
        /// 删除某联系人（可爱猫2.0内测版暂未实现）
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
功能：删除某联系人（可爱猫2.0内测版暂未实现）
")]
        public static String SendContractDelete(String RobotWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendContractClassData SendContractDeleteData(String RobotWxid, String Wxid)
        {
            String json = SendContractDelete(RobotWxid, Wxid);
            return JsonConvert.DeserializeObject<SendContractClassData>(json);
        }
        /// <summary>
        /// 拉黑某联系人（可爱猫2.0内测版暂未实现）
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
功能：拉黑某联系人（可爱猫2.0内测版暂未实现）
")]
        public static String SendContractBlock(String RobotWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendContractClassData SendContractBlockData(String RobotWxid, String Wxid)
        {
            String json = SendContractBlock(RobotWxid, Wxid);
            return JsonConvert.DeserializeObject<SendContractClassData>(json);
        }
        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Start：开始的联系人序号（默认1）
    Number：本次提取的联系人数量（默认100）
功能：获取联系人列表
")]
        public static String GetContactList(String RobotWxid, String Start, String Number)
        {
            if (Start == "") Start = "1";
            if (Number == "") Number = "100";

            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Start", Start);
            jObject.Add("Number", Number);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetContactListClassData GetContactListData(String RobotWxid, String Start, String Number)
        {
            String json = GetContactList(RobotWxid, Start, Number);
            return JsonConvert.DeserializeObject<GetContactListClassData>(json);
        }
        /// <summary>
        /// 通过微信ID取得微信昵称，仅限联系人
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Start：开始的联系人序号（默认1）
    Number：本次提取的联系人数量（默认100）
功能：获取联系人列表
")]
        public static String GetContractName(String RobotWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetContractNameClassData GetContractNameData(String RobotWxid, String Wxid)
        {
            String json = GetContractName(RobotWxid, Wxid);
            return JsonConvert.DeserializeObject<GetContractNameClassData>(json);
        }

        /// <summary>
        /// 通过微信昵称取得联系人微信ID，仅限联系人
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Start：开始的联系人序号（默认1）
    Number：本次提取的联系人数量（默认100）
功能：获取联系人列表
")]
        public static String GetContractWxids(String RobotWxid, String WxName)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("WxName", WxName);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetContractWxidsClassData GetContractWxidsData(String RobotWxid, String WxName)
        {
            String json = GetContractWxids(RobotWxid, WxName);
            return JsonConvert.DeserializeObject<GetContractWxidsClassData>(json);
        }
        /// <summary>
        /// 获取用户结构
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Start：开始的联系人序号（默认1）
    Number：本次提取的微信User（非好友）人数量（默认100）
功能：获取联系人列表
")]
        public static String GetUserList(String RobotWxid, String Start, String Number)
        {
            if (Start == "") Start = "1";
            if (Number == "") Number = "100";

            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("Start", Start);
            jObject.Add("Number", Number);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetUserListClassData GetUserListData(String RobotWxid, String Start, String Number)
        {
            String json = GetUserList(RobotWxid, Start, Number);
            return JsonConvert.DeserializeObject<GetUserListClassData>(json);
        }
        /// <summary>
        /// 通过微信ID取得微信昵称，可能有多个，参考Api_GetContractName
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Start：开始的联系人序号（默认1）
    Number：本次提取的微信User（非好友）人数量（默认100）
功能：获取联系人列表
")]
        public static String GetUserName(String RobotWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("Start", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetUserNameClassData GetUserNameData(String RobotWxid, String Wxid)
        {
            String json = GetUserName(RobotWxid, Wxid);
            return JsonConvert.DeserializeObject<GetUserNameClassData>(json);
        }
        /// <summary>
        /// 通过微信ID取得微信昵称，可能有多个，参考Api_GetContractName
        /// </summary>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Start：开始的联系人序号（默认1）
    Number：本次提取的微信User（非好友）人数量（默认100）
功能：获取联系人列表
")]
        public static String GetUserWxids(String RobotWxid, String WxName)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("WxName", WxName);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetUserListClassData GetUserWxidsData(String RobotWxid, String WxName)
        {
            String json = GetUserWxids(RobotWxid, WxName);
            return JsonConvert.DeserializeObject<GetUserListClassData>(json);
        }
        /// <summary>
        /// 向可爱猫日志界面中填写一条内容
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    Msg：向可爱猫日中中发送的内容
功能：向可爱猫日志界面中填写一条内容
")]
        public static String SendLogs(string Msg)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SendLogsClassData SendLogsData(string Msg)
        {
            String json = SendLogs(Msg);
            return JsonConvert.DeserializeObject<SendLogsClassData>(json);
        }
        /// <summary>
        /// 订阅指定的微信公众账号
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：微信公众账号
功能：订阅指定的微信公众账号（可爱猫2.0内测版暂不支持）
")]
        public static String SumbitOfficialAccount(String RobotWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SumbitOfficialAccountClassData SumbitOfficialAccountData(String RobotWxid, String Wxid)
        {
            String json = SumbitOfficialAccount(RobotWxid, Wxid);
            return JsonConvert.DeserializeObject<SumbitOfficialAccountClassData>(json);
        }
        /// <summary>
        /// 邀请成员进群（链接形式）
        /// </summary>
        /// <param name="ClusterUsername"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    Wxid：指定联系人的微信号
功能：直接拉好友进群（可爱猫2.0内测版暂不支持）
")]
        public static String InviteContractToRoom(String RobotWxid, String RoomWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("Wxid", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static InviteContractToRoomClassData InviteContractToRoomData(String RobotWxid, String RoomWxid, String Wxid)
        {
            String json = InviteContractToRoom(RobotWxid, RoomWxid, Wxid);
            return JsonConvert.DeserializeObject<InviteContractToRoomClassData>(json);
        }
        /// <summary>
        /// 创建群聊
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
    Wxid1：指定联系人的微信号
功能：创建微信群，包括机器人自己在内至少有三人才能创建（可爱猫2.0内测版暂不支持）
")]
        public static String CreateRoom(String RobotWxid,String RoomName, String Wxid, String Wxid1)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomName", RoomName);
            jObject.Add("Wxid", Wxid);
            jObject.Add("Wxid1", Wxid1);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static CreateRoomClassData CreateRoomData(String RobotWxid, String RoomName, String Wxid, String Wxid1)
        {
            String json = CreateRoom(RobotWxid, RoomName, Wxid, Wxid1);
            return JsonConvert.DeserializeObject<CreateRoomClassData>(json);
        }
        /// <summary>
        /// 机器人自己退出某群
        /// </summary>
        /// <param name="ClusterID"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
功能：机器人自己退出某群（可爱猫2.0内测版暂不支持）
")]
        public static String QuitFromRoom(String RobotWxid, String RoomWxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static QuitFromRoomClassData QuitFromRoomData(String RobotWxid, String RoomWxid)
        {
            String json = QuitFromRoom(RobotWxid, RoomWxid);
            return JsonConvert.DeserializeObject<QuitFromRoomClassData>(json);
        }
        /// <summary>
        /// 修改群公告（需群主）
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    RoomWxid：微信群的微信号
    Wxid1：指定联系人的微信号
功能：修改群公告（需群主）（可爱猫2.0内测版暂不支持）
")]
        public static String SetRoomAnnouncement(String RobotWxid, String RoomWxid, String Msg)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("RoomWxid", RoomWxid);
            jObject.Add("Msg", Msg);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static SetRoomAnnouncementData SetRoomAnnouncementData(String RobotWxid, String RoomWxid, String Msg)
        {
            String json = SetRoomAnnouncement(RobotWxid, RoomWxid, Msg);
            return JsonConvert.DeserializeObject<SetRoomAnnouncementData>(json);
        }
        /// <summary>
        /// 提取联系人信息
        /// </summary>
        /// <param name="RobotWxid"></param>
        /// <param name="Wxid"></param>
        /// <returns></returns>
        [ApiComment(@"
参数：
    RobotWxid：机器人微信号
    Wxid：指定联系人的微信号
功能：提取联系人信息
")]
        public static String GetContactInfo(String RobotWxid, String Wxid)
        {
            JObject jObject = new JObject();
            jObject.Add("Cmd", (new StackTrace()).GetFrame(0).GetMethod().Name);
            jObject.Add("RobotWxid", RobotWxid);
            jObject.Add("Wxid", Wxid);
            return HttpPost(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        public static GetContactInfoData GetContactInfoData(String RobotWxid, String Wxid)
        {
            String json = GetContactInfo(RobotWxid, Wxid);
            return JsonConvert.DeserializeObject<GetContactInfoData>(json);
        }
        #region 内置非公开的方法

        /// <summary>
        /// 调试信息事件
        /// </summary>
        /// <param name="debugMessage"></param>
        private static void DebugMessage(String debugMessage)
        {
            DebugEventArgs debugEventArgs = new DebugEventArgs();
            debugEventArgs.DebugMessage = debugMessage;
            DebugEvent?.Invoke(null, debugEventArgs);
        }
        private static EventHandler<DebugEventArgs> DebugEvent;
        public event EventHandler<DebugEventArgs> OnDebug
        {
            add { DebugEvent += value; }
            remove { DebugEvent -= value; }
        }

        /// <summary>
        /// 向WebApi发送HttpPost请求
        /// </summary>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        private static string HttpPost(String JsonString)
        {
            try
            {
                DebugMessage("发送POST:" + JsonString);

                //拼接URL
                String urlPath = "http://" + airWebApiIP + ":" + airWebApiPort + "/?token=" + airWebApiToken;
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
                DebugMessage("收到POST:" + ReqResult);
                return ReqResult;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return "";
            }
        }
        /// <summary>
        /// 把一个文件转换成BASE64编码
        /// </summary>
        /// <param name="FileFullName"></param>
        /// <returns></returns>
        private static string FileBase64(string FileFullName)
        {
            if (File.Exists(FileFullName) != true)
            {
                return "";
            }

            FileStream fileStream = File.Open(FileFullName, FileMode.OpenOrCreate);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            fileStream.Close();
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 从一个BASE64字符串创建一个文件
        /// </summary>
        /// <param name="FileFullName"></param>
        /// <param name="Base64String"></param>
        private static void Base64File(string FileFullName, string Base64String)
        {
            String Dir = Path.GetDirectoryName(FileFullName);
            if (Directory.Exists(Dir) == false)
            {
                Directory.CreateDirectory(Dir);
            }

            FileStream fileStream = new FileStream(FileFullName, FileMode.Create);
            byte[] buffer = Convert.FromBase64String(Base64String);
            fileStream.Write(buffer, 0, buffer.Length);
            fileStream.Close();
        }
        #endregion
    }
}
