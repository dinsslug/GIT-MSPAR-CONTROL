using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Nemont
{
    public class TextBox : BaseTextBox
    {
        static TextBox()
        {
            ClearCommand = new RoutedCommand("ClearCommand", typeof(TextBox));

            CommandManager.RegisterClassCommandBinding(typeof(TextBox), new CommandBinding(ClearCommand, OnClearCommand));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        public TextBox()
        {
            Loaded += NemontTextBox_Loaded;
        }

        private void NemontTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_Loaded(sender, e);

            CheckTextLength();
        }
        
        [Category("Common Properties")]
        public string DefaultText { get { return (string)GetValue(DefaultTextProperty); } set { SetValue(DefaultTextProperty, value); } }
        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.RegisterAttached("DefaultText", typeof(string), typeof(TextBox), new FrameworkPropertyMetadata(""));

        [Category("Common Properties")]
        public string EmptyDescription { get { return (string)GetValue(EmptyDescriptionProperty); } set { SetValue(EmptyDescriptionProperty, value); } }
        public static DependencyProperty EmptyDescriptionProperty =
            DependencyProperty.Register("EmptyDescription", typeof(string), typeof(TextBox), new PropertyMetadata(""));

        [Category("Common Properties")]
        public bool ClearButtonVisible { get { return (bool)GetValue(ClearButtonVisibleProperty); } set { SetValue(ClearButtonVisibleProperty, value); } }
        public static DependencyProperty ClearButtonVisibleProperty =
            DependencyProperty.Register("ClearButtonVisible", typeof(bool), typeof(TextBox), new PropertyMetadata(true));

        [Category("Brushes")]
        public Brush EmptyDescBrush { get { return (Brush)GetValue(EmptyDescBrushProperty); } set { SetValue(EmptyDescBrushProperty, value); } }
        public static DependencyProperty EmptyDescBrushProperty =
            DependencyProperty.Register("EmptyDescBrush", typeof(Brush), typeof(TextBox), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static RoutedCommand ClearCommand { get; set; }

        protected static void OnClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox control = sender as TextBox;

            control?.OnClear();
        }

        private void OnClear()
        {
            Text = DefaultText;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            CheckTextLength();
        }

        private void CheckTextLength()
        {
            if (Template == null) {
                return;
            }
            var btnClear = (Button)Template.FindName("Btn_Clear", this);
            var partEmptyDescription = (TextBlock)Template.FindName("PART_EmptyDescription", this);

            if (btnClear != null) {
                if (!Text.Equals(DefaultText) && ClearButtonVisible == true) {
                    btnClear.Visibility = Visibility.Visible;
                }
                else {
                    btnClear.Visibility = Visibility.Collapsed;
                }
            }

            if (partEmptyDescription != null) {
                if (Text == "") {
                    partEmptyDescription.Visibility = Visibility.Visible;
                }
                else {
                    partEmptyDescription.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
