using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using Nemont.WPF.Service;

namespace Nemont.WPF.AppService.Threading
{
    internal class DMessageDialog : Model.BindingModelBase
    {
        private string message;
        private string progressText;
        private double progressValue;
        private bool isProcessMode;
        private Visibility progressVisibility = Visibility.Collapsed;
        public string Message { get { return message; } set { message = value; OnPropertyChanged(nameof(Message)); } }
        public string ProgressText { get { return progressText; } set { progressText = value; OnPropertyChanged(nameof(ProgressText)); } }
        public double ProgressValue { get { return progressValue; } set { progressValue = value; OnPropertyChanged(nameof(ProgressValue)); } }
        public bool IsProcessMode { get { return isProcessMode; } set { isProcessMode = value; OnPropertyChanged(nameof(IsProcessMode)); } }
        public Visibility ProgressVisibility { get { return progressVisibility; } set { progressVisibility = value; OnPropertyChanged(nameof(ProgressVisibility)); } }

        public RelayCommand RcClear { get; set; }

        public DMessageDialog()
        {
            RcClear = new RelayCommand(OnClear);
        }

        public void OnMessageChanged(string text)
        {
            Message = text;

            Application.Current.Dispatcher.Invoke((ThreadStart)(() => { }), DispatcherPriority.ApplicationIdle);
        }

        public void OnProgressChanged(string progressText, double progressValue)
        {
            ProgressText = progressText;
            ProgressValue = progressValue;

            Application.Current.Dispatcher.Invoke((ThreadStart)(() => { }), DispatcherPriority.ApplicationIdle);
        }

        private void OnClear()
        {
            Message = "";
        }
    }

    /// <summary>
    /// MessageDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageDialog : Window
    {
        internal DMessageDialog ViewModel => DataContext as DMessageDialog;

        private bool isCompleted = false;
        public bool IsCompleted {
            get {
                return isCompleted;
            }
            set {
                isCompleted = value;
                if (value == true) {
                    ViewModel.IsProcessMode = false;
                }
            }
        }

        internal MessageDialog(DMessageDialog viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        /*
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
        */

        public void CompleteClose()
        {
            IsCompleted = true;

            Close();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox.CaretIndex = TextBox.Text.Length;
            TextBox.ScrollToEnd();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (IsCompleted == false) {
                e.Cancel = true;
            }
            Hide();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
