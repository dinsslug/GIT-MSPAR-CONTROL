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
using Nemont.WPF.Service;

namespace Nemont.WPF.AppService.Threading
{
    internal class VMMessageDialog : Model.BindingModelBase
    {
        private string message;
        private string progressText;
        private double progressValue;
        private Visibility progressVisibility = Visibility.Collapsed;
        public string Message { get { return message; } set { message = value; OnPropertyChanged(nameof(Message)); } }
        public string ProgressText { get { return progressText; } set { progressText = value; OnPropertyChanged(nameof(ProgressText)); } }
        public double ProgressValue { get { return progressValue; } set { progressValue = value; OnPropertyChanged(nameof(ProgressValue)); } }
        public Visibility ProgressVisibility { get { return progressVisibility; } set { progressVisibility = value; OnPropertyChanged(nameof(ProgressVisibility)); } }

        public RelayCommand RcClear { get; }

        public VMMessageDialog()
        {
            RcClear = new RelayCommand(OnClear);
        }

        private void OnClear()
        {
            Message = "";
        }

        public void OnMessageChanged(string source)
        {
            Message = source;
        }
    }

    /// <summary>
    /// MessageDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageDialog : Window
    {
        internal VMMessageDialog ViewModel => DataContext as VMMessageDialog;

        private bool isCompleted = false;
        public bool IsCompleted {
            get {
                return isCompleted;
            }
            set {
                isCompleted = value;
                if (value == true) {
                    ButtonCancel.Content = "Close";
                }
            }
        }

        internal MessageDialog(VMMessageDialog viewModel, StartInfo startInfo)
        {
            InitializeComponent();

            DataContext = viewModel;

            Owner = startInfo.Owner ?? Owner;
            Width = startInfo.Width ?? Width;
            Height = startInfo.Height ?? Height;
            Title = startInfo.Title ?? Title;
            ShowInTaskbar = startInfo.ShowInTaskBar ?? ShowInTaskbar;
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
        }
    }
}
