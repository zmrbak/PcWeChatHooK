using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiSdk
{
    /// <summary>
    /// 微信群中的消息
    /// </summary>
    public class GroupMsgEventArgs : EventArgs
    {
        public string Cmd { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string RoomWxid { get; set; }
        public string RoomName { get; set; }
        public string ToWxid { get; set; }
        public string ToName { get; set; }
        public string Type { get; set; }
        public string FromWxid { get; set; }
        public string FromName { get; set; }
        public string Msg { get; set; }
        public string Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GroupMsgEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class EventSysMsgArgs : EventArgs
    {
        public string Cmd { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string FromWxid { get; set; }
        public string FromName { get; set; }
        public string ToWxid { get; set; }
        public string ToName { get; set; }
        public string Msg { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            EventSysMsgArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class FriendMessageEventArgs : EventArgs
    {
        public string Cmd { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string FromWxid { get; set; }
        public string FromName { get; set; }
        public string ToWxid { get; set; }
        public string ToName { get; set; }
        public string Type { get; set; }
        public string Msg { get; set; }
        public Location MsgLocation { get; set; }
        public string Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }

        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            FriendMessageEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
        public class Location
        {
            public string X { get; set; }
            public string Y { get; set; }
            public string Desc { get; set; }
            public string Name { get; set; }
        }
    }

    public class LoginEventArgs : EventArgs
    {
        public string Cmd { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string Type { get; set; }
        public string Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            LoginEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class EventAcceptAccountsArgs : EventArgs
    {
        public string Cmd { get; set; }
        public string RobotWxid { get; set; }
        public string RobotName { get; set; }
        public string FromWxid { get; set; }
        public string FromName { get; set; }
        public string ToWxid { get; set; }
        public string ToName { get; set; }
        public String Money { get; set; }
        public Msg MsgAccounts { get; set; }

        public class Msg
        {
            public string PaySubtype { get; set; }
            public string IsArrived { get; set; }
            public string RobotPayId { get; set; }
            public string PayId { get; set; }
            public string Money { get; set; }
            public string Remark { get; set; }

            public override string ToString()
            {
                return ToString(Newtonsoft.Json.Formatting.None);
            }
            public string ToString(Newtonsoft.Json.Formatting formatting)
            {
                Msg classData = this;
                return JsonConvert.SerializeObject(classData, formatting);
            }
        }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            EventAcceptAccountsArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class EventUnkownArgs : EventArgs
    {
        public string Cmd { get; set; }
        
        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            EventUnkownArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class EventStartArgs : EventArgs
    {
        public string Cmd { get; set; }
        public string ManagerIP { get; set; }
        public int ManagerPort { get; set; }
        public int WebApiPort { get; set; }
        public string WebToken { get; set; }
        public Boolean DebugOn { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            EventStartArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
    public class EventStopArgs : EventArgs
    {
        public string Cmd { get; set; }
        public string Type { get; set; }
        public DateTime Datetime { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            EventStopArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class ScanCashMoneyEventArgs : EventArgs
    {
        public string From_wxid { get; set; }
        public string To_wxid { get; set; }
        public string Msg { get; set; }
        public string Receive_type { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            ScanCashMoneyEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class FriendRequestEventArgs : EventArgs
    {
        public string From_wxid { get; set; }
        public string To_wxid { get; set; }
        public string Msg { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            FriendRequestEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class CashMoneyEventArgs : EventArgs
    {
        public string From_wxid { get; set; }
        public string To_wxid { get; set; }
        public string Msg { get; set; }
        public string Pay_id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            CashMoneyEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class GroupMemberAddEventArgs : EventArgs
    {
        public string From_wxid { get; set; }
        public string To_wxid { get; set; }
        public string Msg { get; set; }
        public string Name { get; set; }
        public WxMessage WxMsg { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GroupMemberAddEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }

        public class WxMessage
        {
            public string Inviter { get; set; }
            public string Inviter_wxid { get; set; }
            public string Inviter_cid { get; set; }
            public string Guest { get; set; }
            public string Guest_wxid { get; set; }
            public string Guest_cid { get; set; }

            public override string ToString()
            {
                return ToString(Newtonsoft.Json.Formatting.None);
            }
            public string ToString(Newtonsoft.Json.Formatting formatting)
            {
                WxMessage classData = this;
                return JsonConvert.SerializeObject(classData, formatting);
            }
        }
    }

    public class DebugEventArgs : EventArgs
    {
        public string DebugMessage { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            DebugEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class SystemMessageEventArgs : EventArgs
    {
        public string From_wxid { get; set; }
        public string To_wxid { get; set; }
        public string Msg { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            SystemMessageEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }

    public class GroupMessageEventArgs : EventArgs
    {
        public string From_wxid { get; set; }
        public string To_wxid { get; set; }
        public string Msg { get; set; }
        public string Final_from_wxid { get; set; }
        public string Final_from_name { get; set; }
        public string From_name { get; set; }

        public override string ToString()
        {
            return ToString(Newtonsoft.Json.Formatting.None);
        }
        public string ToString(Newtonsoft.Json.Formatting formatting)
        {
            GroupMessageEventArgs classData = this;
            return JsonConvert.SerializeObject(classData, formatting);
        }
    }
}
