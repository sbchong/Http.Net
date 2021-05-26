using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Http.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress iPAddress = IPAddress.Parse("0.0.0.0");
            int port = 443;
            var ipe = new IPEndPoint(iPAddress, port);
            var listenner = new TcpListener(ipe);

            listenner.Start();
            Console.WriteLine("启动");
            while (true)
            {
                var client = listenner.AcceptTcpClient();

                if (client.Connected)
                {
                    Task.Run(() =>
                    {
                        string receiveMsg = string.Empty;
                        byte[] receiveBytes = new byte[client.ReceiveBufferSize];
                        int numberOfBytesRead = 0;
                        NetworkStream rec = client.GetStream();

                        if (rec.CanRead)
                        {
                            do
                            {
                                numberOfBytesRead = rec.Read(receiveBytes, 0, client.ReceiveBufferSize);
                                receiveMsg += Encoding.UTF8.GetString(receiveBytes, 0, numberOfBytesRead);
                            }
                            while (rec.DataAvailable);
                        }

                        //Console.WriteLine(receiveMsg);

                        if (string.IsNullOrEmpty(receiveMsg))
                        {
                            return;
                        }
                        var request = receiveMsg.Split("\r\n");
                        var info = request[0].Split(" ");
                        var method = info[0];
                        var route = info[1];

                        if (method == "GET")
                        {
                            string path = @"C:\Users\Y-Y-L.DESKTOP-NCB8HH5\source\repos\Http.Net\Http.Net\bin\Debug\net5.0\html";
                            var response = string.Empty;
                            byte[] resp = null;
                            if (route == "/" || route == "/index.html")
                            {
                                path = Path.Combine(path, "index.html");
                            }
                            else
                            {
                                var paths = route.Split('/');
                                foreach (var item in paths)
                                {
                                    path = Path.Combine(path, item);
                                }

                            }
                            Console.WriteLine(path);
                            if (File.Exists(path))
                            {
                                //if (new string[] { ".html", ".css", ".js" }.Contains(Path.GetExtension(path)))
                                //    response = File.ReadAllText(path, Encoding.UTF8);
                                //if (new string[] { ".jpg", ".png", ".webp" }.Contains(Path.GetExtension(path)))
                                //   response = File.ReadAllText(path, Encoding.UTF8);

                                //using var fs = File.OpenRead(path);
                                //resp = new byte[fs.Length];
                                //fs.Read(resp, 0, resp.Length);
                                resp = File.ReadAllBytes(path);

                                NetworkStream send = client.GetStream();
                                if (send.CanWrite)
                                {
                                    string header = $"HTTP/1.1 200 OK\r\nDate: {DateTime.UtcNow.ToString("r")}\r\nServer:Http.Net\r\n\r\n";

                                    byte[] msgByte = Encoding.UTF8.GetBytes(header).Concat(resp).ToArray();
                                    send.Write(msgByte, 0, msgByte.Length);
                                    client.Close();

                                    //string header = $"HTTP/1.1 200 OK\r\nDate: {DateTime.UtcNow.ToString("r")}\r\nServer:Http.Net\r\n\r\n";

                                    //byte[] msgByte = Encoding.UTF8.GetBytes(response);

                                    //MemoryStream ms = new MemoryStream();
                                    //using (GZipStream zipStream = new GZipStream(ms, CompressionMode.Compress))   //代开压缩文件流
                                    //{
                                    //    zipStream.Write(msgByte, 0, msgByte.Length);  //写入压缩文件
                                    //    var bytes = ms.ToArray();
                                    //    var res = msgByte.Concat(bytes).ToArray();
                                    //    //ms.CopyTo(send);

                                    //    send.Write(res, 0, res.Length);
                                    //    client.Close();
                                    //}
                                }
                            }
                            else
                            {
                                NetworkStream send = client.GetStream();
                                if (send.CanWrite)
                                {
                                    string header = $"HTTP/1.1 404 OK\r\nDate: {DateTime.UtcNow.ToString("r")}\r\nServer:Http.Net\r\nContent-Type: image/x-icon\r\n\r\n";

                                    byte[] msgByte = Encoding.UTF8.GetBytes(header);
                                    send.Write(msgByte, 0, msgByte.Length);
                                    client.Close();
                                }
                            }
                        }
                    });
                }
            }
        }
    }
}
