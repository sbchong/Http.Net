namespace Http.Net.Hosting;

public class WebApplication
{
    public IPAddress IPAddress { get; set; }

    public TcpListener Listener { get; set; }

    public WebApplication()
    {
        IPAddress = IPAddress.Any;

    }

    public async Task RunAsycn()
    {
        int port = 80;
        var ipe = new IPEndPoint(IPAddress, port);
        Listener = new TcpListener(ipe);
        Listener.Start();



        Log.LogInformation($"HTTP服务器启动成功，监听：http://{ipe}");
        while (true)
        {
            var client = Listener.AcceptTcpClient();

            if (client.Connected)
            {
                Task.Run(async () =>
                {
                    Guid requestId = Guid.NewGuid();
                    DateTime startTime = DateTime.Now;
                    Log.LogInformation($"收到客户端请求：{requestId}");
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

                        NetworkStream send = client.GetStream();
                        if (send.CanWrite)
                        {
                            //await Task.Delay(2000);
                            string header = $"HTTP/1.1 200 OK\r\nDate: {DateTime.UtcNow.ToString("r")}\r\nServer:Http.Net\r\n\r\n";
                            string body = "OK";
                            byte[] msgByte = Encoding.UTF8.GetBytes(header).Concat(Encoding.UTF8.GetBytes(body)).ToArray();
                            send.Write(msgByte, 0, msgByte.Length);
                            client.Close();

                            Log.LogInformation($"完成请求{requestId}    {route}    200    {(DateTime.Now - startTime).TotalMilliseconds}ms");
                        }
                    }
                });
            }
        }
    }
}
