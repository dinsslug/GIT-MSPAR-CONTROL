using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nemont.WPF.Service;

namespace Nemont.WPF.Controls
{
    public class ExplorerViewItem : TreeViewItem
    {
        public static new readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(ExplorerViewItem),
            new PropertyMetadata(false));

        public new bool IsSelected {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        static ExplorerViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerViewItem), new FrameworkPropertyMetadata(typeof(ExplorerViewItem)));
        }

        public ExplorerViewItem()
        {
            RequestBringIntoView += ExplorerViewItem_RequestBringIntoView;
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);

            e.Handled = true;

            var view = Support.FindVisualParent<ExplorerView>(this);

            view.ExplorerView_ChangeSelectedItem(this);
        }

        private void ExplorerViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
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
