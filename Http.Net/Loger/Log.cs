using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Http.Net.Loger
{
    public class Log
    {
        public static Loger Loger => new Loger();



        public static void LogInformation(string message)
        {
            Loger.LogInformation(message);
        }

        public static void LogWarn(string message)
        {
            Loger.LogWarn(message);
        }

        public static void LogError(string message)
        {
            Loger.LogError(message);
        }
    }
}
