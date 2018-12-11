using Nemont.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Model.ExplorerView
{
    public class VMExplorerView : ModelBase
    {
        private ObservableCollection<Base> root = new ObservableCollection<Base>();
        public ObservableCollection<Base> Root { get { return root; } set { root = value; OnPropertyChanged("Root"); } }
    }
}
