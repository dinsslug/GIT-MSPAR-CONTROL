using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Nemont.Model.ExplorerView
{
    public class Folder : Base
    {
        // Binding Variables
        protected ObservableCollection<Base> sub = new ObservableCollection<Base>();
        protected bool isNodeExpanded = true;
        public ObservableCollection<Base> Sub { get { return sub; } set { sub = value; OnPropertyChanged("Children"); } }
        public bool IsNodeExpanded { get { return isNodeExpanded; } set { isNodeExpanded = value; OnPropertyChanged("IsNodeExpanded"); } }

        public Folder(string name) : base(name) { }

        public IList Children { get { return new CompositeCollection() { new CollectionContainer() { Collection = Sub } }; } }
    }
}
