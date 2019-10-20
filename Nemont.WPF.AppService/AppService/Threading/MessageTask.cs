using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Nemont.WPF.AppService.Threading
{
    internal delegate void RaiseProcessChangedHandler(string line);

    public class MessageTask 
    {
        internal event RaiseProcessChangedHandler OnProcessChanged;

        internal bool IsHandleException;

        /// <summary>
        /// 현재 작업 중인지에 대한 여부를 가져옵니다.
        /// </summary>
        public bool IsBusy => Worker.IsBusy;

        public bool IsWarning = false;
        public bool IsCompleted = false;
        public bool IsCanceled = false;
        public Process Process;
        public string ProcessLog;

        internal BackgroundWorker Worker;
        internal Action WorkerAction;
        internal Action WorkerCompleteAction = null;

        internal Exception Exception;

        internal MessageTask()
        {
            Worker = new BackgroundWorker();
            Worker.DoWork += Worker_DoWork;
            Worker.WorkerSupportsCancellation = true;
        }

        public void InitializeProcess()
        {
            Process = new Process();
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardInput = true;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.RedirectStandardError = true;
            Process.StartInfo.CreateNoWindow = true;
            Process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            Process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            Process.Exited += Process_Exited;
            Process.EnableRaisingEvents = true;
            ProcessLog = "";
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Process = null;
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            OnProcessChanged?.Invoke(outLine.Data);

            ProcessLog += outLine.Data + "\r\n";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (IsHandleException == true) {
                try {
                    WorkerAction?.Invoke();
                }
                catch (Exception ex) {
                    Exception = ex;
                }
            }
            else {
                try {
                    WorkerAction?.Invoke();
                }
                catch (TaskCanceledException ex) {
                    Exception = ex;
                }
            }
        }

        public void OnStopProcess()
        {
            if (Process != null && Process.HasExited == false) {
                Process.Kill();
            }
            Worker.CancelAsync();
        }

        public void ThrowIfCancellationRequested()
        {
            if (Worker.CancellationPending == true) {
                IsCanceled = true;
                throw new TaskCanceledException();
            }
        }
    }
}
