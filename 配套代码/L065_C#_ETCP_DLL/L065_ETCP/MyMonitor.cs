using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L065_ETCP
{
    public static class MyMonitor
    {
        public static event EventHandler<MessageArgs> PartEvent;
        public static void InFunction(String message)
        {
            MessageArgs messageArgs = new MessageArgs { TextMessage = message };
            if(PartEvent!=null)
            {
                PartEvent(null, messageArgs);
            }
        }
        public static void MonitorCenter(String s)
        {
            InFunction(s);
        }
    }

    public class MessageArgs:EventArgs
    {
        public String TextMessage { set; get; }
    }
}
