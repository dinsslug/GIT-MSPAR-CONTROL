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
    
    public class BaseTextBox : System.Windows.Controls.TextBox
    {
        public BaseTextBox()
        {
            Loaded += TextBox_Loaded;
        }

        static BaseTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseTextBox), new FrameworkPropertyMetadata(typeof(BaseTextBox)));
        }

        // InputMode Property
        [Category("Common Properties")]
        public TextBoxMode InputMode { get { return (TextBoxMode)GetValue(InputModeProperty); } set { SetValue(InputModeProperty, value); } }
        public static readonly DependencyProperty InputModeProperty =
            DependencyProperty.RegisterAttached("InputMode", typeof(TextBoxMode), typeof(BaseTextBox), new FrameworkPropertyMetadata(TextBoxMode.Normal));

        protected void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var partContentHost = (ScrollViewer)Template.FindName("PART_ContentHost", this);

                partContentHost.MouseLeftButtonDown += PartContentHost_MouseLeftButtonDown;
            }
            catch { }
        }

        private void PartContentHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
