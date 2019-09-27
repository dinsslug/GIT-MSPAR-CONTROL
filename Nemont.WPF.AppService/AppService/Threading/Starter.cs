using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.WPF.AppService.Threading
{
    public static class Starter
    {
        public static void RunMessage(Action<MessageTask> method)
        {
            RunMessage(method, new StartInfo());
        }

        public static void RunMessage(Action<MessageTask> method, StartInfo startInfo)
        {
            var vMessage = new DMessageDialog();
            var wMessage = new MessageDialog(vMessage);
            var proc = new MessageTask();

            proc.WorkerAction = new Action(() => {
                try {
                    proc.Process.Kill();
                }
                catch {
                    Debug.WriteLine("Failed to kill process.");
                }

                method(proc);
            });
            proc.Worker.RunWorkerAsync();
            
            if (startInfo.IsDialog == true) {
                wMessage.ShowDialog();
            }
            else {
                wMessage.Show();
            }
        }
    }
}
