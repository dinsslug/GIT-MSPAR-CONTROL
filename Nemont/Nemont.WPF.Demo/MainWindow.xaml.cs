using MsparControlDemo.Engine;
using MsparControlDemo.Model;
using Nemont.Model.ExplorerView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public VMExplorerView VMExplorerView { get; set; } = new VMExplorerView();
        private ObservableCollection<Data> dataList = new ObservableCollection<Data>();
        private double spinnerValue = 5;
        public ObservableCollection<Data> DataList { get { return dataList; } set { dataList = value; OnPropertyChanged("DataList"); } }
        public double SpinnerValue { get { return spinnerValue; } set { spinnerValue = value; OnPropertyChanged("SpinnerValue"); } }

        public VMMainWindow()
        {
            DataList.Add(new Data("Sample1", "0", "ZZZ"));
            DataList.Add(new Data("Sample2", "2", "AAA"));
            DataList.Add(new Data("Sample3", "5", "DDD"));
            DataList.Add(new Data("Sample4", "3", "EEE"));

            VMExplorerView.Root = new ObservableCollection<Base>();
            var file = new File("ASDF", "Directory");
            file.Click = () => { MessageBox.Show("ASDFASDF"); };
            file.RightClick = () => { MessageBox.Show("ASDF#@"); };
            file.CheckMode = Enumerables.CheckMode.Checked;
            ///VMExplorerView.Root.Add(file);

            var file2 = new File("ASDF", "Directory");
            file2.Click = () => { MessageBox.Show("AS34DF"); };
            file2.RightClick = () => { MessageBox.Show("AS34@"); };
            file2.CheckMode = Enumerables.CheckMode.Undefined;
            var file3 = new File("FF", "");

            var folder2 = new FileFolder("SDF", "");

            var folder = new FileFolder("DFDF", "Dsaf");
            folder.Sub.Add(folder2);
            folder2.Sub.Add(file3);
            folder.Sub.Add(file);
            folder.Sub.Add(file2);

            folder.RightClick = () => { MessageBox.Show("ASDF#@"); };
            VMExplorerView.Root.Add(folder);
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
