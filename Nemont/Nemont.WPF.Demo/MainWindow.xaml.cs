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

namespace MsparControlDemo
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
            VMExplorerView.Root.Add(file);
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
