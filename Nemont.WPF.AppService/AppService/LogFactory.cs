using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nemont.WPF.AppService
{
    public class LogFactory
    {
        private Action<string> RaiseLogChanged;

        private string log;
        public string Log {
            get {
                return log;
            }
            set {
                log = value;

                RaiseLogChanged?.Invoke(log);
            }
        }
        private string logBody = "";
        private string LogBody {
            get { return logBody; }
            set {
                logBody = value;
                Log = LogBody;
            }
        }

        private string logLine = "";
        private string LogLine {
            get { return logLine; }
            set {
                logLine = value;
                Log = LogBody + logLine;
            }
        }

        public void SetLogEvent(Action<string> onLogChanged)
        {
            RaiseLogChanged = onLogChanged;
            RaiseLogChanged(Log);
        }

        public void Clear()
        {
            LogBody = "";
            LogLine = "";
        }

        public string GetLog()
        {
            return Log;
        }

        public void Write(string value)
        {
            LogBody += value;
        }

        public void WriteLine()
        {
            LogBody += "\r\n";
        }

        public void WriteLine(string value)
        {
            LogBody += value + "\r\n";
        }

        public void ReplaceLine(string value)
        {
            LogLine = value;
        }

        public void PrintError(string message, Exception ex)
        {
            WriteLine("\r\n> " + message);
            WriteLine("> " + ex.Message);
            WriteLine("> Stack Trace : ");
            WriteLine(ex.StackTrace + "\r\n");
        }
    }
}
