using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Nemont.WPF.Controls
{
    public class Spinner : BaseTextBox
    {
        static Spinner()
        {
            IncreaseCommand = new RoutedCommand("IncreaseCommand", typeof(Spinner));
            DecreaseCommand = new RoutedCommand("DecreaseCommand", typeof(Spinner));

            CommandManager.RegisterClassCommandBinding(typeof(Spinner), new CommandBinding(IncreaseCommand, OnIncreaseCommand));
            CommandManager.RegisterClassCommandBinding(typeof(Spinner), new CommandBinding(DecreaseCommand, OnDecreaseCommand));

            CommandManager.RegisterClassInputBinding(typeof(Spinner), new InputBinding(IncreaseCommand, new KeyGesture(Key.Up)));
            CommandManager.RegisterClassInputBinding(typeof(Spinner), new InputBinding(IncreaseCommand, new KeyGesture(Key.Right)));
            CommandManager.RegisterClassInputBinding(typeof(Spinner), new InputBinding(DecreaseCommand, new KeyGesture(Key.Down)));
            CommandManager.RegisterClassInputBinding(typeof(Spinner), new InputBinding(DecreaseCommand, new KeyGesture(Key.Left)));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));
        }
        
        public Spinner()
        {
            Loaded += Spinner_Loaded;
        }

        private void Spinner_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var partButtonUp = (RepeatButton)Template.FindName("PART_ButtonUp", this);
                var partButtonDown = (RepeatButton)Template.FindName("PART_ButtonDown", this);
                
                partButtonUp.PreviewMouseLeftButtonDown += PartButton_PreviewMouseLeftButtonDown;
                partButtonDown.PreviewMouseLeftButtonDown += PartButton_PreviewMouseLeftButtonDown;
                partButtonUp.PreviewMouseLeftButtonUp += PartButton_PreviewMouseLeftButtonUp;
                partButtonDown.PreviewMouseLeftButtonUp += PartButton_PreviewMouseLeftButtonUp;
            }
            catch { }
        }

        private void PartButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var be = GetBindingExpression(TextProperty);

            be.UpdateSource();
        }

        private void PartButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        [Category("Common Properties"), Description("값의 변화량을 설정합니다.")]
        public double Change { get { return (double)GetValue(ChangeProperty); } set { SetValue(ChangeProperty, value); } }
        private static readonly DependencyProperty ChangeProperty =
            DependencyProperty.Register("Change", typeof(double), typeof(Spinner),
            new PropertyMetadata(1.0));

        public static RoutedCommand IncreaseCommand { get; set; }
        public static RoutedCommand DecreaseCommand { get; set; }

        protected static void OnIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Spinner control = sender as Spinner;
            
            control?.OnIncrease();
        }

        protected void OnIncrease()
        {
            double parsedValue;

            if (double.TryParse(Text, out parsedValue))
            {
                Text = (parsedValue + Change).ToString();
            }
        }

        protected static void OnDecreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Spinner control = sender as Spinner;

            control?.OnDecrease();
        }

        protected void OnDecrease()
        {
            double parsedValue;

            if (double.TryParse(Text, out parsedValue))
            {
                Text = (parsedValue - Change).ToString();
            }
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Spinner control = obj as Spinner;

            if (control != null)
            {
                var newValue = (string)args.NewValue;
                var oldValue = (string)args.OldValue;

                var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue, ValueChangedEvent);

                control.OnValueChanged(e);
            }
        }

        virtual protected void OnValueChanged(RoutedPropertyChangedEventArgs<string> e)
        {
            RaiseEvent(e);
        }

        private static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<string>), typeof(Spinner));

        public event RoutedPropertyChangedEventHandler<string> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
    }
}
