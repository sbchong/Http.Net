namespace Http.Net.Loger;

public class Loger
{
    private List<LogEvent> next = new List<LogEvent>();
    private bool isBusy = false;
    private void LogInner(LogLevel level, string message)
    {
        LogEvent logEvent = new(level, message);
        if (!isBusy)
        {
            isBusy = true;
            var levelStr = logEvent.LogLevel.ToString().ToLower();
            _ = level switch
            {
                LogLevel.Info => Console.ForegroundColor = ConsoleColor.Green,
                LogLevel.Warn => Console.ForegroundColor = ConsoleColor.Yellow,
                LogLevel.Error => Console.ForegroundColor = ConsoleColor.Red,
                _ => Console.ForegroundColor = ConsoleColor.Green,
            };
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.Write(levelStr);
            //Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{levelStr} : {logEvent.Message}");
            //Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            isBusy = false;
        }
        else
        {
            next.Add(logEvent);
        }
    }

    public void LogInformation(string message)
    {
        LogInner(LogLevel.Info, message);
    }

    public void LogWarn(string message)
    {
        LogInner(LogLevel.Warn, message);
    }

    public void LogError(string message)
    {
        LogInner(LogLevel.Error, message);
    }
}

