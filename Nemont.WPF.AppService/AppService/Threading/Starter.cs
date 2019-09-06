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
            var vMessage = new VMMessageDialog();
            var wMessage = new MessageDialog(vMessage, startInfo);
            var proc = new MessageTask(wMessage);

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
