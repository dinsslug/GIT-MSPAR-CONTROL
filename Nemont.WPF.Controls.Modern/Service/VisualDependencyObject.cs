using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nemont.WPF.Service
{
    public static class VisualDependencyObject
    {
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child != null && child is T) {
                    return (T)child;
                }
                else {
                    var childOfChild = FindChild<T>(child);

                    if (childOfChild != null) {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) {
                return null;
            }

            T foundChild = null;

            int child_count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < child_count; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (!(child is T)) {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName)) {
                    var frameworkElement = child as FrameworkElement;

                    if (frameworkElement != null && frameworkElement.Name == childName) {
                        foundChild = (T)child;
                        break;
                    }
                }
                else {
                    foundChild = (T)child;

                    break;
                }
            }

            return foundChild;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
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
