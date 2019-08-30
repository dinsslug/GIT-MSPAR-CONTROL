using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.WPF.AppService.Threading
{
    public class ProgressTask
    {
        internal ProgressDialog WDialog { get; }
        internal VMProgressDialog VDialog => WDialog.ViewModel;

        public bool IsWarning = false;

        internal BackgroundWorker Worker;
        internal Action WorkerAction;
        internal Action WorkerCompleteAction = null;

        internal Exception Exception;

        internal ProgressTask(ProgressDialog window)
        {
            WDialog = window;
            WDialog.ButtonCancel.Click += ButtonCancel_Click;

            Worker = new BackgroundWorker();
            Worker.DoWork += Worker_DoWork;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Worker.WorkerSupportsCancellation = true;
        }
    }
}
