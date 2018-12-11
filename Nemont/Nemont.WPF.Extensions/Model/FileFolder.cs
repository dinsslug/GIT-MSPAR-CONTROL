using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Model.ExplorerView
{
    public class FileFolder : Folder
    {
        // Icon
        public string IconClosedUri { get; set; } = "pack://application:,,,/MSPAR-SITE;component/Asset/TreeView/folder_close.png";
        public string IconOpenedUri { get; set; } = "pack://application:,,,/MSPAR-SITE;component/Asset/TreeView/folder_open.png";
        public BitmapImage IconClosed { get { return new BitmapImage(new Uri(IconClosedUri)); } }
        public BitmapImage IconOpened { get { return new BitmapImage(new Uri(IconOpenedUri)); } }
    }
}
