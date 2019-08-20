using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nemont.WPF.AppService.Progress
{
    public static class ProgressManager
    {
        private static Action RaiseMessageChanged;
        private static Action RaiseProgressTextChanged;
        private static Action RaiseProgressValueChanged;

        private static CancellationTokenSource CancellationTokenSource;
        public static CancellationToken CancellationToken;

        private static string message;
        public static string Message {
            get { return message; }
            set { message = value; RaiseMessageChanged?.Invoke(); }
        }

        private static string progressText;
        public static string ProgressText {
            get { return progressText; }
            set { progressText = value; RaiseProgressTextChanged?.Invoke(); }
        }

        private static double progressValue;
        public static double ProgressValue {
            get { return progressValue; }
            set { progressValue = value; RaiseProgressValueChanged?.Invoke(); }
        }

        public static void Initialize()
        {
            message = "";
            progressText = "";
            progressValue = 0.0;

            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
        }

        public static void Cancel()
        {
            CancellationTokenSource.Cancel();
        }

        public static void SetProgressEvent(Action onMessageChanged, Action onProgressTextChanged, Action onProgressValueChanged)
        {
            RaiseMessageChanged = onMessageChanged;
            RaiseProgressTextChanged = onProgressTextChanged;
            RaiseProgressValueChanged = onProgressValueChanged;
            RaiseMessageChanged();
            RaiseProgressTextChanged();
            RaiseProgressValueChanged();
        }
    }
}
