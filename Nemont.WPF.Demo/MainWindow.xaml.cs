using Nemont.Demo.Model;
using Nemont.Demo.Services;
using Nemont.Explorer;
using Nemont.Explorer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        public VMMainWindow()
        {
            RcTest = new RelayCommand(OnTest);

            DataList.Add(new Data("Sample1", "0", "ZZZ"));
            DataList.Add(new Data("Sample2", "2", "AAA"));
            DataList.Add(new Data("Sample3", "5", "DDD"));
            DataList.Add(new Data("Sample4", "3", "EEE"));

            ViewManager = new ViewManager("F:\\TEST", true);
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

        public void OnTest(object param)
        {
            ViewManager.Filtering(FilterMode.FolderOnly);
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
