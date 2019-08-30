using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nemont.WPF.AppService.Threading
{
    public class StartInfo
    {
        public Window Owner;
        public double? Height;
        public double? Width;
        public bool IsDialog;
        public string Title;
        public bool? ShowInTaskBar;
    }
}
