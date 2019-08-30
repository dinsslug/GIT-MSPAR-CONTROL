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
    public class MessageTask
    {
        internal MessageDialog WDialog { get; }
        internal VMMessageDialog VDialog => WDialog.ViewModel;

        public bool IsWarning = false;

        public Process Process;
        public string ProcessLog;

        internal BackgroundWorker Worker;
        internal Action WorkerAction;
        internal Action WorkerCompleteAction = null;

        internal Exception Exception;

        internal MessageTask(MessageDialog window)
        {
            WDialog = window;
            WDialog.ButtonCancel.Click += ButtonCancel_Click;

            Worker = new BackgroundWorker();
            Worker.DoWork += Worker_DoWork;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Worker.WorkerSupportsCancellation = true;
        }

        public void InitializeProgress()
        {
            WDialog.Dispatcher.Invoke(() => {
                VDialog.ProgressVisibility = Visibility.Visible;
                VDialog.ProgressText = "";
                VDialog.ProgressValue = 0.0;
            });
        }

        public void CompleteProgress()
        {
            WDialog.Dispatcher.Invoke(() => {
                VDialog.ProgressVisibility = Visibility.Collapsed;
            });
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
            WriteLine(outLine.Data);

            ProcessLog += outLine.Data + "\r\n";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try {
                WorkerAction?.Invoke();
            }
            catch (Exception ex) {
                Exception = ex;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerCompleteAction?.Invoke();

            if (Exception == null) {
                WDialog.IsCompleted = true;
                if (IsWarning == false) {
                    WDialog.Close();
                }
            }
            else {
                if (Exception is TaskCanceledException) {
                    WDialog.CompleteClose();
                }
                WriteLine("\r\nTHE PROCESS ABORTED DUE TO AN ERROR.\r\nERROR : " + Exception.Message);
                WriteLine(Exception.StackTrace);

                WDialog.IsCompleted = true;
            }
        }

        public void OnStopProcess()
        {
            if (Process != null && Process.HasExited == false) {
                Process.Kill();
            }
            Worker.CancelAsync();
        }

        public void Clear()
        {
            WDialog.Dispatcher.Invoke(() => {
                VDialog.Message = "";
            });
        }

        public void Write(string message)
        {
            WDialog.Dispatcher.Invoke(() => {
                VDialog.Message += message;
            });
        }

        public void WriteLine(string message)
        {
            WDialog.Dispatcher.Invoke(() => {
                VDialog.Message += message + "\r\n";
            });
        }

        public void SetProgressText(string message)
        {
            WDialog.Dispatcher.Invoke(() => {
                VDialog.ProgressText = message;
            });
        }

        public void SetProgressValue(double value)
        {
            WDialog.Dispatcher.Invoke(() => {
                VDialog.ProgressValue = value;
            });
        }

        public void ThrowIfCancellationRequested()
        {
            if (Worker.CancellationPending == true) {
                throw new TaskCanceledException();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (WDialog.ButtonCancel.Content.ToString() == "Close") {
                WDialog.Close();
            }

            OnStopProcess();
        }
    }
}
