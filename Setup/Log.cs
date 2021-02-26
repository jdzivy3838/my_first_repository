using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public enum LOGTYPE
    {
        CONSOLE,
        TRACE,
        FILE,
        DEBUG
    }
    public enum LOGLEVEL
    {
        INFO,
        DEBUG
    }

    /// <summary>
    /// class used for logging informations within framework. Right now working in static way
    /// TODO: update to non-static execution
    /// </summary>
    class Log
    {
        public static string LogString = "";
        private static LOGLEVEL loglevel = LOGLEVEL.INFO;

        public static LOGLEVEL LogLevel
        {
            set { loglevel = value; }
            get { return loglevel; }
        }
        private static LOGTYPE logType = LOGTYPE.CONSOLE;

        public static LOGTYPE LogType
        {
            set { logType = value; }
            get { return logType; }
        }
        // message type
        public const string LOG_INFO = "INFO";
        public const string LOG_WARNING = "WARNING";
        public const string LOG_ERROR = "ERROR";
        public const string LOG_DEBUG = "DEBUG";

        /// <summary>
        /// Initialization of static class for Log message
        /// </summary>
        /// <param name="logType">typ of log. e.g LOGTYPE.CONSOLE,LOGTYPE.TRACE </param>
        /// <param name="logLevel"></param>
        public static void Init(LOGTYPE logType, LOGLEVEL logLevel)
        {
            LogType = logType;

            switch (LogType)
            {
                case (LOGTYPE.CONSOLE):
                    break;
                case (LOGTYPE.TRACE):
                    Trace.Listeners.Add(new ConsoleTraceListener()); //need to add listener
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        public static void Message(string messageType, string message)
        {
            //if (Setup.GetData("IsGettingMailContentLogs") == "false" && messageType.Equals("DEBUG")) // omitted logs due to reading setting too many times
            //{
            //    Console.WriteLine("This log is omitted due to setting IsGettingMailContentLogs in App.config");
            //    return;
            //}

            LogString = LogString + DateTime.Now + " " + messageType + " : " + message + "\n";
            switch (LogType)
            {
                case LOGTYPE.CONSOLE:
                    Console.WriteLine(DateTime.Now + " " + messageType + " : " + message);
                    break;
                case LOGTYPE.DEBUG:
                    Debug.WriteLine(DateTime.Now + " " + messageType + " : " + message);
                    break;
                case LOGTYPE.TRACE:
                    Trace.WriteLine(DateTime.Now + " " + messageType + " : " + message);
                    break;
                default:
                    break;
            }
        }
    }
}
