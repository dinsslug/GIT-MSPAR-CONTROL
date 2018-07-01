using MsparControlDemo.Engine;
using MsparControlDemo.Model;
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
        private ObservableCollection<Data> dataList = new ObservableCollection<Data>();
        private double spinnerValue = 1E+100;
        public ObservableCollection<Data> DataList { get { return dataList; } set { dataList = value; OnPropertyChanged("DataList"); } }
        public double SpinnerValue { get { return spinnerValue; } set { spinnerValue = value; OnPropertyChanged("SpinnerValue"); } }

        public VMMainWindow()
        {
            DataList.Add(new Data("Sample1", "0"));
            DataList.Add(new Data("Sample2", "1"));
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
