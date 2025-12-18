using System;
using System.Windows;
using System.Windows.Controls;

namespace PrintDialogX.PrintControl.UserControls
{
    partial class NumberPicker : TextBox
    {
        double lastNumber = 0;
        Button numberUpButton = new Button();
        Button numberDownButton = new Button();

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(NumberPicker), new PropertyMetadata(0, new PropertyChangedCallback(OnMinValueChanged)));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(NumberPicker), new PropertyMetadata(999, new PropertyChangedCallback(OnMaxValueChanged)));

        public static readonly DependencyProperty StepValueProperty =
            DependencyProperty.Register("Step", typeof(int), typeof(NumberPicker), new PropertyMetadata(1, new PropertyChangedCallback(OnStepChanged)));

        static void OnMinValueChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            NumberPicker source = (NumberPicker)sender;
            source.MinValue = (int)args.NewValue;

            if (source.Visibility == Visibility.Visible && source.IsLoaded == true)
            {
                source.UpdateLayout();

                source.ChangeButtonEnabled();
            }
        }

        static void OnMaxValueChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            NumberPicker source = (NumberPicker)sender;
            source.MaxValue = (int)args.NewValue;

            if (source.Visibility == Visibility.Visible && source.IsLoaded == true)
            {
                source.UpdateLayout();

                source.ChangeButtonEnabled();
            }
        }

        static void OnStepChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            NumberPicker source = (NumberPicker)sender;
            source.Step = (int)args.NewValue;

            if (source.Visibility == Visibility.Visible && source.IsLoaded == true)
            {
                source.UpdateLayout();

                source.ChangeButtonEnabled();
            }
        }

        public int MinValue
        {
            get
            {
                return (int)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public int MaxValue
        {
            get
            {
                return (int)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public int Step
        {
            get
            {
                return (int)GetValue(StepValueProperty);
            }
            set
            {
                SetValue(StepValueProperty, value);
            }
        }

        public static readonly RoutedEvent InputCompletedEvent = EventManager.RegisterRoutedEvent("InputCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberPicker));
        public event RoutedEventHandler InputCompleted
        {
            add
            {
                AddHandler(InputCompletedEvent, value);
            }
            remove
            {
                RemoveHandler(InputCompletedEvent, value);
            }
        }

        public NumberPicker()
        {
            InitializeComponent();
        }

        public void ChangeButtonEnabled()
        {
            if (numberUpButton == null || numberDownButton == null)
            {
                numberUpButton = (Button)this.Template.FindName("numberUpButton", this);
                numberDownButton = (Button)this.Template.FindName("numberDownButton", this);
            }

            try
            {
                if (numberUpButton != null || numberDownButton != null)
                {
                    numberUpButton.IsEnabled = true;
                    numberDownButton.IsEnabled = true;

                    if (int.Parse(this.Text) <= this.MinValue)
                    {
                        this.Text = this.MinValue.ToString();
                        numberDownButton.IsEnabled = false;
                    }
                    if (int.Parse(this.Text) >= this.MaxValue)
                    {
                        this.Text = this.MaxValue.ToString();
                        numberUpButton.IsEnabled = false;
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private void NumberUpButtonClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = this.MinValue.ToString();
            }

            if (int.Parse(this.Text) + this.Step <= this.MaxValue)
            {
                this.Text = (double.Parse(this.Text) + this.Step).ToString();
            }
            else
            {
                this.Text = this.MaxValue.ToString();
            }

            ChangeButtonEnabled();

            this.Focus();
            this.SelectAll();

            RoutedEventArgs args = new RoutedEventArgs(InputCompletedEvent);
            this.RaiseEvent(args);
        }

        private void NumberDownButtonClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = this.MinValue.ToString();
            }

            if (int.Parse(this.Text) - this.Step >= this.MinValue)
            {
                this.Text = (double.Parse(this.Text) - this.Step).ToString();
            }
            else
            {
                this.Text = this.MinValue.ToString();
            }

            ChangeButtonEnabled();

            this.Focus();
            this.SelectAll();

            RoutedEventArgs args = new RoutedEventArgs(InputCompletedEvent);
            this.RaiseEvent(args);
        }

        private void NumberPickerControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.Text))
            {
                lastNumber = this.MinValue;
                return;
            }

            try
            {
                int number = int.Parse(this.Text);
                lastNumber = number;
            }
            catch
            {
                int index = this.SelectionStart;

                this.Text = lastNumber.ToString();

                this.SelectionStart = index - 1;
            }
        }

        private void NumberPickerControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = this.MinValue.ToString();
            }

            if (int.Parse(this.Text) > this.MaxValue)
            {
                this.Text = this.MaxValue.ToString();
            }
            else if (int.Parse(this.Text) < this.MinValue)
            {
                this.Text = this.MinValue.ToString();
            }

            ChangeButtonEnabled();

            RoutedEventArgs args = new RoutedEventArgs(InputCompletedEvent);
            this.RaiseEvent(args);
        }

        private void NumberPickerControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();

            if (String.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = this.MinValue.ToString();
            }

            lastNumber = int.Parse(this.Text);
            numberUpButton = (Button)this.Template.FindName("numberUpButton", this);
            numberDownButton = (Button)this.Template.FindName("numberDownButton", this);

            ChangeButtonEnabled();
        }

        private void NumberPickerControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible == true)
            {
                NumberPickerControl_Loaded(sender, new RoutedEventArgs());
            }
        }
    }
}
