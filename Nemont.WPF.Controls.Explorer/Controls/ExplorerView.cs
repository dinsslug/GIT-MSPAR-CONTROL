using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nemont.WPF.Controls
{
    public class ExplorerView : System.Windows.Controls.TreeView
    {
        static ExplorerView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerView), new FrameworkPropertyMetadata(typeof(ExplorerView)));
        }
    }
}
