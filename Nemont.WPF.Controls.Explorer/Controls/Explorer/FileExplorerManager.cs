using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemont.WPF.Model;
using Nemont.WPF.Service;

namespace Nemont.WPF.Controls.Explorer
{
    public class FileExplorerManager : ExplorerManager
    {
        private string filter = "*";
        private Dictionary<string, string> IconSetter = new Dictionary<string, string>();
        private Dictionary<string, FilterInfo> Filter = new Dictionary<string, FilterInfo>();
        private FileSystemWatcher Watcher;

        private bool autoRefresh = false;

        /// <summary>
        /// 탐색기가 참조하는 디렉토리의 내용이 바뀔 때 자동으로 새로 고침할지에 대한 여부를 설정합니다.
        /// </summary>
        public bool AutoRefresh { get { return autoRefresh; } set { autoRefresh = value; Watcher.EnableRaisingEvents = value; } }

        /// <summary>
        /// 새로 고침 시 탐색기 목록에서 제외할 확장자를 포함합니다.
        /// </summary>
        public List<string> ExceptExtensions = new List<string>();

        /// <summary>
        /// 최상위 폴더 경로입니다.
        /// </summary>
        public string RootDirectoryPath { get; }

        /// <summary>
        /// 빈 내용의 파일 탐색기 매니저입니다.
        /// </summary>
        public FileExplorerManager() { }

        /// <summary>
        /// 주어진 최상위 경로에 대해 새 파일 탐색기를 초기화합니다.
        /// </summary>
        public FileExplorerManager(string rootDirectoryPath, bool autoRefresh)
        {
            RootDirectoryPath = rootDirectoryPath;
            Filter.Add("?", new FilterInfo(typeof(EvItem), -2));
            Filter.Add("//", new FilterInfo(typeof(EvFileFolder), -1));
            Filter.Add("*", new FilterInfo(typeof(EvFile), 0));
            Root.Add(new EvFileFolder(RootDirectoryPath, ""));
            Watcher = new FileSystemWatcher() {
                Path = RootDirectoryPath,
                Filter = "*.*",
                IncludeSubdirectories = true,
            };
            // Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Watcher.Changed += new FileSystemEventHandler(OnChanged);
            Watcher.Created += new FileSystemEventHandler(OnChanged);
            Watcher.Deleted += new FileSystemEventHandler(OnChanged);
            Watcher.Renamed += new RenamedEventHandler(OnRenamed);
            AutoRefresh = autoRefresh;
        }

        /// <summary>
        /// 새로 고침 시 특정 확장자에 대응할 고유한 아이콘 경로를 지정합니다.
        /// </summary>
        public void SetIconSetter(string extension, string iconUri)
        {
            var lower_ext = extension.ToLower();

            if (IconSetter.ContainsKey(lower_ext) == true) {
                IconSetter[lower_ext] = iconUri;
            }
            else {
                IconSetter.Add(extension.ToLower(), iconUri);
            }
        }

        public string GetIconSetter(string extension)
        {
            if (IconSetter.ContainsKey(extension.ToLower()) == false) {
                return null;
            }
            return IconSetter[extension];
        }

        public void SetFilter(Type type, string extension)
        {
            var newItem = new FilterInfo(type, Filter.Count - 2);
            Filter.Add(extension.ToLower(), newItem);
            Filter["*"].Order = Filter.Count - 2;
        }

        /// <summary>
        /// 특정 필터링 모드에 대해 필터링을 수행합니다.
        /// </summary>
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
            else if (filterMode == FilterMode.Nothing) {
                foreach (var root in Root) {
                    root.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 특정 확장자에 대해 필터링을 수행합니다.
        /// </summary>
        public void Filtering(string fileExtension)
        {
            var lower_ext = fileExtension.ToLower();

            if (lower_ext != "\\" && lower_ext != "*" && Filter.ContainsKey(lower_ext) == true) {
                filter = lower_ext;
                RefreshDirectory();
            }
        }

        /// <summary>
        /// 수동으로 새로 고침을 수행합니다.
        /// </summary>
        public void RefreshDirectory()
        {
            RefreshRecur(Root[0] as EvFileFolder);
        }

        private void RefreshRecur(EvFileFolder parent)
        {
            var absPath = RootDirectoryPath + "\\" + parent.RelativePath;
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

            /// 찾은 파일이 탐색기 목록에 없을 경우 추가
            for (int i = 0; i < existChk.Length; i++) {
                if (existChk[i] == false) {
                    var name = fileSystems[i].Name;
                    var path = GetRelativePath(RootDirectoryPath, fileSystems[i].FullName);
                    var toolTip = fileSystems[i].FullName;

                    if (Directory.Exists(fileSystems[i].FullName)) {
                        /// 폴더가 존재할 경우 폴더 추가
                        var addFolder = new EvFileFolder(name, path) { ToolTip = toolTip };
                        RefreshRecur(addFolder);
                        parent.Sub.Add(addFolder);
                    }
                    else {
                        /// 폴더가 아닌 파일일 경우 파일 추가
                        EvItem addItem;
                        var ext = Path.GetExtension(fileSystems[i].Name).ToLower();
                        var fil = Filter.ContainsKey(ext);
                        var isException = ExceptExtensions.FindIndex(item => item.ToLower() == ext);
                        if (isException != -1) {
                            continue;
                        }
                        if (fil == true) {
                            addItem = Activator.CreateInstance(Filter[ext].Type, name, path) as EvFile;
                        }
                        else {
                            addItem = new EvFile(name, path);
                        }
                        if (IconSetter.ContainsKey(ext) == true) {
                            addItem.IconUri = IconSetter[ext];
                        }
                        addItem.ToolTip = toolTip;
                        parent.Sub.Add(addItem);
                    }
                }
            }

            /// Filtering
            for (int i = 0; i < parent.Sub.Count; i++) {
                var item = parent.Sub[i];
                var ext = Path.GetExtension(item.Name).ToLower();
                if (item is EvFile && (filter == "\\" || (filter != "*" && filter != ext))) {
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
    }
}
