using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiSdk
{
    public partial class AirCatApi
    {
        #region 变量初始化
        private static int servicePort = 19686;
        private static int airWebApiPort = 19685;
        private static string airWebApiIP = "127.0.0.1";
        private static string airWebApiToken = "HttpWebApi";

        public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;

        /// <summary>
        /// 本管理器提供的Web端口
        /// </summary>
        public int ServicePort { get => servicePort; set => servicePort = value; }

        /// <summary>
        /// WebApiServerWebApi提供的Web端口
        /// </summary>
        public int AirWebApiPort { get => airWebApiPort; set => airWebApiPort = value; }

        /// <summary>
        /// 与WebApiServerWebApi通讯时的Token
        /// </summary>
        public string AirWebApiToken { get => airWebApiToken; set => airWebApiToken = value; }

        /// <summary>
        /// WebApiServerWebApi所在的IP地址
        /// </summary>
        public string AirWebApiIP { get => airWebApiIP; set => airWebApiIP = value; }
        #endregion
    }
}
