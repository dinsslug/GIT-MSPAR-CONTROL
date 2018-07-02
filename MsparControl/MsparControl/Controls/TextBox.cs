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

namespace MsparControl.Controls
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
            Loaded += TextBox_Loaded;
        }

        private new void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            base.TextBox_Loaded(sender, e);

            try
            {
                CheckTextLength();
            }
            catch { }
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
            try
            {
                var btnX = Template.FindName("BtnX", this) as Button;
                var partEmptyDescription = Template.FindName("PART_EmptyDescription", this) as TextBlock;

                if (!Text.Equals(DefaultText) && ClearButtonVisible == true)
                {
                    btnX.Visibility = Visibility.Visible;
                }
                else
                {
                    btnX.Visibility = Visibility.Collapsed;
                }

                if (Text == "")
                {
                    partEmptyDescription.Visibility = Visibility.Visible;
                }
                else
                {
                    partEmptyDescription.Visibility = Visibility.Hidden;
                }
            }
            catch { }
        }
    }
}
