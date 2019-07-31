using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace L027WeChatWebServer
{
    public class HttpServer
    {
        public delegate void DataReceivedEventHandler(HttpListenerRequest reqeust, HttpListenerResponse response);
        public event DataReceivedEventHandler OnDataReceived;

        int port = 8421;
        public int Port { get => port; set => port = value; }

        public Boolean Start()
        {
            try
            {
                if (!HttpListener.IsSupported)
                {
                    Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return false;
                }
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://+:"+ port + "/");
                listener.Start();

                listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                return false;
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
            OnDataReceived(context.Request, context.Response);
        }
    }
}
