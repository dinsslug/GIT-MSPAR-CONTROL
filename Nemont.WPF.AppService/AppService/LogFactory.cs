using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Nemont.WPF.AppService.Threading;

namespace Nemont.WPF.AppService
{
    public class LogFactory
    {
        public delegate void RaiseLogChangedHandler(string log);
        public virtual event RaiseLogChangedHandler OnLogChanged;
        protected MessageTask Task;
        
        protected string log;
        public virtual string Log {
            get {
                return log;
            }
            set {
                log = value;

                OnLogChanged?.Invoke(log);
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

        public LogFactory() { }

        public virtual void RunTask(Action<MessageTask> method)
        {
            Task = new MessageTask();
            Task.OnProcessChanged += (line) => WriteLine(line);
            Task.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            Task.WorkerAction = new Action(() => {
                try {
                    Task.Process.Kill();
                }
                catch {
                    Debug.WriteLine("Failed to kill process.");
                }

                method(Task);
            });
            Task.Worker.RunWorkerAsync();
        }

        protected virtual void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Task.WorkerCompleteAction?.Invoke();

            if (Task.Exception == null) {
                WriteLine("\r\nPROCESS IS COMPLETED");
            }
            else {
                if (Task.Exception is TaskCanceledException) {
                    WriteLine("\r\nPROCESS IS CANCELED");

                    return;
                }
                WriteLine("\r\nPROCESS ABORTED DUE TO AN ERROR.\r\nERROR : " + Task.Exception.Message);
                WriteLine(Task.Exception.StackTrace);
            }
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
