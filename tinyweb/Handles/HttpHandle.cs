namespace tinyweb.Handles;

public class HttpHandle
{
    private readonly TcpClient _tcpClient;
    private string _originRequestMessage;
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
        DateTime startTime = DateTime.Now;
        Log.LogInformation($"收到客户端请求：{Id}");


        if (!await ResolveRequest())
        {
            return;
        }


        if (Method == HttpMethod.GET)
        {
            if (Stream.CanWrite)
            {
                //await Task.Delay(2000);
                string header = $"HTTP/1.1 200 OK\r\nDate: {DateTime.UtcNow.ToString("r")}\r\nServer:Http.Net\r\n\r\n";
                string body = "OK";
                byte[] msgByte = Encoding.UTF8.GetBytes(header).Concat(Encoding.UTF8.GetBytes(body)).ToArray();
                Stream.Write(msgByte, 0, msgByte.Length);
                _tcpClient.Close();

                Log.LogInformation($"完成请求{Id}    {Route}    200    {(DateTime.Now - startTime).TotalMilliseconds}ms");
            }
        }
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
        Log.LogInformation(_originRequestMessage);
        var request = requestMessage.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        var info = request[0].Split(" ");
        Log.LogInformation(info[0]);
        ResolveMethod(info[0]);
        Route = info[1];

        for (int i = 1; i < request.Length; i++)
        {
            var header = request[i].Split(":", StringSplitOptions.RemoveEmptyEntries);
            if (header.Length == 2)
            {
                var key = header[0].Trim();
                var value = header[1].Trim();
                Log.LogInformation($"{key} - {value}");
                Headers.Add(key, value);
            }
        }
        return true;
    }

    private void ResolveMethod(string methodName)
    {
        Method = methodName.ToUpper() switch
        {
            "GET" => HttpMethod.GET,
            "POST" => HttpMethod.POST,
            "PUT" => HttpMethod.PUT,
            "DELETE" => HttpMethod.DELETE,
            "HEAD" => HttpMethod.HEAD,
            "OPTION" => HttpMethod.OPTION,
            _ => HttpMethod.OPTION,
        };
    }
}
