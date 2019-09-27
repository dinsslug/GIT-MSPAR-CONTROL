using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nemont.WPF.Service
{
    internal static class Support
    {
        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child != null && child is T) {
                    return (T)child;
                }
                else {
                    var childOfChild = FindVisualChild<T>(child);

                    if (childOfChild != null) {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var res = child;

            while (!(res is T)) {
                res = VisualTreeHelper.GetParent(res);

                if (res == null) {
                    return null;
                }
            }

            return (T)res;
        }
    }
}
