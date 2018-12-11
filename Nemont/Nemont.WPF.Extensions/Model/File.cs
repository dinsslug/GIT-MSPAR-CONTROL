using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Model.ExplorerView
{
    public class File : Item
    {
        public string RelPath;

        public File(string name, string relPath) : base(name, "pack://application:,,,/Nemont.WPF.Extensions;Component/Asset/file.png")
        {
            RelPath = relPath;
        }
    }
}
