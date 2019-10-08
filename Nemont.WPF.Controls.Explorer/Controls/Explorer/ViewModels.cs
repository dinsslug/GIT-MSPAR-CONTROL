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
using Nemont.WPF.Service;

namespace Nemont.WPF.Controls.Explorer
{
    public static class EvModel
    {
        public static EvBase ElementAt(EvFileFolder parent, string relativePath)
        {
            return _ElementAt(parent, relativePath);
        }

        private static EvBase _ElementAt(EvBase parent, string relativePath)
        {
            EvBase result = null;

            if (parent is IFile && (parent as IFile).RelativePath == relativePath) {
                return parent;
            }

            if (parent is EvFileFolder) {
                var tvFolder = parent as EvFileFolder;
                foreach (var item in tvFolder.Sub) {
                    result = _ElementAt(item, relativePath);
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
        protected string toolTip = "";
        protected FontWeight fontWeight = FontWeights.Normal;
        protected int status;
        protected bool isNodeSelected = false;
        protected Visibility visibility = Visibility.Visible;

        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public string ToolTip { get { return toolTip; } set { toolTip = value; OnPropertyChanged(nameof(ToolTip)); } }
        public FontWeight FontWeight { get { return fontWeight; } set { fontWeight = value; OnPropertyChanged(nameof(FontWeight)); } }
        public int Status { get { return status; } set { status = value; OnPropertyChanged(nameof(Status)); } }
        public bool IsNodeSelected { get { return isNodeSelected; } set { isNodeSelected = value; OnPropertyChanged(nameof(IsNodeSelected)); } }
        public Visibility Visibility { get { return visibility; } set { visibility = value; OnPropertyChanged(nameof(Visibility)); } }

        protected EvBase(string name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class EvItem : EvBase
    {
        public string IconUri { get; set; }
        public bool IsNodeExpanded { get; set; } = false;

        public EvItem(string name) : base(name)
        {
            IconUri = "pack://application:,,,/Nemont.WPF.Controls.Explorer;Component/Asset/icon_empty.png";
        }

        public EvItem(string name, string iconUri) : base(name)
        {
            IconUri = iconUri;
        }
    }

    public class EvFile : EvItem, IFile
    {
        public string RelativePath { get; set; }
        public virtual string DefaultIconUri => "pack://application:,,,/Nemont.WPF.Controls.Explorer;Component/Asset/icon_default.png";

        public EvFile(string fileName, string relativePath) : base(fileName)
        {
            RelativePath = relativePath;

            IconUri = DefaultIconUri;
        }

        public static EvBase Create(string fileName, string relativePath)
        {
            return new EvFile(fileName, relativePath);
        }
    }

    public class EvFolder : EvBase
    {
        // Icon
        public string IconClosedUri { get; set; }
        public string IconOpenedUri { get; set; }

        // Binding Variables
        protected ObservableCollection<EvBase> sub = new ObservableCollection<EvBase>();
        protected bool isNodeExpanded = false;
        public ObservableCollection<EvBase> Sub { get { return sub; } set { sub = value; OnPropertyChanged(nameof(Children)); } }
        public bool IsNodeExpanded { get { return isNodeExpanded; } set { isNodeExpanded = value; OnPropertyChanged(nameof(IsNodeExpanded)); } }

        public EvBase this[int index] { get { return Sub[index]; } set { Sub[index] = value; } }

        public IList Children => new CompositeCollection() { new CollectionContainer() { Collection = Sub } };

        public EvFolder(string name) : base(name) { }
    }

    public class EvFileFolder : EvFolder, IFile
    {
        public string RelativePath { get; set; }

        public EvFileFolder(string folderName, string relativePath) : base(folderName)
        {
            RelativePath = relativePath;

            IsNodeExpanded = true;
            IconClosedUri = "pack://application:,,,/Nemont.WPF.Controls.Explorer;Component/Asset/folder_close.png";
            IconOpenedUri = "pack://application:,,,/Nemont.WPF.Controls.Explorer;Component/Asset/folder_open.png";
        }
    }
}
