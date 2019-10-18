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
    public delegate void RaiseLogChangedHandler(string log);

    public class LogFactory
    {
        public virtual event RaiseLogChangedHandler OnLogChanged;
        public MessageTask Task;

        protected Stopwatch Stopwatch;
        public int UpdateIntervalTime = 0;

        protected string log;
        public virtual string Log {
            get { return log; }
            set {
                log = value;

                if (Stopwatch.ElapsedMilliseconds > UpdateIntervalTime) {
                    OnLogChanged?.Invoke(value);

                    Stopwatch.Restart();
                }
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

        public LogFactory()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public void RunTask(Action<LogFactory> method)
        {
            Action action = () => {
                try {
                    Task.Process.Kill();
                }
                catch {
                    Debug.WriteLine("Failed to kill process.");
                }
                method.Invoke(this);
            };
            _RunTask(action);
        }

        public virtual void RunTask(Action method)
        {
            Action action = () => {
                try {
                    Task.Process.Kill();
                }
                catch {
                    Debug.WriteLine("Failed to kill process.");
                }
                method.Invoke();
            };
            _RunTask(action);
        }

        protected virtual void _RunTask(Action action)
        {
            if (Task != null && Task.IsBusy == true) {
                return;
            }
            Task = new MessageTask();
            Task.OnProcessChanged += (line) => WriteLine(line);
            Task.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Task.WorkerAction = action;

            Task.Worker.RunWorkerAsync();
        }

        protected virtual void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try {
                Task.WorkerCompleteAction?.Invoke();

                if (Task.Exception == null) {
                    WriteLine("\r\nPROCESS IS COMPLETED");
                }
                else {
                    if (Task.Exception is TaskCanceledException) {
                        WriteLine("\r\nPROCESS IS CANCELED");

                        return;
                    }
                    WriteLine("\r\nPROCESS ABORTED DUE TO AN ERROR.\r\n" + Task.Exception.Message);
                    WriteLine(Task.Exception.StackTrace);
                }
            }
            finally {
                Task.IsCompleted = true;
                Flush();
                Task = null;
            }
        }

        public void StopTask()
        {
            Task?.OnStopProcess();
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

        /// <summary>
        /// 작업 중단 또는 종료 시 표시되지 않은 로그를 모두 출력하도록 인보크를 수행합니다.
        /// </summary>
        public virtual void Flush()
        {
            OnLogChanged?.Invoke(log);
        }
    }
}
