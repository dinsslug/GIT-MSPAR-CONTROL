using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemont.Demo.Model
{
    public class EvText : WPF.Controls.Explorer.EvFile
    {
        public override string DefaultIconUri => "pack://application:,,,/Nemont.WPF.Demo;Component/Asset/work_et.png";

        public EvText(string name, string relativePath) : base(name, relativePath)
        {
        }
    }
}
