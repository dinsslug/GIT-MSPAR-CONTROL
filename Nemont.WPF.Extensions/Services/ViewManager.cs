using Nemont.Explorer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Explorer
{
    public class ViewManager : INotifyPropertyChanged
    {
        // Binding Variables
        private bool autoRefresh = false;
        private ObservableCollection<EvBase> root = new ObservableCollection<EvBase>();
        public bool AutoRefresh { get { return autoRefresh; } set { autoRefresh = value; Watcher.EnableRaisingEvents = value; } }
        public ObservableCollection<EvBase> Root { get { return root; } set { root = value; OnPropertyChanged("Root"); } }

        // Local Variables
        public string Path { get; set; }
        private string filter = "*";
        private Dictionary<string, FilterInfo> Filter = new Dictionary<string, FilterInfo>();
        private FileSystemWatcher Watcher;

        public ViewManager() { }
        public ViewManager(string path, bool autoRefresh)
        {
            Path = path;
            Filter.Add("?", new FilterInfo(typeof(EvItem), -2));
            Filter.Add("//", new FilterInfo(typeof(EvFileFolder), -1));
            Filter.Add("*", new FilterInfo(typeof(EvFile), 0));
            Root.Add(new EvFileFolder(Path, ""));
            Watcher = new FileSystemWatcher();
            Watcher.Path = Path;
            Watcher.Filter = "*.*";
            Watcher.IncludeSubdirectories = true;
            // Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Watcher.Changed += new FileSystemEventHandler(OnChanged);
            Watcher.Created += new FileSystemEventHandler(OnChanged);
            Watcher.Deleted += new FileSystemEventHandler(OnChanged);
            Watcher.Renamed += new RenamedEventHandler(OnRenamed);
            AutoRefresh = autoRefresh;
        }

        public void AddFilter(Type type, string extension)
        {
            try {
                Filter.Add(extension, new FilterInfo(type, Filter.Count - 2));
                Filter["*"].Order = Filter.Count - 2;
            }
            catch {
                throw;
            }
        }

        public void Filtering(FilterMode filterMode)
        {
            if (filterMode == FilterMode.FolderOnly) {
                filter = "\\";
                RefreshDirectory();
            }
            else if (filterMode == FilterMode.All) {
                filter = "*";
                RefreshDirectory();
            }
        }

        public void Filtering(string fileExtension)
        {
            if (fileExtension != "\\" && fileExtension != "*" && Filter.ContainsKey(fileExtension) == true) {
                filter = fileExtension;
                RefreshDirectory();
            }
        }

        public void RefreshDirectory()
        {
            RefreshRecur(Root[0] as EvFileFolder);
        }

        private void RefreshRecur(EvFileFolder parent)
        {
            var absPath = Path + "\\" + parent.RelativePath;
            var dirInfo = new DirectoryInfo(absPath);

            var fileSystems = dirInfo.GetFileSystemInfos();
            var existChk = new bool[fileSystems.Length];

            for (int i = 0; i < parent.Sub.Count; i++) {
                var item = parent.Sub[i];
                if (!(item is IFile)) {
                    continue;
                }
                var find = Array.FindIndex(fileSystems, j => j.Name == item.Name);
                if (find == -1) {
                    parent.Sub.Remove(parent.Sub[i]);
                    i--;
                }
                else {
                    if (item is EvFileFolder) {
                        RefreshRecur(parent.Sub[i] as EvFileFolder);
                    }
                    existChk[find] = true;
                }
            }

            /// Add item if a file don't exist in the treeview
            for (int i = 0; i < existChk.Length; i++) {
                if (existChk[i] == false) {
                    if (Directory.Exists(fileSystems[i].FullName)) {
                        var addFolder = new EvFileFolder(fileSystems[i].Name, parent.RelativePath + "\\" + fileSystems[i].Name);
                        RefreshRecur(addFolder);
                        parent.Sub.Add(addFolder);
                    }
                    else {
                        EvFile addItem;
                        var name = fileSystems[i].Name;
                        var path = parent.RelativePath + "\\" + fileSystems[i].Name;
                        var ext = System.IO.Path.GetExtension(fileSystems[i].Name);
                        var fil = Filter.ContainsKey(ext);
                        if (fil == true) {
                            addItem = Activator.CreateInstance(Filter[ext].Type, name, path) as EvFile;
                        }
                        else {
                            addItem = new EvFile(name, path);
                        }
                        parent.Sub.Add(addItem);
                    }
                }
            }

            /// Filtering
            for (int i = 0; i < parent.Sub.Count; i++) {
                var item = parent.Sub[i];
                if (item is EvFile && (filter == "\\" || (filter != "*" && filter != System.IO.Path.GetExtension(item.Name)))) {
                    parent.Sub.Remove(parent.Sub[i]);
                    i--;
                }
            }

            /// Sort
            parent.Sub = new ObservableCollection<EvBase>(parent.Sub.OrderBy(item => {
                if (item is EvFileFolder) {
                    return Filter["//"].Order;
                }
                else if (item is EvFile) {
                    var ext = System.IO.Path.GetExtension((item as EvFile).Name);
                    var fil = Filter.ContainsKey(ext);
                    if (fil == false) {
                        return Filter["*"].Order;
                    }
                    else {
                        return Filter[ext].Order;
                    }
                }
                return Filter["?"].Order;
            }));
        }

        private string GetRelativePath(string basePath, string absolutePath)
        {
            if (basePath == absolutePath) {
                return "";
            }
            var result = absolutePath.Substring(basePath.Length + 1);

            return result;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                RefreshDirectory();
            });
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                RefreshDirectory();
            });
        }

        internal void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
