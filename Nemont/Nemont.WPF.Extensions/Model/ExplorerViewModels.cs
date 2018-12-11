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
    public abstract class Base : INotifyPropertyChanged
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

        protected Base(string name)
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

    public class Item : Base
    {
        protected string iconUri;
        public BitmapImage Icon { get { return new BitmapImage(new Uri(iconUri)); } }

        public Item(string name, string iconUri) : base(name)
        {
            this.iconUri = iconUri;
        }
    }

    public class File : Item
    {
        public string RelPath;

        public File(string name, string relPath) : base(name, "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/file.png")
        {
            RelPath = relPath;
        }
    }

    public class Folder : Base
    {
        // Binding Variables
        protected ObservableCollection<Base> sub = new ObservableCollection<Base>();
        protected bool isNodeExpanded = true;
        public ObservableCollection<Base> Sub { get { return sub; } set { sub = value; OnPropertyChanged("Children"); } }
        public bool IsNodeExpanded { get { return isNodeExpanded; } set { isNodeExpanded = value; OnPropertyChanged("IsNodeExpanded"); } }

        // Icon
        public string IconClosedUri { get; set; } = "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/folder_close.png";
        public string IconOpenedUri { get; set; } = "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/folder_open.png";
        public BitmapImage IconClosed { get { return new BitmapImage(new Uri(IconClosedUri)); } }
        public BitmapImage IconOpened { get { return new BitmapImage(new Uri(IconOpenedUri)); } }

        public IList Children { get { return new CompositeCollection() { new CollectionContainer() { Collection = Sub } }; } }

        public Folder(string name) : base(name) { }
    }

    public class FileFolder : Folder
    {
        public string RelPath;

        public FileFolder(string name, string relPath) : base(name)
        {
            RelPath = relPath;
        }
    }
}
