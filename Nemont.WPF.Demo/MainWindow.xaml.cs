using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Nemont.Demo.Model;
using Nemont.Demo.Services;
using Nemont.WPF.AppService.Threading;
using Nemont.WPF.Controls;
using Nemont.WPF.Controls.Explorer;

namespace Nemont.Demo
{
    public class VMMainWindow : ModelBase
    {
        private ObservableCollection<Data> dataList = new ObservableCollection<Data>();
        private double spinnerValue = 5;
        public ObservableCollection<Data> DataList { get { return dataList; } set { dataList = value; OnPropertyChanged("DataList"); } }
        public double SpinnerValue { get { return spinnerValue; } set { spinnerValue = value; OnPropertyChanged("SpinnerValue"); } }
        public ViewManager ViewManager { get; set; }
        public RelayCommand RcTest { get; set; }
        public RelayCommand RcProgress1 { get; }
        public RelayCommand RcProgress2 { get; }
        public RelayCommand RcMessage1 { get; }
        public RelayCommand RcMessage2 { get; }

        public VMMainWindow()
        {
            RcTest = new RelayCommand(OnTest);
            RcProgress1 = new RelayCommand(OnProgress1);
            RcProgress2 = new RelayCommand(OnProgress2);
            RcMessage1 = new RelayCommand(OnMessage1);
            RcMessage2 = new RelayCommand(OnMessage2);

            DataList.Add(new Data("Sample2", "3", "ZZZ"));
            DataList.Add(new Data("Sample1", "3", "AAA"));
            DataList.Add(new Data("Sample2", "5", "DDD"));
            DataList.Add(new Data("Sample1", "0", "EEE"));
            DataList.Add(new Data("Sample3", "3", "EEE"));

            InitializeExplorer();
        }

        public void InitializeExplorer()
        {
            ViewManager = new ViewManager("E:\\HNC", true);
            ViewManager.ExceptExtensions.Add(".txt");
            ViewManager.AddFilter(typeof(EvText), ".txt1");
            ViewManager.AddFilter(typeof(EvText), ".txt2");
            ViewManager.AddFilter(typeof(EvText), ".txt3");
            StatusManager.Add((int)StatusMode.None, "");
            StatusManager.Add((int)StatusMode.Checked, "pack://application:,,,/Nemont.WPF.Demo;component/Asset/status_check.png");
            StatusManager.Add((int)StatusMode.Undefined, "pack://application:,,,/Nemont.WPF.Demo;component/Asset/status_undefine.png");
            ViewManager.RefreshDirectory();
            var item1 = new EvItem("Item 1", "pack://application:,,,/Nemont.WPF.Demo;component/Asset/polyline.png");
            var item2 = new EvItem("Item 2", "pack://application:,,,/Nemont.WPF.Demo;component/Asset/polyline.png");
            var folder1 = new EvFolder("Custom Folder");
            item1.Status = (int)StatusMode.Checked;
            folder1.IconOpenedUri = "pack://application:,,,/Nemont.WPF.Demo;component/Asset/polygon.png";
            folder1.IconClosedUri = "pack://application:,,,/Nemont.WPF.Demo;component/Asset/polygon.png";
            folder1.Sub.Add(item1);
            folder1.Sub.Add(item2);
            (ViewManager.Root[0] as EvFolder).Sub.Insert(0, folder1);

            ViewManager.Filtering(FilterMode.All);

            ViewManager.OnDoubleClick = OnDoubleClick;
            ViewManager.OnRightClick = OnRighteClick;
        }

        public void OnDoubleClick(EvBase item, TreeViewItem tItem, ExplorerView view, MouseButtonEventArgs e)
        {
            MessageBox.Show("Double Clicked");
        }

        public void OnRighteClick(EvBase item, TreeViewItem tItem, ExplorerView view, MouseButtonEventArgs e)
        {
            tItem.ContextMenu = new ContextMenu();

            tItem.ContextMenu.Items.Add(new MenuItem() { Header = item.Name, });
        }

        public void OnTest()
        {
            ViewManager.Filtering(FilterMode.FolderOnly);
        }

        private void OnProgress1()
        {
            //ProgressDialog.Run(Progress1);
        }

        private void OnProgress2()
        {
            //ProgressDialog.Run(Progress2, true, true);
        }

        private void OnMessage1()
        {
            var startInfo = new StartInfo() {
                Owner = Application.Current.MainWindow,
                IsDialog = false,
                Title = "Thread 1",
                ShowInTaskBar = false,
            };
            Starter.RunMessage(Message1, startInfo);
        }

        private void OnMessage2()
        {
            Starter.RunMessage(Message2);
        }

        private void Progress1()
        {
        }

        private void Progress2()
        {
        }

        private void Message1(MessageTask proc)
        {
            for (int i = 0; i < 20; i++, proc.ThrowIfCancellationRequested()) {
                Thread.Sleep(100);

                if (i > 10) {
                    //proc.IsWarning = true;
                }
                if (i > 30) {
                    //throw new Exception("ASDFASDF");
                }

                proc.ReplaceLine(string.Format("Line {0}", i + 1));
            }
            proc.WriteLine("Complete!");

            proc.WriteLine("Run Progress...");
            proc.InitializeProgress();
            for (int i = 0; i < 200; i++, proc.ThrowIfCancellationRequested()) {
                Thread.Sleep(10);

                var p = (i + 1) / 200.0;
                proc.SetProgressText(string.Format("Progress {0:0.0}%", p * 100.0));
                proc.SetProgressValue(p);
            }
            proc.CompleteProgress();

            for (int i = 0; i < 20; i++, proc.ThrowIfCancellationRequested()) {
                Thread.Sleep(100);

                proc.WriteLine(string.Format("Line {0}", i + 1));
            }

            proc.WriteLine("Run Progress...");
            proc.InitializeProgress();
            for (int i = 0; i < 20; i++, proc.ThrowIfCancellationRequested()) {
                Thread.Sleep(100);

                var p = (i + 1) / 20.0;
                proc.SetProgressText(string.Format("Progress {0:0.0}%", p * 100.0));
                proc.SetProgressValue(p);
            }
            proc.CompleteProgress();

            if (proc.IsWarning == true) {
                proc.WriteLine("Warning 발생!!!!!");
            }
        }

        private void Message2(MessageTask proc)
        {
            proc.InitializeProcess();

            var startInfo = proc.Process.StartInfo;
            startInfo.FileName = "Exec\\TestProgram1.exe";
            startInfo.WorkingDirectory = "Exec";
            startInfo.Arguments = "";
            proc.Process.Start();
            proc.Process.BeginOutputReadLine();
            proc.Process.BeginErrorReadLine();
            proc.Process.WaitForExit();

            for (int i = 0; i < 20; i++) {
                proc.ThrowIfCancellationRequested();
                Thread.Sleep(100);

                proc.WriteLine(string.Format("Line {0}", i + 1));
            }

            proc.InitializeProcess();

            startInfo = proc.Process.StartInfo;
            startInfo.FileName = "Exec\\TestProgram1.exe";
            startInfo.WorkingDirectory = "Exec";
            startInfo.Arguments = "";
            proc.Process.Start();
            proc.Process.BeginOutputReadLine();
            proc.Process.BeginErrorReadLine();
            proc.Process.WaitForExit();
        }
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new VMMainWindow();
        }
    }
}
