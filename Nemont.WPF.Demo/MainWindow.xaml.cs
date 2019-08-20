using Nemont.Demo.Model;
using Nemont.Demo.Services;
using Nemont.WPF.AppService;
using Nemont.WPF.AppService.Progress;
using Nemont.WPF.Controls.Explorer;
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

        public VMMainWindow()
        {
            RcTest = new RelayCommand(OnTest);
            RcProgress1 = new RelayCommand(OnProgress1);
            RcProgress2 = new RelayCommand(OnProgress2);
            RcMessage1 = new RelayCommand(OnMessage1);

            DataList.Add(new Data("Sample2", "3", "ZZZ"));
            DataList.Add(new Data("Sample1", "3", "AAA"));
            DataList.Add(new Data("Sample2", "5", "DDD"));
            DataList.Add(new Data("Sample1", "0", "EEE"));
            DataList.Add(new Data("Sample3", "3", "EEE"));

            InitializeExplorer();

            MessageDialog.DefaultWidth = 600;
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
        }

        public void OnTest()
        {
            ViewManager.Filtering(FilterMode.FolderOnly);
        }

        private void OnProgress1()
        {
            ProgressDialog.Run(Progress1);
        }

        private void OnProgress2()
        {
            ProgressDialog.Run(Progress2, true, true);
        }

        private void OnMessage1()
        {
            MessageDialog.Run(Message1);
        }

        private void Progress1()
        {
            ProgressManager.Message = "Running Thread ...";

            for (int i = 0; i < 50; i++) {
                Thread.Sleep(100);
            }
        }

        private void Progress2()
        {
            ProgressManager.Message = "Running Thread ...";

            for (int i = 0; i < 1000; i++) {
                ProgressManager.CancellationToken.ThrowIfCancellationRequested();
                Thread.Sleep(10);

                ProgressManager.ProgressValue = i / 1000.0;
                ProgressManager.ProgressText = string.Format("{0}", i);
            }
        }

        private void Message1()
        {
            SystemLog.Clear();

            for (int i = 0; i < 20; i++) {
                SystemLog.CancellationToken.ThrowIfCancellationRequested();
                Thread.Sleep(100);

                if (i > 10) {
                    throw new Exception("ASDFASDF");
                }

                SystemLog.WriteLine(string.Format("Line {0}", i + 1));
            }
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

        private void ExplorerView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null) {
                MessageBox.Show("Right click");
            }
        }

        private void ExplorerView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null) {

            }
        }

        private TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem)) {
                source = VisualTreeHelper.GetParent(source);
            }
            return source as TreeViewItem;
        }
    }
}
