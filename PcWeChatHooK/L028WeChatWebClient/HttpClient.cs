using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WxWebClient
{
    public class HttpClient
    {
        private WebClient webClient;
        private Uri url = null;
        private Methods requestMethod = Methods.GET;
        private string boundary = String.Empty;
        private int paramIndex = 0;
        private List<PostData> dataList = new List<PostData>();
        private List<ParamNameValue> paramList = new List<ParamNameValue>();
        /// <summary>
        /// 目标URL
        /// </summary>
        public Uri Url { get => url; set => url = value; }

        public delegate void WebServerEventHandler(Byte[] returnData);
        public event WebServerEventHandler OnDataReturn;

        public delegate void WebServerExceptionHandler(Exception ex);
        public event WebServerExceptionHandler OnException;

        public HttpClient(Uri uri) : this()
        {
            this.url = uri;
        }

        public HttpClient()
        {

        }

        /// <summary>
        ///  下载完成之后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OnException(e.Error);
                return;
            }

            byte[] serverReturnData = e.Result;
            OnDataReturn(serverReturnData);
        }

        /// <summary>
        /// 上传完数据之后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebClient_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OnException(e.Error);
                return;
            }

            byte[] serverReturnData = e.Result;
            OnDataReturn(serverReturnData);
        }

        /// <summary>
        /// 下载完成之后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    if (e.Error != null)
        //    {
        //        OnException(e.Error);
        //        return;
        //    }

        //    byte[] serverReturnData = Encoding.UTF8.GetBytes(e.Result);
        //    OnDataReturn(serverReturnData);
        //}

        /// <summary>
        /// 提交一个字符串
        /// </summary>
        /// <param name="stringStr"></param>
        public void AddString(String stringStr)
        {
            paramIndex++;
            if (requestMethod == Methods.POST)
            {
                //#################################################
                //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                //Content-Disposition: form-data; name="n_01"

                //aaa
                //#################################################
                string ContentDisposition = $"Content-Disposition: form-data; name=\"\"";
                string Content = stringStr;
                string ContentType = $"Content-Type: text/html";

                PostData postData = new PostData();
                postData.DataType = DataType.Str;
                postData.Boundary = boundary;
                postData.ContentDisposition = ContentDisposition;
                postData.Content = Content;
                postData.ContentType = ContentType;

                dataList.Add(postData);
            }
            else
            {
                paramList.Add(new ParamNameValue(stringStr, ""));
            }
        }

        /// <summary>
        /// 提交一个16进制字符串
        /// </summary>
        /// <param name="hexStr"></param>
        public void AddHex(String hexStr)
        {
            if (requestMethod == Methods.POST)
            {
                //#################################################
                //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                //Content-Disposition: form-data; name="hex_01"

                //aaa
                //#################################################
                string ContentDisposition = $"Content-Disposition: form-data; name=\"\"";
                string Content = hexStr.Replace(" ", "").Replace(" ", "");
                string ContentType = $"Content-Type: application/octet-stream";


                PostData postData = new PostData();
                postData.DataType = DataType.Hex;
                postData.Boundary = boundary;
                postData.ContentDisposition = ContentDisposition;
                postData.Content = Content;
                postData.ContentType = ContentType;

                dataList.Add(postData);
            }
        }

        /// <summary>
        /// 添加一个form数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParam(String key, String value)
        {
            if (requestMethod == Methods.POST)
            {
                //#################################################
                //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                //Content-Disposition: form-data; name="fname"

                //aaa
                //#################################################
                string ContentDisposition = $"Content-Disposition: form-data; name=\"{key}\"";
                string Content = value;

                PostData postData = new PostData();
                postData.DataType = DataType.Param;
                postData.Boundary = boundary;
                postData.ContentDisposition = ContentDisposition;
                postData.Content = Content;

                dataList.Add(postData);
            }
            else
            {
                paramList.Add(new ParamNameValue(key, value));
            }
        }

        public void AddFile(String fileName)
        {
            if (requestMethod == Methods.POST)
            {
                //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                //Content-Disposition: form-data; name="lfile"; filename="test.html"
                //Content-Type: text/html

                //<form action="http://127.0.0.1:8421/" method="POST" enctype="multipart/form-data">
                //  <p>First name: <input type="text" name="fname" value="aaa" /></p>
                //  <p>Last name: <input type="text" name="lname" value="bbb" /></p>
                //  <p>Last name: <input type="file" name="lfile" /></p>
                //  <input type="submit" value="Submit" />
                //</form>

                if (fileName == "") return;

                if(File.Exists(fileName)==false)
                {
                    throw new Exception("文件未找到");
                }
                string ContentDisposition = $"Content-Disposition: form-data; name=\"f_{paramIndex.ToString("d4")}\"; filename=\"{ Path.GetFileName(fileName)}\"";
                string ContentType = $"Content-Type: application/octet-stream";

                PostData postData = new PostData();
                postData.DataType = DataType.File;
                postData.Boundary = boundary;
                postData.ContentDisposition = ContentDisposition;
                postData.ContentType = ContentType;
                postData.ContentBytes = File.ReadAllBytes(fileName);

                dataList.Add(postData);
            }
        }

        /// <summary>
        /// 把文件转换成Bytes，添加
        /// </summary>
        /// <param name="fileBytes"></param>
        public void AddFile(Byte[] fileBytes)
        {
            if (requestMethod == Methods.POST)
            {
                //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                //Content-Disposition: form-data; name="lfile"; filename="test.html"
                //Content-Type: text/html

                //<form action="http://127.0.0.1:8421/" method="POST" enctype="multipart/form-data">
                //  <p>First name: <input type="text" name="fname" value="aaa" /></p>
                //  <p>Last name: <input type="text" name="lname" value="bbb" /></p>
                //  <p>Last name: <input type="file" name="lfile" /></p>
                //  <input type="submit" value="Submit" />
                //</form>
                string ContentDisposition = $"Content-Disposition: form-data; name=\"f_{paramIndex.ToString("d4")}\"; filename=\"f_{paramIndex.ToString("d4")}\"";
                string ContentType = $"Content-Type: application/octet-stream";

                PostData postData = new PostData();
                postData.DataType = DataType.File;
                postData.Boundary = boundary;
                postData.ContentDisposition = ContentDisposition;
                postData.ContentType = ContentType;
                postData.ContentBytes = fileBytes;

                dataList.Add(postData);
            }
        }



        /// <summary>
        /// 开始向服务器发送数据
        /// </summary>
        public void Start()
        {
            webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadDataCompleted += WebClient_UploadDataCompleted;
            webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;

            if (requestMethod == Methods.POST)
            {
                byte[] bytes = MergeContent();
                webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
                webClient.UploadDataAsync(url, bytes);
            }
            else
            {
                String queryString = url.AbsoluteUri;
                String query = MergeQuery();
                if (query != "")
                {
                    queryString += "?" + query;
                }
                webClient.DownloadDataAsync(new Uri(queryString));
            }
        }

        /// <summary>
        /// 初始化POST/GET
        /// </summary>
        /// <param name="methods"></param>
        public void MethodInit(Methods methods)
        {
            requestMethod = methods;

            if (requestMethod == Methods.POST)
            {
                dataList = new List<PostData>();
                boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            }
            else
            {
                paramList = new List<ParamNameValue>();
            }
        }

        private String MergeQuery()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (ParamNameValue item in paramList)
            {
                stringBuilder.Append(HttpUtility.UrlEncode(item.ParamName, Encoding.GetEncoding("utf-8")) + "=" + HttpUtility.UrlEncode(item.ParamValue, Encoding.GetEncoding("utf-8")) + "&");
            }
            String temp = stringBuilder.ToString(); ;
            if (temp.Length <= 0) return "";

            return temp.Substring(0, temp.Length - 1);
        }

        /// <summary>
        /// 合并请求数据
        /// </summary>
        /// <returns></returns>
        private byte[] MergeContent()
        {
            List<byte[]> byteList = new List<byte[]>();

            foreach (PostData postData in dataList)
            {
                byteList.Add(postData.ToBytes());
            }

            string endBoundary = boundary + "--\r\n";
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes(endBoundary);
            byteList.Add(endBoundaryBytes);

            int length = 0;
            foreach (Byte[] item in byteList)
            {
                length += item.Length;
            }

            byte[] tempBytes = new byte[length];
            int index = 0;
            foreach (Byte[] item in byteList)
            {
                index += item.Length;
                Array.Copy(item, 0, tempBytes, index - item.Length, item.Length);
            }

            String aa = Encoding.UTF8.GetString(tempBytes);

            return tempBytes;
        }
    }

    public class PostData
    {
        //------WebKitFormBoundarypOpVzy4TGV9qmhuu
        //Content-Disposition: form-data; name="fname"

        //aaa
        //------WebKitFormBoundarypOpVzy4TGV9qmhuu
        //Content-Disposition: form-data; name="lname"

        //bbb
        //------WebKitFormBoundarypOpVzy4TGV9qmhuu
        //Content-Disposition: form-data; name="lfile"; filename="test.html"
        //Content-Type: text/html

        //<form action="http://127.0.0.1:8421/" method="POST" enctype="multipart/form-data">
        //  <p>First name: <input type = "text" name="fname" value="aaa" /></p>
        //  <p>Last name: <input type = "text" name="lname" value="bbb" /></p>
        //  <p>Last name: <input type = "file" name="lfile" /></p>
        //  <input type = "submit" value="Submit" />
        //</form>
        //------WebKitFormBoundarypOpVzy4TGV9qmhuu--

        string boundary = "";
        string contentDisposition = "";
        string contentType = "";
        string content = "";
        byte[] contentBytes;
        DataType dataType = DataType.Param;


        public string Boundary { get => boundary; set => boundary = value; }
        public string ContentDisposition { get => contentDisposition; set => contentDisposition = value; }
        public string ContentType { get => contentType; set => contentType = value; }
        public string Content { get => content; set => content = value; }
        public DataType DataType { get => dataType; set => dataType = value; }
        public byte[] ContentBytes { get => contentBytes; set => contentBytes = value; }

        public byte[] ToBytes()
        {
            String bb = "";
            byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary);
            byte[] contentDispositionBytes = Encoding.UTF8.GetBytes(contentDisposition);
            byte[] contentTypeBytes = Encoding.UTF8.GetBytes(contentType);
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);

            switch (dataType)
            {
                case DataType.Param:
                case DataType.Str:
                    {
                        //######################################################
                        //------WebKitFormBoundarypOpVzy4TGV9qmhuu\r\n
                        //Content-Disposition: form-data; name="fname"\r\n
                        //\r\n
                        //aaa\r\n
                        //######################################################

                        int length = boundaryBytes.Length + contentDispositionBytes.Length + contentTypeBytes.Length + contentBytes.Length;
                        //回车，换行，一共10个
                        length += 10;
                        byte[] Bytes = new byte[length];

                        //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                        int index = boundaryBytes.Length;
                        Array.Copy(boundaryBytes, 0, Bytes, index - boundaryBytes.Length, boundaryBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //Content-Disposition: form-data; name="fname"
                        index += contentDispositionBytes.Length;
                        Array.Copy(contentDispositionBytes, 0, Bytes, index - contentDispositionBytes.Length, contentDispositionBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //Content-Type: text/html
                        index += contentTypeBytes.Length;
                        Array.Copy(contentTypeBytes, 0, Bytes, index - contentTypeBytes.Length, contentTypeBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //aaa
                        index += contentBytes.Length;
                        Array.Copy(contentBytes, 0, Bytes, index - contentBytes.Length, contentBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        bb = Encoding.UTF8.GetString(Bytes);
                        return Bytes;
                    }
                case DataType.Hex:
                    {
                        //######################################################
                        //------WebKitFormBoundarypOpVzy4TGV9qmhuu\r\n
                        //Content-Disposition: form-data; name="fname"\r\n
                        //\r\n
                        //aaa\r\n
                        //######################################################

                        //把16进制字符串转换成byte数组
                        //多了一位
                        if (content.Length % 2 == 1)
                        {
                            contentBytes = new byte[(content.Length + 1) / 2];

                            byte myByte = 0;
                            if (byte.TryParse(content.Substring(content.Length - 1, 1), System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out myByte) == true)
                            {
                                contentBytes[(content.Length + 1) / 2 - 1] = myByte;
                            }
                        }
                        else
                        {
                            contentBytes = new byte[content.Length / 2];
                        }

                        for (int i = 0; i < content.Length / 2; i++)
                        {
                            byte myByte = 0;
                            if (byte.TryParse(content.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out myByte) == true)
                            {
                                contentBytes[i] = myByte;
                            }
                        }

                        int length = boundaryBytes.Length + contentDispositionBytes.Length + contentTypeBytes.Length + contentBytes.Length;
                        //回车，换行，一共10个
                        length += 10;
                        byte[] Bytes = new byte[length];

                        //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                        int index = boundaryBytes.Length;
                        Array.Copy(boundaryBytes, 0, Bytes, index - boundaryBytes.Length, boundaryBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //Content-Disposition: form-data; name="fname"
                        index += contentDispositionBytes.Length;
                        Array.Copy(contentDispositionBytes, 0, Bytes, index - contentDispositionBytes.Length, contentDispositionBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //Content-Type: text/html
                        index += contentTypeBytes.Length;
                        Array.Copy(contentTypeBytes, 0, Bytes, index - contentTypeBytes.Length, contentTypeBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //aaa
                        index += contentBytes.Length;
                        Array.Copy(contentBytes, 0, Bytes, index - contentBytes.Length, contentBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //bb = Encoding.UTF8.GetString(Bytes);
                        return Bytes;
                    }
                case DataType.File:
                    {
                        //######################################################
                        //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                        //Content-Disposition: form-data; name="lfile"; filename="test.html"
                        //Content-Type: text/html

                        //<form action="http://127.0.0.1:8421/" method="POST" enctype="multipart/form-data">
                        //  <p>First name: <input type="text" name="fname" value="aaa" /></p>
                        //  <p>Last name: <input type="text" name="lname" value="bbb" /></p>
                        //  <p>Last name: <input type="file" name="lfile" /></p>
                        //  <input type="submit" value="Submit" />
                        //</form>
                        //######################################################

                        int length = boundaryBytes.Length + contentDispositionBytes.Length + contentTypeBytes.Length + this.contentBytes.Length;
                        //回车，换行，一共10个（多了一行文字，多了\r\n）
                        length += 10;
                        byte[] Bytes = new byte[length];

                        //------WebKitFormBoundarypOpVzy4TGV9qmhuu
                        int index = boundaryBytes.Length;
                        Array.Copy(boundaryBytes, 0, Bytes, index - boundaryBytes.Length, boundaryBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //Content-Disposition: form-data; name="fname"
                        index += contentDispositionBytes.Length;
                        Array.Copy(contentDispositionBytes, 0, Bytes, index - contentDispositionBytes.Length, contentDispositionBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //Content-Type: text/html
                        index += contentTypeBytes.Length;
                        Array.Copy(contentTypeBytes, 0, Bytes, index - contentTypeBytes.Length, contentTypeBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        //内容
                        index += this.contentBytes.Length;
                        Array.Copy(this.contentBytes, 0, Bytes, index - this.contentBytes.Length, this.contentBytes.Length);
                        Bytes[index] = (Byte)('\r');
                        index += 1;
                        Bytes[index] = (Byte)('\n');
                        index += 1;

                        bb = Encoding.UTF8.GetString(Bytes);
                        return Bytes;
                    }
                default:
                    return new byte[0];
            }
        }
    }

    public enum DataType
    {
        Param = 1,
        File = 2,
        Str = 3,
        Hex = 4
    }

    public enum Methods
    {
        POST = 0,
        GET = 1
    }

    //public class ParamNameValue
    //{
    //    private string paramName;
    //    private string paramValue;

    //    public ParamNameValue(string item, string value)
    //    {
    //        this.paramName = item;
    //        this.paramValue = value;
    //    }

    //    public string ParamName { get => paramName; set => paramName = value; }
    //    public string ParamValue { get => paramValue; set => this.paramValue = value; }
    //}
}
