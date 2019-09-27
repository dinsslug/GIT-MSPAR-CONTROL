using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Nemont.WPF.AppService.Threading;

namespace Nemont.WPF.AppService
{
    public class LogDialogFactory : LogFactory
    {
        public delegate void RaiseProgressChangedHandler(string text, double value);
        public override event RaiseLogChangedHandler OnLogChanged;
        public event RaiseProgressChangedHandler OnProgressChanged;

        private DMessageDialog DMessageDialog;
        private MessageDialog WMessageDialog;
        private Stopwatch Stopwatch;

        public int UpdateInterval = 500;
        private bool IsDialog = false;

        private string progressText;
        public string ProgressText {
            get => progressText;
            set {
                progressText = value;

                if (Stopwatch.ElapsedMilliseconds > UpdateInterval) {
                    if (IsDialog == false) {
                        WMessageDialog.Show();
                    }
                    else {
                        WMessageDialog.ShowDialog();
                    }

                    OnProgressChanged?.Invoke(value, ProgressValue);
                }
            }
        }

        private double progressValue;
        public double ProgressValue {
            get => progressValue;
            set {
                progressValue = value;

                if (Stopwatch.ElapsedMilliseconds > UpdateInterval) {
                    if (IsDialog == false) {
                        WMessageDialog.Show();
                    }
                    else {
                        WMessageDialog.ShowDialog();
                    }

                    OnProgressChanged?.Invoke(ProgressText, value);

                    Stopwatch.Restart();
                }
            }
        }

        public override string Log {
            get => log;
            set {
                log = value;

                if (Stopwatch.ElapsedMilliseconds > UpdateInterval) {
                    if (IsDialog == false) {
                        WMessageDialog.Show();
                    }
                    else {
                        WMessageDialog.ShowDialog();
                    }

                    OnLogChanged?.Invoke(value);

                    Stopwatch.Restart();
                }
            }
        }

        public LogDialogFactory() : base()
        {
            DMessageDialog = new DMessageDialog();
            WMessageDialog = new MessageDialog(DMessageDialog);

            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            OnLogChanged += DMessageDialog.OnMessageChanged;
            OnProgressChanged += DMessageDialog.OnProgressChanged;
            DMessageDialog.RcClear = new Service.RelayCommand(() => { Clear(); });
        }

        public LogDialogFactory(StartInfo dialogStartInfo) : this()
        {
            SetDialogInfo(dialogStartInfo);
        }

        public override void RunTask(Action<MessageTask> method)
        {
            DMessageDialog.IsProcessMode = true;
            WMessageDialog.ButtonCancel.Click += (sender, e) => Task.OnStopProcess();

            base.RunTask(method);
        }

        protected override void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Task.WorkerCompleteAction?.Invoke();

            if (Task.Exception == null) {
                WriteLine("\r\nPROCESS IS COMPLETED");
                WMessageDialog.IsCompleted = true;
                if (Task.IsWarning == false) {
                    WMessageDialog.Close();
                }
            }
            else {
                if (Task.Exception is TaskCanceledException) {
                    WriteLine("\r\nPROCESS IS CANCELED");

                    WMessageDialog.CompleteClose();
                }
                WriteLine("\r\nTHE PROCESS ABORTED DUE TO AN ERROR.\r\nERROR : " + Task.Exception.Message);
                WriteLine(Task.Exception.StackTrace);

                WMessageDialog.IsCompleted = true;
            }
        }

        public void SetDialogInfo(StartInfo dialogStartInfo)
        {
            WMessageDialog.Owner = dialogStartInfo.Owner ?? WMessageDialog.Owner;
            WMessageDialog.Width = dialogStartInfo.Width ?? WMessageDialog.Width;
            WMessageDialog.Height = dialogStartInfo.Height ?? WMessageDialog.Height;
            WMessageDialog.Title = dialogStartInfo.Title ?? WMessageDialog.Title;
            WMessageDialog.ShowInTaskbar = dialogStartInfo.ShowInTaskBar ?? WMessageDialog.ShowInTaskbar;
            IsDialog = dialogStartInfo.IsDialog;
        }

        public void SetProgressText(string text)
        {
            ProgressText = text;
        }

        public void SetProgressValue(double value)
        {
            ProgressValue = value;
        }

        public void InitializeProgress()
        {
            WMessageDialog.Dispatcher.Invoke(() => {
                DMessageDialog.ProgressVisibility = Visibility.Visible;
                DMessageDialog.ProgressText = "";
                DMessageDialog.ProgressValue = 0.0;
            });
        }

        public void CompleteProgress()
        {
            WMessageDialog.Dispatcher.Invoke(() => {
                DMessageDialog.ProgressVisibility = Visibility.Collapsed;
            });
        }

        public void CloseDialog()
        {
            WMessageDialog.CompleteClose();
        }

        public void Flush()
        {
            OnLogChanged?.Invoke(log);
            OnProgressChanged?.Invoke(ProgressText, ProgressValue);
        }
    }
}
