using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nemont.WPF.Controls
{
    public class TabGroupBox : ItemsControl
    {
        [Category("Common Properties")]
        public string Header { get { return (GetValue(HeaderProperty) ?? "").ToString(); } set { SetValue(HeaderProperty, value); } }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(TabGroupBox), new PropertyMetadata());

        [Category("Common Properties")]
        public bool IsChecked { get { return (bool)GetValue(IsCheckedProperty); } set { SetValue(IsCheckedProperty, value); } }
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(TabGroupBox), new PropertyMetadata());

        [Category("Brushes")]
        public Brush ExpandBackground { get { return (Brush)GetValue(SelectedForegroundProperty); } set { SetValue(SelectedForegroundProperty, value); } }
        public static DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register(nameof(ExpandBackground), typeof(Brush), typeof(TabButton), new PropertyMetadata(SystemColors.HighlightTextBrush));

        static TabGroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabGroupBox), new FrameworkPropertyMetadata(typeof(TabGroupBox)));
        }
    }
}
