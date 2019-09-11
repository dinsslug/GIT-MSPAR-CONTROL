using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Nemont.WPF.Controls.Explorer;

namespace Nemont.WPF.Controls
{
    public class ExplorerView : TreeView
    {
        public ViewManager Manager { get { return (ViewManager)GetValue(ManagerProperty); } set { SetValue(ManagerProperty, value); } }
        public static readonly DependencyProperty ManagerProperty =
            DependencyProperty.Register(nameof(Manager), typeof(ViewManager), typeof(ExplorerView),
                new FrameworkPropertyMetadata(null, OnManagerPropertyChanged));

        public new IEnumerable ItemsSource {
            get { return ((ViewManager)GetValue(ManagerProperty)).Root; }
            private set { SetValue(ItemsSourceProperty, value); }
        }

        static ExplorerView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerView), new FrameworkPropertyMetadata(typeof(ExplorerView)));
        }

        private static void OnManagerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (ExplorerView)d;

            source.ItemsSource = source.Manager.Root;
        }

        public ExplorerView()
        {
            MouseDoubleClick += ExplorerView_MouseDoubleClick;
            MouseRightButtonDown += ExplorerView_MouseRightButtonDown;
        }

        private T FindVisualChild<T>(DependencyObject o) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++) {
                var child = VisualTreeHelper.GetChild(o, i);

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

        private T FindVisualParent<T>(DependencyObject o) where T : DependencyObject
        {
            var res = o;

            while (!(res is T)) {
                res = VisualTreeHelper.GetParent(res);

                if (res == null){
                    return null;
                }
            }

            return (T)res;
        }

        private void ExplorerView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var target_item = FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            if (target_item == null) {
                return;
            }

            var context = target_item.DataContext as EvBase;
            if (context == null) {
                return;
            }

            target_item.Focus();
            Manager.OnRightClick?.Invoke(context, target_item, this, e);
            e.Handled = true;
        }

        private void ExplorerView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right) {
                return;
            }
            var target_item = FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            if (target_item == null) {
                return;
            }

            var context = target_item.DataContext as EvBase;
            if (context == null) {
                return;
            }

            Manager.OnDoubleClick?.Invoke(context, target_item, this, e);
        }
    }
}
