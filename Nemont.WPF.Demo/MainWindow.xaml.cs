using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Threading;
using Nemont.Demo.Model;
using Nemont.Demo.Services;
using Nemont.WPF.AppService;
using Nemont.WPF.AppService.Threading;
using Nemont.WPF.Controls;
using Nemont.WPF.Controls.Explorer;

namespace Nemont.Demo
{
    public class VMMainWindow : ModelBase
    {
        private ObservableCollection<Data> dataList = new ObservableCollection<Data>();
        private double spinnerValue = 5;
        private string textThread = "";
        public ObservableCollection<Data> DataList { get { return dataList; } set { dataList = value; OnPropertyChanged("DataList"); } }
        public double SpinnerValue { get { return spinnerValue; } set { spinnerValue = value; OnPropertyChanged("SpinnerValue"); } }
        public string TextThread { get { return textThread; } set { textThread = value; OnPropertyChanged(nameof(TextThread)); } }
        public FileExplorerManager ViewManager { get; set; }
        public RelayCommand RcStop { get; }
        public RelayCommand RcTest { get; }
        public RelayCommand RcProgress1 { get; }
        public RelayCommand RcProgress2 { get; }
        public RelayCommand RcMessage1 { get; }
        public RelayCommand RcMessage2 { get; }

        private int idxTab;
        public int IdxTab { get { return idxTab; } set { idxTab = value; OnPropertyChanged(nameof(IdxTab)); } }

        public MainWindow C;

        public LogFactory TextLog;
        public MessageTask TextTask => TextLog.Task;

        public VMMainWindow()
        {
            RcStop = new RelayCommand(OnStop);
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

            //InitializeExplorer();

            App.Log = new LogDialogFactory();

            TextLog = new LogFactory();
            TextLog.OnLogChanged += TextLog_OnLogChanged;
        }

        private void TextLog_OnLogChanged(string log)
        {
            TextThread = log;
        }

        private void OnStop()
        {
            TextLog.StopTask();
        }

        public void InitializeExplorer()
        {
            ViewManager = new FileExplorerManager("E:\\HNC", true);
            ViewManager.ExceptExtensions.Add(".txt");
            ViewManager.SetFilter(typeof(EvFile), ".exe");
            ViewManager.SetFilter(typeof(EvText), ".txt2");
            ViewManager.SetFilter(typeof(EvText), ".txt3");
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
            ViewManager.OnRightClick = OnRightClick;
        }

        public void OnDoubleClick(EvBase item, TreeViewItem tItem, ExplorerView view, MouseButtonEventArgs e)
        {
            MessageBox.Show("Double Clicked");
        }

        public void OnRightClick(EvBase item, TreeViewItem tItem, ExplorerView view, MouseButtonEventArgs e)
        {
            tItem.ContextMenu = new ContextMenu();

            tItem.ContextMenu.Items.Add(new MenuItem() { Header = item.Name, });
        }

        public void OnTest()
        {
            App.Log.UpdateIntervalTime = 100;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 500; i++) {
                Thread.Sleep(10);

                App.Log.ReplaceLine(string.Format("Line {0}", i + 1));
            }
            sw.Stop();
            App.Log.WriteLine("RUN TIME : " + sw.ElapsedMilliseconds);

            App.Log.Flush();
            //ViewManager.Filtering(FilterMode.FolderOnly);
        }

