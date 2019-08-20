using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Nemont.WPF.AppService
{
    internal class VMMessage : Model.BindingModelBase
    {
        private string message;
        public string Message { get { return message; } set { message = value; OnPropertyChanged(nameof(Message)); } }

        public VMMessage() { }

        public void OnClearOutput()
        {
            SystemLog.Clear();
        }

        public void OnLogChanged()
        {
            Message = SystemLog.Log;
        }
    }

    /// <summary>
    /// MessageDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageDialog : Window
    {
        internal VMMessage ViewModel => DataContext as VMMessage;

        private bool isCompleted = false;
        public bool IsCompleted { get { return isCompleted; } set { isCompleted = value; ButtonCancel.Content = "Close"; } }

        public static bool IsWarning;
        public static ImageSource DefaultIcon;
        public static double? DefaultWidth;
        public static double? DefaultHeight;

        internal MessageDialog(VMMessage viewModel)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            if (DefaultIcon != null) {
                Icon = DefaultIcon;
            }
            if (DefaultWidth != null) {
                Width = (double)DefaultWidth;
            }
            if (DefaultHeight != null) {
                Height = (double)DefaultHeight;
            }

            DataContext = viewModel;
        }

        public static void Clear()
        {
            SystemLog.Clear();
        }

        public static void Run(Action action)
        {
            var vMessage = new VMMessage();
            var wMessage = new MessageDialog(vMessage);
            Exception exception = null;
            IsWarning = false;

            Task.Factory.StartNew(() => {
                try {
                    action();
                }
                catch (Exception ex) {
                    exception = ex;

                    return;
                }
            }).ContinueWith(w => {
                if (exception == null) {
                    wMessage.IsCompleted = true;
                    if (IsWarning == false) {
                        wMessage.Close();
                    }
                }
                else {
                    if (exception is OperationCanceledException) {
                        wMessage.CompleteClose();
                    }
                    SystemLog.WriteLine("\r\n에러가 발생하여 작업을 중단했습니다.\r\n오류 : " + exception.Message);
                    SystemLog.WriteLine(exception.StackTrace);
                    wMessage.IsCompleted = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            wMessage.ShowDialog();
        }

        public void CompleteClose()
        {
            IsCompleted = true;

            Close();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => {
                TextBox.CaretIndex = TextBox.Text.Length;
                TextBox.ScrollToEnd();
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SystemLog.SetLogEvent(ViewModel.OnLogChanged);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (IsCompleted == false) {
                e.Cancel = true;
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonCancel.Content.ToString() == "Close") {
                Close();
            }

            SystemLog.Cancel();
        }
    }
}
