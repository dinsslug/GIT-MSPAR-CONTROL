using System;
using System.Collections.Generic;
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
using Nemont.WPF.AppService.Progress;
using Nemont.WPF.Service;

namespace Nemont.WPF.AppService.Threading
{
    internal class VMProgressDialog : Model.BindingModelBase
    {
        public string Message { get; set; }
        public string ProgressText { get; set; }
        public double ProgressValue { get; set; }
        public Visibility VisibilityProgress { get; }
        public Visibility VisibilityButton { get; }

        public VMProgressDialog(Cancelable cancelable, IsProgress isProgress)
        {
            if (isProgress == IsProgress.Yes) {
                VisibilityProgress = Visibility.Visible;
            }
            else {
                VisibilityProgress = Visibility.Collapsed;
            }

            if (cancelable == Cancelable.No) {
                VisibilityButton = Visibility.Collapsed;
            }
            else {
                VisibilityButton = Visibility.Visible;
            }
        }

        public void OnMessageChanged()
        {
            Message = ProgressManager.Message;

            OnPropertyChanged(nameof(Message));
        }

        public void OnProgressTextChanged()
        {
            ProgressText = ProgressManager.ProgressText;

            OnPropertyChanged(nameof(ProgressText));
        }

        public void OnProgressValueChanged()
        {
            ProgressValue = ProgressManager.ProgressValue;

            OnPropertyChanged(nameof(ProgressValue));
        }
    }

    /// <summary>
    /// ProgressDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgressDialog : Window
    {
        internal VMProgressDialog ViewModel => DataContext as VMProgressDialog;

        internal ProgressDialog(VMProgressDialog viewModel)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            DataContext = viewModel;
        }

        /// <summary>
        /// 작업을 다른 스레드에서 실행하고 프로세스 창을 엽니다.
        /// </summary>
        public static void Run(Action action, bool isCancelable, bool isProgressBar)
        {
            var vProgress = new VMProgressDialog(
                (isCancelable == true) ? Cancelable.Yes : Cancelable.No,
                (isProgressBar == true) ? IsProgress.Yes : IsProgress.No);
            var wProgress = new ProgressDialog(vProgress);
            ProgressManager.Initialize();
            Exception exception = null;

            Task.Factory.StartNew(() => {
                try {
                    Thread.Sleep(100);
                    action();
                }
                catch (Exception ex) {
                    exception = ex;

                    return;
                }
            }, ProgressManager.CancellationToken).ContinueWith(w => {
                wProgress.Close();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            wProgress.ShowDialog();
            if (exception != null) {
                if (exception is OperationCanceledException) {
                    return;
                }
                throw exception;
            }
        }

        /// <summary>
        /// 작업을 다른 스레드에서 실행하고 프로세스 창을 엽니다.
        /// </summary>
        public static void Run(Action action)
        {
            Run(action, false, false);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ProgressManager.Cancel();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) {
                DragMove();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressManager.SetProgressEvent(ViewModel.OnMessageChanged, ViewModel.OnProgressTextChanged, ViewModel.OnProgressValueChanged);
        }
    }
}
