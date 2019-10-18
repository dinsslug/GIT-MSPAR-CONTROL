using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nemont.WPF.Controls
{
    public class TabColors
    {
        public static Brush SelectedBackground { get; }
        public static ComponentResourceKey SelectedBackgroundKey => new ComponentResourceKey(typeof(TabColors), nameof(SelectedBackground));

        public static Brush SelectedForeground { get; }
        public static ComponentResourceKey SelectedForegroundKey => new ComponentResourceKey(typeof(TabColors), nameof(SelectedForeground));

        public static Brush SelectedChildForeground { get; }
        public static ComponentResourceKey SelectedChildForegroundKey => new ComponentResourceKey(typeof(TabColors), nameof(SelectedChildForeground));

        public static Brush SelectedChildBackground { get; }
        public static ComponentResourceKey SelectedChildBackgroundKey => new ComponentResourceKey(typeof(TabColors), nameof(SelectedChildBackground));

        public static Brush ExpandedGroupBoxBackground { get; }
        public static ComponentResourceKey ExpandedGroupBoxBackgroundKey => new ComponentResourceKey(typeof(TabColors), nameof(ExpandedGroupBoxBackground));
    }
}
