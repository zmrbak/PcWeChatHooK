using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WxWebClient
{
    public class HttpServer
    {
        public delegate void PostDataReceivedEventHandler(List<ChunkData> chunkDataList, HttpListenerResponse response);
        public event PostDataReceivedEventHandler OnPostDataReceived;

        public delegate void GetDataReceivedEventHandler(List<ParamNameValue> paramList, HttpListenerResponse response);
        public event GetDataReceivedEventHandler OnGetDataReceived;


        public Boolean Start()
        {
            return Start(8421);
        }

        /// <summary>
        /// 启动Web监听
        /// </summary>
        /// <returns></returns>
        public Boolean Start(int port)
        {
            HttpListener httpListener = new HttpListener();
            try
            {
                if (httpListener.IsListening) httpListener.Stop();

                httpListener.Prefixes.Clear();
                httpListener.Prefixes.Add("http://+:" + port + "/");
                httpListener.Start();
                httpListener.BeginGetContext(new AsyncCallback(OnGetContext), httpListener);

                return true;
            }
            catch (Exception ex)
            {
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
                //OnDataReceived(request, response);

                DataParas(request, response);
            }
            catch (Exception ex)
            {
                //Logs.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void DataParas(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod == "POST")
            {
                //上传文件：包含文件名
                if (request.ContentType.Length > 20 && string.Compare(request.ContentType.Substring(0, 20), "multipart/form-data;", true) == 0)
                {
                    if (!request.HasEntityBody)
                    {
                        return;
                    }

                    List<ChunkData> chunkDataList;

                    //解析ContentType
                    string[] HttpListenerPostValue = request.ContentType.Split(';').Skip(1).ToArray();
                    string boundary = string.Join(";", HttpListenerPostValue).Replace("boundary=", "").Trim();

                    //分割字符串
                    byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary);
                    byte[] boundaryEndBytes = Encoding.UTF8.GetBytes(boundary + "--\r\n");

                    //提取POST来的数据
                    Stream stream = request.InputStream;
                    BinaryReader binaryReader = new BinaryReader(stream);
                    byte[] currentChunk = new byte[request.ContentLength64];
                    binaryReader.Read(currentChunk, 0, (int)(request.ContentLength64));
                    stream.Close();

                    List<int> boundaryList = new List<int>();
                    List<byte[]> chunkBytesList = new List<byte[]>();
                    chunkDataList = new List<ChunkData>();

                    //从数组中读取一行，遇到\r\n为结束

                    //找出所有的分割字符串
                    for (int i = 0; i < currentChunk.Length - 1; i++)
                    {
                        if (currentChunk[i] == boundaryBytes[1])
                        {
                            Boolean find = true;
                            for (int j = 1; j < boundaryBytes.Length; j++)
                            {
                                if (currentChunk[i + j] != boundaryBytes[j])
                                {
                                    find = false;
                                    break;
                                }
                            }
                            if (find == true)
                            {
                                boundaryList.Add(i);
                                i = i + boundaryBytes.Length;
                            }
                        }
                    }

                    //分解成每一个块
                    int lastIndex = 0;
                    foreach (int item in boundaryList)
                    {
                        if (item == 0) continue;
                        //去掉---------------------------8d6b5a2914da90f\r\n
                        byte[] chunkBytes = new byte[item - lastIndex - boundaryBytes.Length - 2];
                        Array.ConstrainedCopy(currentChunk, lastIndex + boundaryBytes.Length + 2, chunkBytes, 0, chunkBytes.Length);
                        chunkBytesList.Add(chunkBytes);
                        lastIndex = item;
                    }

                    //解析每一个块
                    //chunkList
                    foreach (Byte[] dataBytes in chunkBytesList)
                    {
                        //找出每个\r\n
                        List<int> lineList = new List<int>();
                        for (int i = 0; i < dataBytes.Length - 1; i++)
                        {
                            if (dataBytes[i] == '\r' && dataBytes[i + 1] == '\n')
                            {
                                lineList.Add(i);
                            }
                        }

                        //找出每一行
                        List<byte[]> lineDataList = new List<byte[]>();
                        lastIndex = 0;
                        foreach (int item in lineList)
                        {
                            byte[] lineData = new byte[item - lastIndex];
                            Array.ConstrainedCopy(dataBytes, lastIndex, lineData, 0, lineData.Length);
                            lineDataList.Add(lineData);
                            //末尾有\r\n
                            lastIndex = item + 2;
                        }

                        //解析数据
                        //至少应该大于等于3行，否则无效
                        if (lineDataList.Count < 3) continue;

                        ChunkData chunkData = new ChunkData();
                        //###########################################################
                        //Content-Disposition: form-data; name="n_0001"\r\n
                        //\r\n
                        //字符串1\r\n
                        //###########################################################

                        //第一行,转换成字符串
                        String line0 = Encoding.UTF8.GetString(lineDataList[0]);

                        //如果是 Content-Disposition: form-data; name="f_0002"; filename="test.html"
                        //是一个POST的文件
                        int fileNameIndex = line0.IndexOf("filename=\"");
                        if (fileNameIndex >= 0)
                        {
                            chunkData.DataType = DataTypeEnum.File;
                            String fileName = line0.Substring(fileNameIndex).Substring("filename=\"".Length);
                            chunkData.FileName = fileName.Substring(0, fileName.Length - 1);

                            //第二行
                            //Content-Type: application/octet-stream
                            String line1 = Encoding.UTF8.GetString(lineDataList[1]);
                            chunkData.ContentType = line1.Substring("Content-Type:".Length + 2).Trim();

                            //忽略第三行

                            //从第四行开始剩下的全部数据，为File内容
                            //计算数据长度
                            int dataLength = 0;
                            for (int i = 0; i < lineDataList.Count; i++)
                            {
                                if (i < 3) continue;
                                dataLength += lineDataList[i].Length;
                            }
                            //填充数据
                            byte[] fileHexData = new byte[dataLength];
                            lastIndex = 0;
                            for (int i = 0; i < lineDataList.Count; i++)
                            {
                                if (i < 3) continue;
                                Array.ConstrainedCopy(lineDataList[i], 0, fileHexData, lastIndex, lineDataList[i].Length);
                                lastIndex += lineDataList[i].Length;
                            }
                            String test1 = Encoding.UTF8.GetString(fileHexData);
                            chunkData.HexData = fileHexData;
                        }
                        else
                        {
                            //Param型
                            //Content-Disposition: form-data; name="n_0001"
                            //
                            int paramIndex = line0.IndexOf("name=\"\"");
                            if (paramIndex >= 0)
                            {
                                //参数，无名称
                                //第二行
                                //Content-Type: application/octet-stream

                                //判断第2行
                                String line1 = Encoding.UTF8.GetString(lineDataList[1]);
                                String contentType = line1.Substring("Content-Type:".Length + 1).Trim();
                                chunkData.ContentType = contentType;
                                if (contentType == "application/octet-stream")
                                {
                                    chunkData.DataType = DataTypeEnum.Hex;
                                }
                                else
                                {
                                    chunkData.DataType = DataTypeEnum.Str;
                                }

                                //忽略第三行

                                //从第四行开始剩下的全部数据，为File内容
                                //计算数据长度
                                int dataLength = 0;
                                for (int i = 0; i < lineDataList.Count; i++)
                                {
                                    if (i < 3) continue;
                                    dataLength += lineDataList[i].Length;
                                }

                                //填充数据
                                byte[] fileHexData = new byte[dataLength];
                                lastIndex = 0;
                                for (int i = 0; i < lineDataList.Count; i++)
                                {
                                    if (i < 3) continue;
                                    String aa = Encoding.UTF8.GetString(lineDataList[i]);
                                    Array.ConstrainedCopy(lineDataList[i], 0, fileHexData, lastIndex, lineDataList[i].Length);
                                    lastIndex += lineDataList[i].Length;
                                }

                                if (chunkData.DataType == DataTypeEnum.Hex)
                                {
                                    chunkData.HexData = fileHexData;
                                }
                                else
                                {
                                    chunkData.ParamValue = Encoding.UTF8.GetString(fileHexData);
                                }
                            }
                            else
                            {
                                paramIndex = line0.IndexOf("name=\"");
                                if (paramIndex >= 0)
                                {
                                    chunkData.DataType = DataTypeEnum.Param;

                                    //ParamName
                                    String paramName = line0.Substring(paramIndex).Substring("name=\"".Length);
                                    chunkData.ParamName = paramName.Substring(0, paramName.Length - 1);

                                    //ParamValue
                                    //忽略第2行

                                    //从第3行开始剩下的全部数据，为输入的内容
                                    //计算数据长度
                                    int dataLength = 0;
                                    for (int i = 0; i < lineDataList.Count; i++)
                                    {
                                        if (i < 2) continue;
                                        dataLength += lineDataList[i].Length;
                                    }
                                    //填充数据
                                    byte[] fileHexData = new byte[dataLength];
                                    lastIndex = 0;
                                    for (int i = 0; i < lineDataList.Count; i++)
                                    {
                                        if (i < 2) continue;
                                        Array.ConstrainedCopy(lineDataList[i], 0, fileHexData, lastIndex, lineDataList[i].Length);
                                        lastIndex += lineDataList[i].Length;
                                    }
                                    chunkData.ParamValue = Encoding.UTF8.GetString(fileHexData);
                                }

                            }
                        }
                        chunkDataList.Add(chunkData);
                    }
                    //请求客户端处理
                    OnPostDataReceived(chunkDataList, response);
                }
            }
            else
            {
                ///?测试字符串=NULL&阿斯达斯=NULL&p0000=测试内容&p0001=阿斯达
                List<ParamNameValue> paramList = new List<ParamNameValue>();
                if (request.RawUrl.Length > 2)
                {
                    String rawUrl = HttpUtility.UrlDecode(request.RawUrl).Substring(2);
                    String[] rawUrls = rawUrl.Split('&');

                    foreach (String item in rawUrls)
                    {
                        String[] temp = item.Split('=');
                        ParamNameValue paramNameValue = new ParamNameValue();
                        paramNameValue.ParamName = temp[0];
                        paramNameValue.ParamValue = temp[1];
                        paramList.Add(paramNameValue);
                    }
                }
                OnGetDataReceived(paramList, response);
            }
        }
    }

    public class ChunkData
    {
        DataTypeEnum dataType;
        string fileName;
        string contentType;
        byte[] hexData;
        String paramName;
        String paramValue;

        public DataTypeEnum DataType { get => dataType; set => dataType = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public string ContentType { get => contentType; set => contentType = value; }
        public byte[] HexData { get => hexData; set => hexData = value; }
        public string ParamName { get => paramName; set => paramName = value; }
        public string ParamValue { get => paramValue; set => paramValue = value; }
    }
    public enum DataTypeEnum
    {
        Param = 1,
        File = 2,
        Str = 3,
        Hex = 4
    }
    public class ParamNameValue
    {
        private string paramName;
        private string paramValue;

        public ParamNameValue()
        {
            this.paramName = "";
            this.paramValue = "";
        }

        public ParamNameValue(string item, string value)
        {
            this.paramName = item;
            this.paramValue = value;
        }

        public string ParamName { get => paramName; set => paramName = value; }
        public string ParamValue { get => paramValue; set => this.paramValue = value; }
    }
}
