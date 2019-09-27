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
using Nemont.WPF.Service;

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

        public bool AllowMultiSelect { get { return (bool)GetValue(AllowMultiSelectProperty); } set { SetValue(AllowMultiSelectProperty, value); } }
        public static readonly DependencyProperty AllowMultiSelectProperty =
            DependencyProperty.Register(nameof(AllowMultiSelect), typeof(bool), typeof(ExplorerView),
                new FrameworkPropertyMetadata(null));

        public IList SelectedItems { get; } = new List<object>();

        private ExplorerViewItem LastSelectedItem;
        private List<ExplorerViewItem> SelectedViewItems { get; } = new List<ExplorerViewItem>();

        private bool IsCtrlPressed => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        private bool IsShiftPressed => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        static ExplorerView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerView), new FrameworkPropertyMetadata(typeof(ExplorerView)));
        }

        private static void OnManagerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (ExplorerView)d;

            source.ItemsSource = source.Manager.Root;
        }

        public void ExplorerView_ChangeSelectedItem(ExplorerViewItem sender)
        {
            if (Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Pressed) {
                if (IsShiftPressed == false && IsCtrlPressed == false) {
                    MultiSelector_CollectSingle(sender);
                }
            }
            else if (IsShiftPressed == true && AllowMultiSelect == true) {
                MultiSelector_CollectRange(sender);
            }
            else if (IsCtrlPressed == true && AllowMultiSelect == true) {
                sender.IsSelected = !sender.IsSelected;
                LastSelectedItem = sender;
            }
            else {
                MultiSelector_CollectSingle(sender);
            }

            if (SelectedViewItems.Contains(sender) == true){
                if (sender.IsSelected == false) {
                    SelectedViewItems.Remove(sender);
                }
            }
            else {
                SelectedViewItems.Add(sender);
            }

            SelectedItems.Clear();
            SelectedViewItems.ForEach(item => SelectedItems.Add(item.DataContext));
        }

        private void MultiSelector_CollectSingle(ExplorerViewItem sender)
        {
            SelectedViewItems.ForEach(item => item.IsSelected = false);
            SelectedViewItems.Clear();
            sender.IsSelected = true;
            LastSelectedItem = sender;
        }

        private void MultiSelector_CollectRange(ExplorerViewItem target)
        {
            if (LastSelectedItem == null) {
                return;
            }
            var collectedItems = MultiSelector_GetListInRange(LastSelectedItem, target);

            if (IsCtrlPressed == true) {
                collectedItems.ForEach(c => SelectedViewItems.Remove(c));
            }
            else {
                SelectedViewItems.ForEach(s => s.IsSelected = false);
                SelectedViewItems.Clear();
            }

            foreach (var c in collectedItems) {
                c.IsSelected = true;
                SelectedViewItems.Add(c);
            }
        }

        private static List<ExplorerViewItem> MultiSelector_GetList(ItemsControl parentItem, bool includeCollapsedItems, List<ExplorerViewItem> itemList = null)
        {
            if (itemList == null) {
                itemList = new List<ExplorerViewItem>();
            }

            for (var index = 0; index < parentItem.Items.Count; index++) {
                var tvItem = parentItem.ItemContainerGenerator.ContainerFromIndex(index) as ExplorerViewItem;
                if (tvItem == null) {
                    continue;
                }

                itemList.Add(tvItem);
                if (includeCollapsedItems || tvItem.IsExpanded) {
                    MultiSelector_GetList(tvItem, includeCollapsedItems, itemList);
                }
            }
            return itemList;
        }

        private List<ExplorerViewItem> MultiSelector_GetListInRange(ExplorerViewItem start, ExplorerViewItem end)
        {
            var items = MultiSelector_GetList(this, false);

            var startIndex = items.IndexOf(start);
            var endIndex = items.IndexOf(end);
            var rangeStart = startIndex > endIndex || startIndex == -1 ? endIndex : startIndex;
            var rangeCount = startIndex > endIndex ? startIndex - endIndex + 1 : endIndex - startIndex + 1;

            if (startIndex == -1 && endIndex == -1) {
                rangeCount = 0;
            }

            else if (startIndex == -1 || endIndex == -1) {
                rangeCount = 1;
            }

            var res = rangeCount > 0 ? items.GetRange(rangeStart, rangeCount) : new List<ExplorerViewItem>();
            if (startIndex > endIndex) {
                res.Reverse();
            }

            return res;
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            var target_item = Support.FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
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

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.ChangedButton == MouseButton.Right) {
                return;
            }
            var target_item = Support.FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            if (target_item == null) {
                return;
            }

            var context = target_item.DataContext as EvBase;
            if (context == null) {
                return;
            }

            Manager.OnDoubleClick?.Invoke(context, target_item, this, e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExplorerViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ExplorerViewItem;
        }
    }
}
