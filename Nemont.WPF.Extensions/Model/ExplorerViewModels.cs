using Nemont.Explorer;
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

namespace Nemont.Explorer.Model
{
    public static class EvModel
    {
        public static EvBase ElementAt(EvFileFolder parent, string relPath)
        {
            return _ElementAt(parent, relPath);
        }

        private static EvBase _ElementAt(EvBase parent, string relPath)
        {
            EvBase result = null;

            if (parent is IFile && (parent as IFile).RelativePath == relPath) {
                return parent;
            }

            if (parent is EvFileFolder) {
                var tvFolder = parent as EvFileFolder;
                foreach (var item in tvFolder.Sub) {
                    result = _ElementAt(item, relPath);
                    if (result != null) {
                        return result;
                    }
                }
            }
            return null;
        }
    }

    public abstract class EvBase : INotifyPropertyChanged
    {
        // Binding Variables
        protected string name = "";
        protected FontWeight fontWeight = FontWeights.Normal;
        protected int status;
        protected bool isNodeSelected = false;
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public FontWeight FontWeight { get { return fontWeight; } set { fontWeight = value; OnPropertyChanged("FontWeight"); } }
        public int Status { get { return status; } set { status = value; OnPropertyChanged("Status"); } }
        public bool IsNodeSelected { get { return isNodeSelected; } set { isNodeSelected = value; OnPropertyChanged("IsNodeSelected"); } }
        public Thickness ContentMargin { get; set; } = new Thickness(0.0);

        internal RelayCommand RcClick { get; set; }
        internal RelayCommand RcRightClick { get; set; }

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
            IconUri = iconUri;
        }
    }

    public class EvFile : EvItem, IFile
    {
        public string RelativePath { get; set; }

        public EvFile(string name, string relativePath) : base(name, "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/icon_default.png")
        {
            RelativePath = relativePath;
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
        public string RelativePath { get; set; }

        public EvFileFolder(string name, string relativePath) : base(name)
        {
            RelativePath = relativePath;

            IconClosedUri = "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/folder_close.png";
            IconOpenedUri = "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/folder_open.png";
        }
    }
}
