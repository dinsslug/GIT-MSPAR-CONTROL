using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nemont.WPF.AppService
{
    public static class SystemLog
    {
        private static Action RaiseLogChanged;

        private static CancellationTokenSource CancellationTokenSource;
        public static CancellationToken CancellationToken;

        private static string log;
        public static string Log {
            get {
                return log;
            }
            set {
                log = value;

                RaiseLogChanged?.Invoke();
            }
        }
        private static string logBody = "";
        private static string LogBody {
            get { return logBody; }
            set {
                logBody = value;
                Log = LogBody;
            }
        }

        private static string logLine = "";
        private static string LogLine {
            get { return logLine; }
            set {
                logLine = value;
                Log = LogBody + logLine;
            }
        }

        public static void SetLogEvent(Action onLogChanged)
        {
            RaiseLogChanged = onLogChanged;
            RaiseLogChanged();
        }

        public static void Cancel()
        {
            CancellationTokenSource.Cancel();
        }

        public static void Clear()
        {
            LogBody = "";
            LogLine = "";

            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
        }

        public static string GetLog()
        {
            return Log;
        }

        public static void Write(string text)
        {
            LogBody += text;
        }

        public static void WriteLine()
        {
            LogBody += "\r\n";
        }

        public static void WriteLine(string text)
        {
            LogBody += text + "\r\n";
        }

        public static void WriteUpdateOneLine(string text)
        {
            LogLine = text;
        }

        public static void WarningJsonLoad(string text)
        {
            WriteLine("> Warning : Loading " + text + " is failed.");
        }

        public static void PrintError(string title, Exception ex)
        {
            WriteLine("\r\n> " + title);
            WriteLine("> " + ex.Message);
            WriteLine("> Stack Trace : ");
            WriteLine(ex.StackTrace + "\r\n");
        }
    }
}
