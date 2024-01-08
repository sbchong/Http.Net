namespace tinyweb.Handles;

public class HttpHandle
{
    private TcpClient _tcpClient;

    public Guid Id { get; set; }
    public TcpClient TcpClient => _tcpClient;


    public HttpHandle(TcpClient tcpClient)
    {
        Id = Guid.NewGuid();
        _tcpClient = tcpClient;
    }
    public async Task Handle()
    {
        DateTime startTime = DateTime.Now;
        Log.LogInformation($"收到客户端请求：{Id}");


        string requestMessage = await ResolveRequest();
        if (string.IsNullOrWhiteSpace(requestMessage))
        {
            Log.LogInformation($"无效的客户端请求");
            _tcpClient.Close();
            return;
        }


        var request = requestMessage.Split("\r\n");
        var info = request[0].Split(" ");
        var method = info[0];
        var route = info[1];

        if (method == "GET")
        {

            NetworkStream send = _tcpClient.GetStream();
            if (send.CanWrite)
            {
                //await Task.Delay(2000);
                string header = $"HTTP/1.1 200 OK\r\nDate: {DateTime.UtcNow.ToString("r")}\r\nServer:Http.Net\r\n\r\n";
                string body = "OK";
                byte[] msgByte = Encoding.UTF8.GetBytes(header).Concat(Encoding.UTF8.GetBytes(body)).ToArray();
                send.Write(msgByte, 0, msgByte.Length);
                _tcpClient.Close();

                Log.LogInformation($"完成请求{Id}    {route}    200    {(DateTime.Now - startTime).TotalMilliseconds}ms");
            }
        }
    }

    public async Task<string> ResolveRequest()
    {
        string receiveMsg = string.Empty;
        byte[] receiveBytes = new byte[_tcpClient.ReceiveBufferSize];
        int numberOfBytesRead = 0;
        NetworkStream rec = _tcpClient.GetStream();
        if (rec.CanRead)
        {
            do
            {
                numberOfBytesRead = await rec.ReadAsync(receiveBytes, 0, _tcpClient.ReceiveBufferSize);
                receiveMsg += Encoding.UTF8.GetString(receiveBytes, 0, numberOfBytesRead);
            }
            while (rec.DataAvailable);
        }

        return receiveMsg;
    }
}
