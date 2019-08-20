using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Demo.Model
{
    public class EvText : WPF.Controls.Explorer.EvFile
    {
        public EvText(string name, string relativePath) : base(name, relativePath)
        {
            IconUri = "pack://application:,,,/Nemont.WPF.Demo;component/Asset/site.png";
        }
    }
}
