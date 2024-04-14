namespace tinyweb.Handles;

public class Request
{
    public HttpMethod Method { get; set; }
    public string Host { get; set; }
    public string Route { get; private set; }
    public Dictionary<string, string> Headers { get; private set; }

    public Request(string requestMessage)
    {
        Headers = new Dictionary<string, string>();
        Log.LogInformation(requestMessage);
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
