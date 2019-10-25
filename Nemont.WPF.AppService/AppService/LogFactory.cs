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

        /// <summary>
        /// 작업 내에서 예외가 발생할 경우 예외를 처리하고 메시지를 보낼 것인지 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool IsHandleException = false;

        /// <summary>
        /// 작업이 완료되었을 경우 보낼 메시지를 가져오거나 설정합니다.
        /// </summary>
        public string TaskMessageCompleted = "";

        /// <summary>
        /// 작업이 취소되었을 경우 보낼 메시지를 가져오거나 설정합니다.
        /// </summary>
        public string TaskMessageCanceled = "";

        /// <summary>
        /// 작업 중 예외가 발생했을 경우 메시지를 가져오거나 설정합니다.
        /// </summary>
        public string TaskMessageErrorOccurred = "";

        public string DefaultTaskMessageCompleted = "";
        public string DefaultTaskMessageCanceled = "";
        public string DefaultTaskMessageErrorOccurred = "";

        /// <summary>
        /// 한 작업이 끝날 경우 작업 메시지를 기본값으로 초기화할지에 대한 여부를 가져오거나 설정합니다.
        /// </summary>
        public bool IsClearTaskMessageTerminated = true;

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
            Task.IsHandleException = IsHandleException;
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
                    Write(TaskMessageCompleted);
                }
                else {
                    if (Task.Exception is TaskCanceledException) {
                        Write(TaskMessageCanceled);

                        return;
                    }
                    Write(TaskMessageErrorOccurred);
                    WriteLine(Task.Exception.Message);
                    WriteLine(Task.Exception.StackTrace);
                }
            }
            finally {
                if (IsClearTaskMessageTerminated == true) {
                    TaskMessageCompleted = DefaultTaskMessageCompleted;
                    TaskMessageCanceled = DefaultTaskMessageCanceled;
                    TaskMessageErrorOccurred = DefaultTaskMessageErrorOccurred;
                }
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

            Flush();
        }

        public string GetLog()
        {
            return Log;
        }

        public void Write(string value)
        {
            Write(value, true);
        }

        public void Write(string value, bool isFlush)
        {
            LogBody += value;

            if (isFlush == true) {
                Flush();
            }
        }

        public void WriteLine(string value)
        {
            WriteLine(value, true);
        }

        public void WriteLine(string value, bool isFlush)
        {
            LogBody += value + "\r\n";

            if (isFlush == true) {
                Flush();
            }
        }

        public void ReplaceLine(string value)
        {
            ReplaceLine(value, true);
        }

        public void ReplaceLine(string value, bool isFlush)
        {
            LogLine = value;

            if (isFlush == true) {
                Flush();
            }
        }

        public void PrintError(string message, Exception ex)
        {
            WriteLine("\r\n> " + message);
            WriteLine("> " + ex.Message);
            WriteLine("> Stack Trace : ");
            WriteLine(ex.StackTrace + "\r\n");

            Flush();
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
