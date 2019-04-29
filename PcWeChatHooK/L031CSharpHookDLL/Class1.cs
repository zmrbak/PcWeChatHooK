using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WxWebClient;

namespace L031CSharpHookDLL
{
    [Guid("26F6CA76-FE98-44EE-8982-C739EAEE15F7")]
    public interface ICSharpHookDLClass
    {
        [DispId(1)]
        String QrcodeStringSend(int qrcodeAddress);
    }

    public class CSharpHookDLClass : ICSharpHookDLClass
    {
        Boolean isFinished = false;
        Boolean isSendOK = false;
        public string QrcodeStringSend(int qrcodeAddress)
        {
            isFinished = false;
            isSendOK = false;
            byte[] lpBuffer = new byte[30];
            IntPtr intPtr = IntPtr.Zero;
            Boolean isOk = ReadProcessMemory(Process.GetCurrentProcess().Handle,
                (IntPtr)qrcodeAddress,
                lpBuffer,
                30,
                out intPtr
                );

            if (isOk == false) return "";

            String qrcode = Encoding.UTF8.GetString(lpBuffer);
            qrcode = qrcode.Substring(0, qrcode.IndexOf("\0"));

            HttpClient httpClient = new HttpClient();
            httpClient.MethodInit(Methods.POST);
            httpClient.AddString("二维码字符串：\t" + qrcode);
            httpClient.OnDataReturn += HttpClient_OnDataReturn;
            httpClient.OnException += HttpClient_OnException;
            httpClient.Start();

            while (isFinished == false)
            {
                Thread.Sleep(10);
            }
            if (isSendOK == false)
            {
                return "Failed!";
            }
            else
            {
                return "Successed!";
            }
        }

        private void HttpClient_OnException(Exception ex)
        {
            isFinished = true;
            isSendOK = false;
        }

        private void HttpClient_OnDataReturn(byte[] returnData)
        {
            isFinished = true;
            isSendOK = true;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);
    }

}
