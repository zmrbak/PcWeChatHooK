using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiSdk
{
    public partial class AirCatApi
    {

        #region 事件
        /// <summary>
        /// 未知事件发生，记录到调试信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventUnkown(string json)
        {
            try
            {
                DebugMessage(json);
                //EventUnkownArgs eventUnkownArgs = JsonConvert.DeserializeObject<EventUnkownArgs>(json);
                //EventUnkown?.Invoke(this, eventUnkownArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<EventStartArgs> EventUnkown;
        public event EventHandler<EventStartArgs> OnEventUnkown
        {
            add { EventUnkown += value; }
            remove { EventUnkown -= value; }
        }
        /// <summary>
        /// 插件启用，运行一次这里.
        /// 登录成功，会启用一次这里.
        /// 之后每次插件被手动启用，都会运行一次
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventStart(string json)
        {
            try
            {
                EventStartArgs eventStartArgs = JsonConvert.DeserializeObject<EventStartArgs>(json);
                EventStart?.Invoke(this, eventStartArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<EventStartArgs> EventStart;
        public event EventHandler<EventStartArgs> OnEventStart
        {
            add { EventStart += value; }
            remove { EventStart -= value; }
        }

        /// <summary>
        /// 插件停止时，运行一次这里
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventStop(string json)
        {
            try
            {
                EventStopArgs eventStopArgs = JsonConvert.DeserializeObject<EventStopArgs>(json);
                EventStop?.Invoke(this, eventStopArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<EventStopArgs> EventStop;
        public event EventHandler<EventStopArgs> OnEventStop
        {
            add { EventStop += value; }
            remove { EventStop -= value; }
        }

        /// <summary>
        /// 收到转账事件
        /// 2018.12.13
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventAcceptAccounts(string json)
        {
            try
            {
                EventAcceptAccountsArgs eventAcceptAccountsArgs = JsonConvert.DeserializeObject<EventAcceptAccountsArgs>(json);
                EventAcceptAccounts?.Invoke(this, eventAcceptAccountsArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<EventAcceptAccountsArgs> EventAcceptAccounts;
        public event EventHandler<EventAcceptAccountsArgs> OnEventAcceptAccounts
        {
            add { EventAcceptAccounts += value; }
            remove { EventAcceptAccounts -= value; }
        }

        /// <summary>
        /// 系统消息事件 At 1.1.0
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventSysMsg(string json)
        {
            try
            {
                EventSysMsgArgs eventSysMsgArgs = JsonConvert.DeserializeObject<EventSysMsgArgs>(json);
                EventSysMsg?.Invoke(this, eventSysMsgArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<EventSysMsgArgs> EventSysMsg;
        public event EventHandler<EventSysMsgArgs> OnEventSysMsg
        {
            add { EventSysMsg += value; }
            remove { EventSysMsg -= value; }
        }

        /// <summary>
        /// 面对面收款
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventScanCashMoney(string json)
        {
            try
            {
                ScanCashMoneyEventArgs scanCashMoneyEventArgs = JsonConvert.DeserializeObject<ScanCashMoneyEventArgs>(json);
                EventScanCashMoney?.Invoke(this, scanCashMoneyEventArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<ScanCashMoneyEventArgs> EventScanCashMoney;
        public event EventHandler<ScanCashMoneyEventArgs> OnEventScanCashMoney
        {
            add { EventScanCashMoney += value; }
            remove { EventScanCashMoney -= value; }
        }

        /// <summary>
        /// 微信群消息事件，完成
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventGroupMsg(string json)
        {
            try
            {
                GroupMsgEventArgs groupMsgEventArgs = JsonConvert.DeserializeObject<GroupMsgEventArgs>(json);
                EventGroupMsg?.Invoke(this, groupMsgEventArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<GroupMsgEventArgs> EventGroupMsg;
        public event EventHandler<GroupMsgEventArgs> OnEventGroupMsg
        {
            add { EventGroupMsg += value; }
            remove { EventGroupMsg -= value; }
        }

        /// <summary>
        /// 私聊消息事件，完成
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventFriendsMsg(string json)
        {
            try
            {
                FriendMessageEventArgs friendMessageEventArgs = JsonConvert.DeserializeObject<FriendMessageEventArgs>(json);
                FriendMessageEvent?.Invoke(this, friendMessageEventArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<FriendMessageEventArgs> FriendMessageEvent;
        public event EventHandler<FriendMessageEventArgs> OnFriendMessageEvent
        {
            add { FriendMessageEvent += value; }
            remove { FriendMessageEvent -= value; }
        }

        /// <summary>
        /// 好友请求事件（1.0.8及以上，与低版本会失效但不影响）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventFriendRQ(string json)
        {
            try
            {
                FriendRequestEventArgs friendMessageEventArgs = JsonConvert.DeserializeObject<FriendRequestEventArgs>(json);
                EventFriendRQ?.Invoke(this, friendMessageEventArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<FriendRequestEventArgs> EventFriendRQ;
        public event EventHandler<FriendRequestEventArgs> OnEventFriendRQ
        {
            add { EventFriendRQ += value; }
            remove { EventFriendRQ -= value; }
        }


        /// <summary>
        /// 收到转账事件
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventCashMoney(string json)
        {
            try
            {
                CashMoneyEventArgs cashMoneyEventArgs = JsonConvert.DeserializeObject<CashMoneyEventArgs>(json);
                EventCashMoney?.Invoke(this, cashMoneyEventArgs);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<CashMoneyEventArgs> EventCashMoney;
        public event EventHandler<CashMoneyEventArgs> OnEventCashMoney
        {
            add { EventCashMoney += value; }
            remove { EventCashMoney -= value; }
        }


        /// <summary>
        /// 群成员增加事件
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventAddGroupMember(String json)
        {
            try
            {
                JObject jObject = JObject.Parse(json);

                GroupMemberAddEventArgs messageAddGroupMember = new GroupMemberAddEventArgs
                {
                    //.参数 from_wxid, 文本型, , 来源id，这里是群的id
                    //.参数 to_wxid, 文本型, , 对象id，你收到了群消息，这里是自己
                    //.参数 msg, 文本型, , { "inviter":"听风说雨（发出邀请人昵称）","inviter_wxid":"wxid_gdsdsgdhfd（发出邀请的人wxid）","inviter_cid":"sdfdsf（发出邀请的人标识id）","guest":"Stately（被邀请的人昵称）","guest_wxid":"wxid_gsdsdssd（被邀请的人wxid）","guest_cid":"SGDKHdsgdds（被邀请的人标识id）"}
                    From_wxid = jObject["from_wxid"].ToString(),
                    To_wxid = jObject["to_wxid"].ToString(),
                    Msg = jObject["msg"].ToString()
                };
                EventAddGroupMember?.Invoke(this, messageAddGroupMember);

                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<GroupMemberAddEventArgs> EventAddGroupMember;
        public event EventHandler<GroupMemberAddEventArgs> OnEventAddGroupMember
        {
            add { EventAddGroupMember += value; }
            remove { EventAddGroupMember -= value; }
        }

        /// <summary>
        /// 微信登陆、注销事件
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Boolean Do_EventLogin(String json)
        {
            try
            {
                LoginEventArgs loginEventArgs = JsonConvert.DeserializeObject<LoginEventArgs>(json);
                EventLogin?.Invoke(this, loginEventArgs);

                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }
        private EventHandler<LoginEventArgs> EventLogin;
        public event EventHandler<LoginEventArgs> OnEventLogin
        {
            add { EventLogin += value; }
            remove { EventLogin -= value; }
        }

        #endregion

    }
}
