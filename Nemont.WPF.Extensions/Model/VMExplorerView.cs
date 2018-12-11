using Nemont.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Model.ExplorerView
{
    public class VMExplorerView : ModelBase
    {
        private ObservableCollection<EvBase> root = new ObservableCollection<EvBase>();
        public ObservableCollection<EvBase> Root { get { return root; } set { root = value; OnPropertyChanged("Root"); } }
        public FileSystemWatcher Watcher;

        public VMExplorerView()
        {
            Watcher = new FileSystemWatcher();
            Watcher.Path = "F:\\TEST";
            Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            Watcher.Filter = "*.*";
            Watcher.Changed += new FileSystemEventHandler(OnChanged);
            Watcher.Created += new FileSystemEventHandler(OnChanged);
            Watcher.Deleted += new FileSystemEventHandler(OnChanged);
            Watcher.Renamed += new RenamedEventHandler(OnRenamed);
            Watcher.EnableRaisingEvents = true;

            RefreshDirectory();
        }

        public void RefreshDirectory()
        {
            Root.Add(RefreshRecur(Watcher.Path, ""));
        }

        public EvFileFolder RefreshRecur(string name, string relPath)
        {
            var absPath = Watcher.Path + "\\" + relPath;
            var dir = new EvFileFolder(name, relPath);
            var dirInfo = new DirectoryInfo(absPath);

            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();

            dir.Sub = new ObservableCollection<EvBase>();
            foreach (var d in directories) {
                var addItem = RefreshRecur(d.Name, GetRelativePath(Watcher.Path, d.FullName));
                dir.Sub.Add(addItem);
            }
            foreach (var f in files) {
                dir.Sub.Add(new EvFile(f.Name, GetRelativePath(Watcher.Path, f.FullName)));
            }
            return dir;
        }

        public string GetRelativePath(string basePath, string absolutePath)
        {
            if (basePath == absolutePath) {
                return "";
            }
            var result = absolutePath.Substring(basePath.Length + 1);

            return result;
        }

        public EvBase ElementAt(string relPath)
        {
            return _ElementAt(Root[0], relPath);
        }

        private EvBase _ElementAt(EvBase parent, string relPath)
        {
            EvBase result = null;

            if (parent is EvFile && (parent as EvFile).RelPath == relPath) return parent;
            if (parent is EvFileFolder && (parent as EvFileFolder).RelPath == relPath) return parent;

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

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var fileInfo = new FileInfo(e.FullPath);
                var dir = Directory.GetParent(e.FullPath);
                var parent = ElementAt(GetRelativePath(Watcher.Path, dir.FullName)) as EvFileFolder;
                var idxTarget = parent.Sub.ToList().FindIndex((EvBase item) => {
                    return (Watcher.Path + "\\" + (item as IFile).RelPath) == e.FullPath;
                });

                if (e.ChangeType == WatcherChangeTypes.Deleted) {
                    parent.Sub.RemoveAt(idxTarget);
                }
                else if (e.ChangeType == WatcherChangeTypes.Created) {
                    parent.Sub.Add(new EvFile(e.Name, GetRelativePath(Watcher.Path, e.FullPath)));
                }
                else if (e.ChangeType == WatcherChangeTypes.Changed) {
                }
            });
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var fileInfo = new FileInfo(e.OldFullPath);
                var dir = Directory.GetParent(e.OldFullPath);
                var parent = ElementAt(GetRelativePath(Watcher.Path, dir.FullName)) as EvFileFolder;
                var target = parent.Sub.ToList().Find((EvBase item) => {
                    return (Watcher.Path + "\\" + (item as IFile).RelPath) == e.OldFullPath;
                });

                target.Name = e.Name;
                (target as IFile).RelPath = GetRelativePath(Watcher.Path, e.FullPath);
            });
        }
    }
}
