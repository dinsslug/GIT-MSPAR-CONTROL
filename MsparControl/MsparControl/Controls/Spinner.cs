using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MsparControl.Controls
{
    public class Spinner : UserControl
    {
        public Spinner()
        {

        }

        static Spinner()
        {
            InitializeCommands();

            DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));
        }


        #region Value property
        /// <summary>
        /// This is the Control property that we expose to the user.
        /// </summary>
        [Category("SpinnerControl")]
        public decimal Value { get { return (decimal)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(Spinner),
            new FrameworkPropertyMetadata(DefaultValue,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnValueChanged
                ));

        /// <summary>
        /// If the value changes, update the text box that displays the Value 
        /// property to the consumer.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Spinner control = obj as Spinner;

            if (control != null)
            {
                var newValue = (decimal)args.NewValue;
                var oldValue = (decimal)args.OldValue;

                RoutedPropertyChangedEventArgs<decimal> e = new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue, ValueChangedEvent);

                control.OnValueChanged(e);
            }
        }

        /// <summary>
        /// Raise the ValueChanged event.  Derived classes can use this.
        /// </summary>
        /// <param name="e"></param>
        virtual protected void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> e)
        {
            RaiseEvent(e);
        }
        #endregion
        

        #region DecimalPlaces property
        /// <summary>
        /// This is the Control property that we expose to the user.
        /// </summary>
        [Category("SpinnerControl")]
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        private static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(Spinner),
            new PropertyMetadata(DefaultDecimalPlaces));

        #endregion


        #region Change property
        /// <summary>
        /// This is the Control property that we expose to the user.
        /// </summary>
        [Category("SpinnerControl")]
        public decimal Change { get { return (decimal)GetValue(ChangeProperty); } set { SetValue(ChangeProperty, value); } }
        private static readonly DependencyProperty ChangeProperty =
            DependencyProperty.Register("Change", typeof(decimal), typeof(Spinner),
            new PropertyMetadata(DefaultChange));
        #endregion


        #region Default values
        /// <summary>
        /// Define the min, max and starting value, which we then expose 
        /// as dependency properties.
        /// </summary>
        private const Decimal DefaultMinimumValue = 0,
            DefaultMaximumValue = 100,
            DefaultValue = DefaultMinimumValue,
            DefaultChange = 1;

        /// <summary>
        /// The default number of decimal places, i.e. 0, and show the
        /// spinner control as an int initially.
        /// </summary>
        private const int DefaultDecimalPlaces = 0;
        #endregion

        
        public static RoutedCommand IncreaseCommand { get; set; }
        public static RoutedCommand DecreaseCommand { get; set; }

        protected static void OnIncreaseCommand(Object sender, ExecutedRoutedEventArgs e)
        {
            Spinner control = sender as Spinner;

            if (control != null)
            {
                control.OnIncrease();
            }
        }

        protected void OnIncrease()
        {
            Value += Change;
        }

        protected static void OnDecreaseCommand(Object sender, ExecutedRoutedEventArgs e)
        {
            Spinner control = sender as Spinner;

            if (control != null)
            {
                control.OnDecrease();
            }
        }

        protected void OnDecrease()
        {
            Value -= Change;
        }

        /// <summary>
        /// Since we're using RoutedCommands for the up/down buttons, we need to
        /// register them with the command manager so we can tie the events
        /// to callbacks in the control.
        /// </summary>
        private static void InitializeCommands()
        {
            //  create instances
            IncreaseCommand = new RoutedCommand("IncreaseCommand", typeof(Spinner));
            DecreaseCommand = new RoutedCommand("DecreaseCommand", typeof(Spinner));

            //  register the command bindings - if the buttons get clicked, call these methods.
            CommandManager.RegisterClassCommandBinding(typeof(Spinner), new CommandBinding(IncreaseCommand, OnIncreaseCommand));
            CommandManager.RegisterClassCommandBinding(typeof(Spinner), new CommandBinding(DecreaseCommand, OnDecreaseCommand));

            //  lastly bind some inputs:  i.e. if the user presses up/down arrow 
            //  keys, call the appropriate commands.
            CommandManager.RegisterClassInputBinding(typeof(Spinner), new InputBinding(IncreaseCommand, new KeyGesture(Key.Up)));
            CommandManager.RegisterClassInputBinding(typeof(Spinner), new InputBinding(IncreaseCommand, new KeyGesture(Key.Right)));
            CommandManager.RegisterClassInputBinding(typeof(Spinner), new InputBinding(DecreaseCommand, new KeyGesture(Key.Down)));
            CommandManager.RegisterClassInputBinding(typeof(Spinner), new InputBinding(DecreaseCommand, new KeyGesture(Key.Left)));
        }


        #region Events
        /// <summary>
        /// The ValueChangedEvent, raised if  the value changes.
        /// </summary>
        private static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(Spinner));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion
    }
}
