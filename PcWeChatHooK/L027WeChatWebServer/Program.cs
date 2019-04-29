using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L027WeChatWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer httpServer = new HttpServer();
            httpServer.OnDataReceived += HttpServer_OnDataReceived;

            if (httpServer.Start())
            {
                Console.WriteLine("Web Server Started!");
            }

            Console.ReadLine();
        }

        private static void HttpServer_OnDataReceived(System.Net.HttpListenerRequest reqeust, System.Net.HttpListenerResponse response)
        {
            if (reqeust.HttpMethod == "POST")
            {
                Console.WriteLine("POST");
                Stream stream = reqeust.InputStream;
                BinaryReader binaryReader = new BinaryReader(stream);

                byte[] data = new byte[reqeust.ContentLength64];
                binaryReader.Read(data, 0, (int)reqeust.ContentLength64);

                String dataString = Encoding.UTF8.GetString(data);
                Console.WriteLine(dataString);


            }
            else
            {
                Console.WriteLine("NOT POST");

            }


            string responseString = "<HTML><BODY> Main:" + DateTime.Now + "</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }
    }
}
