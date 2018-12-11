using Nemont.Engine;
using Nemont.Enumerables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Nemont.Model.ExplorerView
{
    public abstract class EvBase : INotifyPropertyChanged
    {
        // Binding Variables
        protected string name = "";
        protected FontWeight selectedFontWeight = FontWeights.Normal;
        protected CheckMode checkMode = CheckMode.Empty;
        protected bool isNodeSelected = false;
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public FontWeight SelectedFontWeight { get { return selectedFontWeight; } set { selectedFontWeight = value; OnPropertyChanged("SelectedFontWeight"); } }
        public CheckMode CheckMode { get { return checkMode; } set { checkMode = value; OnPropertyChanged("CheckMode"); } }
        public bool IsNodeSelected { get { return isNodeSelected; } set { isNodeSelected = value; OnPropertyChanged("IsNodeSelected"); } }
        public Thickness ContentMargin { get; set; } = new Thickness(0.0);

        public RelayCommand RcClick { get; set; }
        public RelayCommand RcRightClick { get; set; }

        public Action Click;
        public Action RightClick;

        private void OnClick()
        {
            Click?.Invoke();
        }

        private void OnRightClick()
        {
            RightClick?.Invoke();
        }

        protected EvBase(string name)
        {
            Name = name;

            RcClick = new RelayCommand(OnClick);
            RcRightClick = new RelayCommand(OnRightClick);
        }

        internal void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class EvItem : EvBase
    {
        public string IconUri { get; set; }

        public EvItem(string name, string iconUri) : base(name)
        {
            this.IconUri = iconUri;
        }
    }

    public class EvFile : EvItem, IFile
    {
        public string RelPath { get; set; }
        public int TypeNumber = 0;

        public EvFile(string name, string relPath) : base(name, "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/file.png")
        {
            RelPath = relPath;
        }
    }

    public class EvFolder : EvBase
    {
        // Binding Variables
        protected ObservableCollection<EvBase> sub = new ObservableCollection<EvBase>();
        protected bool isNodeExpanded = false;
        public ObservableCollection<EvBase> Sub { get { return sub; } set { sub = value; OnPropertyChanged("Children"); } }
        public bool IsNodeExpanded { get { return isNodeExpanded; } set { isNodeExpanded = value; OnPropertyChanged("IsNodeExpanded"); } }

        // Icon
        public string IconClosedUri { get; set; }
        public string IconOpenedUri { get; set; }

        public IList Children { get { return new CompositeCollection() { new CollectionContainer() { Collection = Sub } }; } }

        public EvFolder(string name) : base(name) { }
    }

    public class EvFileFolder : EvFolder, IFile
    {
        public string RelPath { get; set; }

        public EvFileFolder(string name, string relPath) : base(name)
        {
            RelPath = relPath;

            IconClosedUri = "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/folder_close.png";
            IconOpenedUri = "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/folder_open.png";
        }
    }
}
