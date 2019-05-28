using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiSdk
{
   
    public class GetServerStatusClassData
    {
        //{"Cmd":"CheckServer","Result":true,"Datetime":"2018-12-11 16:58:02.082"}
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string Side { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }


        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetServerStatusClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }


    public class GetRobotListClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public IList<RobotsClassData> Robots { get; set; }
        public DateTime Datetime { get; set; }

        public class RobotsClassData
        {
            public Boolean RobotStatus { get; set; }
            public String RobotWxid { get; set; }
            public String RobotWxName { get; set; }
            public String RobotHeadImg { get; set; }
            public String RobotHandle { get; set; }
            public DateTime LoginTime { get; set; }

            public override string ToString()
            {
                return ToString(Newtonsoft.Json.Formatting.None);
            }

            public string ToString(Newtonsoft.Json.Formatting formatting)
            {
                RobotsClassData classData = this;
                return JsonConvert.SerializeObject(classData, formatting);
            }
        }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetRobotListClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }


    public class GetRoomListClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public int AllCount { get; set; }
        public IList<SimpleRoomClassData> Rooms { get; set; }
        public int Start { get; set; }
        public int Next { get; set; }
        public int Status { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetRoomListClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SimpleRoomClassData
    {
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public string RoomImg { get; set; }
        public string MemberCount { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SimpleRoomClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    /// <summary>
    /// 微信用户数据
    /// </summary>
    public class UserClassData
    {
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public string WxImg { get; set; }
        public string WxNote { get; set; }
        public string WxStatus { get; set; }
        public string RobotWxid { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            UserClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class GetRoomInfoClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public IList<WxRoom> Members { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public string Datetime { get; set; }

        public class WxRoom
        {
            public string Wxid { get; set; }
            public string WxName { get; set; }
            public string WxImg { get; set; }
            public string WxNote { get; set; }
            public string WxStatus { get; set; }
            public string RotobWxid { get; set; }

            public override string ToString()
            {
                return ToString(Newtonsoft.Json.Formatting.None);
            }

            public string ToString(Newtonsoft.Json.Formatting formatting)
            {
                WxRoom classData = this;
                return JsonConvert.SerializeObject(classData, formatting);
            }
        }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetRoomInfoClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class GetContactListClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public int AllCount { get; set; }
        public IList<UserClassData> Members { get; set; }
        public int Start { get; set; }
        public int Next { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetContactListClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class GetContractNameClassData
    {
        //{"Cmd":"GetContactList","Result":true,"AllCount":27,"Start":1,"Member":[{"Wxid":"fmessage","User":"","Name":"朋友推荐消息","HeadImgUrl":"","Type":1,"Note":"","V1":""}]}
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetContractNameClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class GetContractWxidsClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string WxName { get; set; }
        public IList<WxidClassData> Wxids { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public class WxidClassData
        {
            public string Wxid { get; set; }

            public override string ToString()
            {
                return ToString(Newtonsoft.Json.Formatting.None);
            }

            public string ToString(Newtonsoft.Json.Formatting formatting)
            {
                WxidClassData classData = this;
                return JsonConvert.SerializeObject(classData, formatting);
            }
        }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetContractWxidsClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    

    public class GetRoomNameClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetRoomNameClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }


    public class GetRoomWxidsClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public IList<RoomWxidClassData> RoomsWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetRoomWxidsClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }

        public class RoomWxidClassData
        {
            public string RoomWxid { get; set; }
            public override string ToString()
            {
                return ToString(Newtonsoft.Json.Formatting.None);
            }

            public string ToString(Newtonsoft.Json.Formatting formatting)
            {
                RoomWxidClassData classData = this;
                return JsonConvert.SerializeObject(classData, formatting);
            }
        }
    }

    public class SendRoomClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }        
    }

    public class SendRoomTextAtClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public string AtWxid { get; set; }
        public string AtWxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomTextAtClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendRoomRenameClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomRenameClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendRoomLinkClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomLinkClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendRoomKickUserClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomKickUserClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendRoomImageClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomImageClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendRoomFileClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomFileClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendRoomEmojiClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomEmojiClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendRoom163MusicClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoom163MusicClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    public class SendRoomVideoClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendRoomVideoClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendContractVideoClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendContractVideoClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendContractTextClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendContractTextClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendContractLinkClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendContractLinkClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendContractImageClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendContractImageClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendContractFileClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendContractFileClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SendContractClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendContractClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }



    public class GetUserListClassData
    {
        public string Cmd { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public Boolean Result { get; set; }
        public int AllCount { get; set; }
        public int Start { get; set; }
        public int Next { get; set; }
        public IList<UserClassData> Members { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetUserListClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }

    }
    
    public class SendLogsClassData
    {
        //{"Cmd":"LogOnAir","Result":true,"Status":0,"Datetime":"2018-12-11 20:53:51.064"}
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SendLogsClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class ModifyRoomNameClassData
    {
        //{"Cmd":"LogOnAir","Result":true,"Status":0,"Datetime":"2018-12-11 20:53:51.064"}
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public int Status { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            ModifyRoomNameClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class AgreeFriendClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            AgreeFriendClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class ConfirmPayClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            ConfirmPayClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class AddUserToContractClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            AddUserToContractClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    public class SumbitOfficialAccountClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SumbitOfficialAccountClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    public class AddContractToRoomClassData
    {
        //{"Cmd":"LogOnAir","Result":true,"Status":0,"Datetime":"2018-12-11 20:53:51.064"}
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            AddContractToRoomClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    public class InviteContractToRoomClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }


        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            InviteContractToRoomClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    public class CreateRoomClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            CreateRoomClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class QuitFromRoomClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }


        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            QuitFromRoomClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SetRoomAnnouncementData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SetRoomAnnouncementData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    public class GetContactInfoData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Wxid { get; set; }
        public string WxName { get; set; }
        public string WxImg { get; set; }
        public string WxNote { get; set; }
        public string WxStatus { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }


        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetContactInfoData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }


    
    public class KickRoomUserClassData
    {
        //{"Cmd":"LogOnAir","Result":true,"Status":0,"Datetime":"2018-12-11 20:53:51.064"}
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public int Status { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            KickRoomUserClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class GetUserNameClassData
    {
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public String RobotWxid { get; set; }
        public string RobotName { get; set; }
        public String Wxid { get; set; }
        public string WxName { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetUserNameClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class GetUserWxidClassData
    {
        //{"Cmd":"GetUserWxid","Result":true,"Member":[{"Wxid":"wxid_jopq6kn5msw822"}],"Datetime":"2018-12-11T22:02:14.178"}
        public string Cmd { get; set; }
        public Boolean Result { get; set; }
        public string UserName { get; set; }
        public IList<WxID> Member { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GetUserWxidClassData classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }

        public class WxID
        {
            public string Wxid { get; set; }

            public override string ToString()
            {
                return ToString(Newtonsoft.Json.Formatting.None);
            }

            public string ToString(Newtonsoft.Json.Formatting formatting)
            {
                WxID classData = this;
                return JsonConvert.SerializeObject(classData, formatting);
            }
        }
    }
    /// <summary>
    /// 微信群简单信息
    /// </summary>
    public class WxRoomSimpleData
    {
        //.成员 Wxid, 文本型, , , 收发消息用的用户标识，类似于，酷Q里的群号
        //.成员 Name, 文本型, , , 群名字
        //.成员 Member, UserData, , "1", 群的成员，是一个数组
        String wxid;
        String name;
        int membercount;

        public string Wxid { get => wxid; set => wxid = value; }
        public string Name { get => name; set => name = value; }
        public int Membercount { get => membercount; set => membercount = value; }
    }

    /// <summary>
    /// 微信群详细信息
    /// </summary>
    public class WxGetRoomInfoData
    {
        String wxid;
        String name;
        List<UserClassData> members = new List<UserClassData>();

        public int MemberCount
        {
            get
            {
                return members.Count;
            }
        }
        public string Name { get => name; set => name = value; }
        public string Wxid { get => wxid; set => wxid = value; }
        public List<UserClassData> Members { get => members; set => members = value; }

        public void Add(UserClassData wxUserData)
        {
            members.Add(wxUserData);
        }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            JObject jObject = new JObject();
            jObject.Add("Wxid", wxid);
            jObject.Add("Name", name);
            jObject.Add("Members", JArray.Parse(JsonConvert.SerializeObject(members)));
            return JsonConvert.SerializeObject(jObject, formatting);
        }
    }

    public class WxUserListsData
    {
        List<UserClassData> wxUserDatas = new List<UserClassData>();

        public List<UserClassData> WxUserDatas { get => wxUserDatas; set => wxUserDatas = value; }

        public void Add(UserClassData wxUser)
        {
            wxUserDatas.Add(wxUser);
        }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            return JsonConvert.SerializeObject(wxUserDatas, formatting);
        }
    }

    
}