        private void OnProgress1()
        {
            App.Log.UpdateIntervalTime = 50;
            App.Log.InitializeProgress();

            for (int i = 0; i < 20; i++) {
                Thread.Sleep(100);

                if (i > 10) {
                    //proc.IsWarning = true;
                }
                if (i > 30) {
                    //throw new Exception("ASDFASDF");
                }

                App.Log.ReplaceLine(string.Format("Line {0}", i + 1));
            }
            App.Log.WriteLine("Complete!");

            App.Log.WriteLine("Run Progress...");
            App.Log.InitializeProgress();
            for (int i = 0; i < 200; i++) {
                Thread.Sleep(10);

                var p = (i + 1) / 200.0;
                App.Log.SetProgressText(string.Format("Progress {0:0.0}%", p * 100.0));
                App.Log.SetProgressValue(p);
            }
            App.Log.CompleteProgress();

            for (int i = 0; i < 20; i++) {
                Thread.Sleep(100);

                App.Log.WriteLine(string.Format("Line {0}", i + 1));
            }

            App.Log.WriteLine("Run Progress...");
            App.Log.InitializeProgress();
            for (int i = 0; i < 20; i++) {
                Thread.Sleep(100);

                var p = (i + 1) / 20.0;
                App.Log.SetProgressText(string.Format("Progress {0:0.0}%", p * 100.0));
                App.Log.SetProgressValue(p);
            }
            App.Log.CompleteProgress();

            App.Log.Flush();
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
            App.Log.SetDialogInfo(startInfo);
            App.Log.RunTask(Message1);
        }

        private void OnMessage2()
        {
            TextLog.RunTask(Message2);
        }

        private void Progress1()
        {
        }

        private void Progress2()
        {
        }

        private void Message1()
        {
            App.Log.UpdateIntervalTime = 10;
            for (int i = 0; i < 30; i++, App.Task.ThrowIfCancellationRequested()) {
                Thread.Sleep(100);

                if (i > 10) {
                    App.Task.IsWarning = false;
                }
                if (i > 25) {
                    throw new Exception("ASDFASDF");
                }

                App.Log.ReplaceLine(string.Format("Line {0}", i + 1));
            }
            App.Log.WriteLine("Complete!");

            App.Log.WriteLine("Run Progress...");
            App.Log.InitializeProgress();
            for (int i = 0; i < 200; i++, App.Task.ThrowIfCancellationRequested()) {
                Thread.Sleep(10);

                var p = (i + 1) / 200.0;
                App.Log.SetProgressText(string.Format("Progress {0:0.0}%", p * 100.0));
                App.Log.SetProgressValue(p);
            }
            App.Log.CompleteProgress();

            for (int i = 0; i < 20; i++, App.Task.ThrowIfCancellationRequested()) {
                Thread.Sleep(100);

                App.Log.WriteLine(string.Format("Line {0}", i + 1));
            }

            App.Log.WriteLine("Run Progress...");
            App.Log.InitializeProgress();
            for (int i = 0; i < 20; i++, App.Task.ThrowIfCancellationRequested()) {
                Thread.Sleep(100);

                var p = (i + 1) / 20.0;
                App.Log.SetProgressText(string.Format("Progress {0:0.0}%", p * 100.0));
                App.Log.SetProgressValue(p);
            }
            App.Log.CompleteProgress();

            if (App.Task.IsWarning == true) {
                App.Log.WriteLine("Warning 발생!!!!!");
            }
        }

        private void Message2()
        {
            TextTask.InitializeProcess();

            var startInfo = TextTask.Process.StartInfo;
            startInfo.FileName = "Exec\\TestProgram1.exe";
            startInfo.WorkingDirectory = "Exec";
            startInfo.Arguments = "";
            TextTask.Process.Start();
            TextTask.Process.BeginOutputReadLine();
            TextTask.Process.BeginErrorReadLine();
            TextTask.Process.WaitForExit();

            for (int i = 0; i < 20; i++) {
                TextTask.ThrowIfCancellationRequested();
                Thread.Sleep(100);

                TextLog.WriteLine(string.Format("Line {0}", i + 1));
            }

            TextTask.InitializeProcess();

            startInfo = TextTask.Process.StartInfo;
            startInfo.FileName = "Exec\\TestProgram1.exe";
            startInfo.WorkingDirectory = "Exec";
            startInfo.Arguments = "";
            TextTask.Process.Start();
            TextTask.Process.BeginOutputReadLine();
            TextTask.Process.BeginErrorReadLine();
            TextTask.Process.WaitForExit();
        }
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public VMMainWindow ViewModel => DataContext as VMMainWindow;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new VMMainWindow();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Log.CloseDialog();

            ViewModel.C = this;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox.CaretIndex = TextBox.Text.Length;
            TextBox.ScrollToEnd();
        }
    }
}
