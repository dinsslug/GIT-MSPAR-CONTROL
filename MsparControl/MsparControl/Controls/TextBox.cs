using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace MsparControl.Controls
{
    public enum TextBoxMode
    {
        Normal, NumberOnly, DecimalOnly
    }
    
    public class TextBox : System.Windows.Controls.TextBox
    {
        // InputMode Property
        [Category("Option")]
        public TextBoxMode InputMode { get { return (TextBoxMode)GetValue(InputModeProperty); } set { SetValue(InputModeProperty, value); } }
        public static readonly DependencyProperty InputModeProperty =
            DependencyProperty.RegisterAttached("InputMode", typeof(TextBoxMode), typeof(TextBox), new FrameworkPropertyMetadata(TextBoxMode.Normal));

        [Category("Option")]
        // Set Clear Button Property
        public bool ClearButtonVisible { get { return (bool)GetValue(ClearButtonVisibleProperty); } set { SetValue(ClearButtonVisibleProperty, value); } }
        public static DependencyProperty ClearButtonVisibleProperty =
            DependencyProperty.Register("ClearButtonVisible", typeof(bool), typeof(TextBox), new PropertyMetadata(true));

        static TextBox()
        {
            ClearCommand = new RoutedCommand("ClearCommand", typeof(TextBox));
            
            CommandManager.RegisterClassCommandBinding(typeof(TextBox), new CommandBinding(ClearCommand, OnClearCommand));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        public static RoutedCommand ClearCommand { get; set; }

        protected static void OnClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control != null)
            {
                control.Text = "";
            }
        }
        
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            
            if(Template == null)
            {
                return;
            }
            var button = Template.FindName("BtnX", this) as Button;

            if (Text.Length > 0 && ClearButtonVisible == true)
            {
                button.Visibility = Visibility.Visible;
            }
            else
            {
                button.Visibility = Visibility.Hidden;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            
            if (!IsKeyboardFocusWithin)
            {
                Focus();
                e.Handled = true;
            }
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            SelectAll();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            SelectAll();
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            switch (InputMode)
            {
                case TextBoxMode.NumberOnly:
                    if (!char.IsDigit(e.Text, e.Text.Length - 1))
                    {
                        e.Handled = true;
                    }
                    return;
                case TextBoxMode.DecimalOnly:
                    bool approvedDecimalPoint = false;
                    bool approvedNegative = false;

                    if (e.Text == ".")
                    {
                        if (!(Text.Contains(".") || CaretIndex == 0 || !char.IsDigit(Text, CaretIndex - 1)))
                        {
                            approvedDecimalPoint = true;
                        }
                    }
                    else if (e.Text == "-")
                    {
                        if (!(Text.Contains("-") || CaretIndex != 0))
                        {
                            approvedNegative = true;
                        }
                    }

                    if (!((char.IsDigit(e.Text, e.Text.Length - 1)) || approvedDecimalPoint || approvedNegative))
                    {
                        e.Handled = true;
                    }
                    return;
            }
        }
    }
}
