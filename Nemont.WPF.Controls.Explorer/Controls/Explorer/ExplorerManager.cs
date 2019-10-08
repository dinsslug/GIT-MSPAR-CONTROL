using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nemont.WPF.Controls.Explorer
{
    public class ExplorerManager : INotifyPropertyChanged
    {
        protected ObservableCollection<EvBase> root = new ObservableCollection<EvBase>();
        public ObservableCollection<EvBase> Root { get { return root; } set { root = value; OnPropertyChanged(nameof(Root)); } }

        public Action<EvBase, TreeViewItem, ExplorerView, MouseButtonEventArgs> OnDoubleClick;
        public Action<EvBase, TreeViewItem, ExplorerView, MouseButtonEventArgs> OnRightClick;

        public ExplorerManager() { }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
