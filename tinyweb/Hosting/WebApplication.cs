using tinyweb.Handles;

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
        int count = 0;
        while (true)
        {
            Log.LogInformation($"当前处理请求数量：{count}");
            var client = await Listener.AcceptTcpClientAsync();

            if (client.Connected)
            {
                count++;
                HttpHandle handle = new HttpHandle(client);

                await handle.Handle();
            }
        }
    }
}
