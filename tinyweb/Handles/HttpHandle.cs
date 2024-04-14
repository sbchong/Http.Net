using System.Diagnostics;

namespace tinyweb.Handles;

public class HttpHandle
{
    private readonly TcpClient _tcpClient;
    private string _originRequestMessage;

    public Request Request { get; private set; }


    public TcpClient TcpClient => _tcpClient;

    public NetworkStream Stream => _tcpClient.GetStream();

    public Guid Id { get; set; }

    public HttpMethod Method { get; set; }
    public string Route { get; set; }

    public Dictionary<string, string> Headers { get; set; }


    public HttpHandle(TcpClient tcpClient)
    {
        Id = Guid.NewGuid();
        Headers = new Dictionary<string, string>();
        _tcpClient = tcpClient;
    }
    public async Task Handle()
    {
        var sw = Stopwatch.StartNew();
        Log.LogInformation($"收到客户端请求：{Id}");


        if (await ResolveRequest())
        {
            await Response();
        }


        sw.Stop();
        Log.LogInformation($"完成请求{Id}    {Route}    200    {sw.ElapsedMilliseconds}ms");
    }

    private async Task<bool> ResolveRequest()
    {
        string requestMessage = string.Empty;
        byte[] receiveBytes = new byte[_tcpClient.ReceiveBufferSize];
        int numberOfBytesRead = 0;

        if (Stream.CanRead)
        {
            do
            {
                numberOfBytesRead = await Stream.ReadAsync(receiveBytes, 0, _tcpClient.ReceiveBufferSize);
                requestMessage += Encoding.UTF8.GetString(receiveBytes, 0, numberOfBytesRead);
            }
            while (Stream.DataAvailable);
        }
        if (string.IsNullOrWhiteSpace(requestMessage))
        {
            Log.LogInformation($"无效的客户端请求");
            _tcpClient.Close();
            return false;
        }

        _originRequestMessage = requestMessage;
        Request = new Request(_originRequestMessage);
        return true;
    }



    private async Task Response()
    {
        if (Request.Method == HttpMethod.GET)
        {
            if (Stream.CanWrite)
            {
                //await Task.Delay(2000);
                string header = $"HTTP/1.1 200 OK\r\nDate: {DateTime.UtcNow.ToString("r")}\r\nServer:Http.Net\r\n\r\n";
                string body = "这次请求狠狠地OK，哈哈哈";
                byte[] msgByte = Encoding.UTF8.GetBytes(header).Concat(Encoding.UTF8.GetBytes(body)).ToArray();
                await Stream.WriteAsync(msgByte, 0, msgByte.Length);
                _tcpClient?.Close();
            }
        }
    }
}
