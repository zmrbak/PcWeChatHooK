using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebApiSdk
{
    public partial class AirCatApi
    {
        /// <summary>
        /// 启动Web监听
        /// </summary>
        /// <returns></returns>
        public Boolean ServiceStart()
        {
            DebugMessage("启动Web监听......");
            DebugMessage("本地管理器的通讯端口：\t" + servicePort);
            DebugMessage("WebApiServerWebApi通讯密钥：\t" + airWebApiToken);
            DebugMessage("WebApiServerWebApi通讯地址：\t" + airWebApiIP);
            DebugMessage("WebApiServerWebApi通讯端口：\t" + airWebApiPort);

            HttpListener httpListener = new HttpListener();
            try
            {
                if (httpListener.IsListening)
                {
                    httpListener.Stop();
                }

                httpListener.Prefixes.Clear();
                httpListener.Prefixes.Add("http://+:" + servicePort + "/");
                httpListener.Start();
                httpListener.BeginGetContext(new AsyncCallback(OnGetContext), httpListener);

                //通知WebApi
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 检索传入的请求
        /// </summary>
        /// <param name="iAsyncResult"></param>
        private void OnGetContext(IAsyncResult iAsyncResult)
        {
            try
            {
                HttpListener httpListener = iAsyncResult.AsyncState as HttpListener;

                //接收到的请求context（一个环境封装体）
                if (httpListener.IsListening == false) return;
                HttpListenerContext context = httpListener.EndGetContext(iAsyncResult);

                //开始 第二次 异步接收request请求
                httpListener.BeginGetContext(new AsyncCallback(OnGetContext), httpListener);

                //接收的request数据
                HttpListenerRequest request = context.Request;

                //用来向客户端发送回复
                HttpListenerResponse response = context.Response;

                //开始处理请求（代码运行流程进入web网站程序）
                HandleRequest(request, response);
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);
            }
        }

        /// <summary>
        /// 开始处理请求
        /// </summary>
        /// <param name="reqeust"></param>
        /// <param name="response"></param>
        private void HandleRequest(HttpListenerRequest reqeust, HttpListenerResponse response)
        {
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            String outputString = "";
            JObject returnJson = new JObject();

            if (reqeust.HttpMethod == "POST")
            {
                if (reqeust.QueryString["token"] == airWebApiToken)
                {
                    Stream postData = reqeust.InputStream;
                    StreamReader sr = new StreamReader(postData);
                    string requestString = sr.ReadToEnd();
                    sr.Close();

                    if (requestString.Trim() != "")
                    {
                        DebugMessage("收到POST:" + requestString);
                        try
                        {
                            JObject jtest = JObject.Parse(requestString);
                            outputString = MessageAnalysis(requestString);
                            DebugMessage("返回POST:" + outputString);
                            //发送回复
                            using (Stream output = response.OutputStream)
                            {
                                byte[] buffer = Encoding.UTF8.GetBytes(outputString);
                                output.Write(buffer, 0, buffer.Length);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            DebugMessage(ex.Message);
                            DebugMessage(ex.StackTrace);

                            DebugMessage("收到POST:" + reqeust.RawUrl + "\t" + requestString);
                            returnJson.Add("Result", false);
                            returnJson.Add("Msg", "输入信息有误");
                        }
                    }
                    else
                    {
                        DebugMessage("收到POST:" + reqeust.RawUrl);
                        returnJson.Add("Result", false);
                        returnJson.Add("Msg", "缺少输入信息");

                    }
                }
                else
                {
                    DebugMessage("收到POST:" + reqeust.RawUrl);
                    returnJson.Add("Result", false);
                    returnJson.Add("Msg", "Token认证失败");
                }
            }
            else
            {
                DebugMessage("收到GET:" + reqeust.RawUrl);
                returnJson.Add("Cmd", "ServerCheck");
                returnJson.Add("Side", "Client");
                returnJson.Add("Result", true);
                returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            }

            using (Stream output = response.OutputStream)  //发送回复
            {
                outputString = returnJson.ToString(Newtonsoft.Json.Formatting.None, null);
                DebugMessage("返回POST:" + outputString);
                byte[] buffer = Encoding.UTF8.GetBytes(outputString);
                output.Write(buffer, 0, buffer.Length);
                return;
            }
        }

        /// <summary>
        /// 解析从AirApi发送过来的数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private String MessageAnalysis(String json)
        {
            JObject returnJson = new JObject();
            try
            {
                JObject jObject = JObject.Parse(json);
                if (jObject.Property("Cmd") != null)
                {
                    String cmd = jObject["Cmd"].ToString().ToLower();
                    switch (cmd)
                    {
                        case "getserverstatus":
                            //完成
                            returnJson = new JObject();
                            returnJson.Add("Cmd", jObject["Cmd"].ToString());
                            returnJson.Add("Result", true);
                            returnJson.Add("Side", "Client");
                            returnJson.Add("Msg", "WebApi客户端/管理端");
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventacceptaccounts":
                            //2018.12.13
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventAcceptAccounts(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventaddgroupmember":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventAddGroupMember(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventfriendrq":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventFriendRQ(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventfriendsmsg":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventFriendsMsg(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventgroupmsg":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventGroupMsg(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventcashmoney":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventCashMoney(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventlogin":
                            //2018.12.13
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventLogin(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventscancashmoney":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventScanCashMoney(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventstart":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventStart(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventstop":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventStop(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case "eventsysmsg":
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventSysMsg(json));
                            returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        default:
                            returnJson = new JObject();
                            returnJson.Add("Result", Do_EventUnkown(json));
                            returnJson.Add("Msg", "收到WebApiServer发送来的未知事件");
                            break;
                    }
                }
                else
                {
                    returnJson = new JObject();
                    returnJson.Add("Result", false);
                    returnJson.Add("Msg", "缺少参数");
                }
            }
            catch (Exception ex)
            {
                DebugMessage(ex.Message);
                DebugMessage(ex.StackTrace);

                returnJson = new JObject();
                returnJson.Add("Result", false);
                returnJson.Add("Datetime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            }
            return returnJson.ToString(Newtonsoft.Json.Formatting.None, null);
        }
    }
}

