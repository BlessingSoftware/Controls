using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BlessingSoftware.Controls {


    [TemplatePart(Name = "PART_HoursTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_MinutesTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_SecondsTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_12TimeSystem1", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_12TimeSystem2", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_24TimeSystem1", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_24TimeSystem2", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_HoursPopup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_MinutesPopup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_SecondsPopup", Type = typeof(Popup))]
    public class TimeSapnPicker :Control {

        private Popup m_hoursPopup = null;

        private StackPanel m_12TimeSystem1 = null;
        private StackPanel m_12TimeSystem2 = null;
        private StackPanel m_24TimeSystem1 = null;
        private StackPanel m_24TimeSystem2 = null;

        private TextBox m_hoursTextBox = null;
        private TextBox m_minutesTextBox = null;
        private TextBox m_secondsTextBox = null;

        public static readonly DependencyProperty ValueProperty;

        public TimeSpan Value {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        public bool Is12TimeSystem {
            get { return (bool)GetValue(Is12TimeSystemProperty); }
            set { SetValue(Is12TimeSystemProperty, value); }
        }

        public static readonly DependencyProperty Is12TimeSystemProperty =
            DependencyProperty.Register("Is12TimeSystem", typeof(bool), typeof(TimeSapnPicker), new PropertyMetadata(false));

        public bool IsAfternoon {
            get { return (bool)GetValue(IsAfternoonProperty); }
            set { SetValue(IsAfternoonProperty, value); }
        }

        public static readonly DependencyProperty IsAfternoonProperty =
            DependencyProperty.Register("IsAfternoon", typeof(bool), typeof(TimeSapnPicker), new PropertyMetadata(false));

        public int Hours {
            get { return (int)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }

        public static readonly DependencyProperty HoursProperty =
            DependencyProperty.Register("Hours", typeof(int), typeof(TimeSapnPicker), new PropertyMetadata(0));


        public int Minutes {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }

        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register("Minutes", typeof(int), typeof(TimeSapnPicker), new PropertyMetadata(0));

        public int Seconds {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        public static readonly DependencyProperty SecondsProperty =
            DependencyProperty.Register("Seconds", typeof(int), typeof(TimeSapnPicker), new PropertyMetadata(0));


        static TimeSapnPicker() {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSapnPicker), new FrameworkPropertyMetadata(typeof(TimeSapnPicker)));
            FrameworkElement.FocusableProperty.OverrideMetadata(typeof(TimeSapnPicker), new FrameworkPropertyMetadata(false));
            ValueProperty = DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeSapnPicker), new PropertyMetadata(DateTime.Now.TimeOfDay));
        }

        public TimeSapnPicker() {
            this.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler(HandleMouseLeftButtonDown));
        }

        private void HandleMouseLeftButtonDown(object sender, RoutedEventArgs e) {
            if(e.OriginalSource is TextBlock) {

            } else if(e.Source is TextBox) {

            }
            System.Diagnostics.Debug.WriteLine(e.OriginalSource);
        }

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

        }

        const string TAG_HOURS = "Hours";
        const string TAG_MINUTES = "Minutes";
        const string TAG_SECONDS = "Seconds";

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            this.m_hoursPopup = this.GetTemplateChild("PART_HoursPopup") as Popup;

            this.m_12TimeSystem1 = this.GetTemplateChild("PART_12TimeSystem1") as StackPanel;
            this.m_12TimeSystem2 = this.GetTemplateChild("PART_12TimeSystem2") as StackPanel;
            this.m_24TimeSystem1 = this.GetTemplateChild("PART_24TimeSystem1") as StackPanel;
            this.m_24TimeSystem2 = this.GetTemplateChild("PART_24TimeSystem2") as StackPanel;

            this.m_hoursTextBox = this.GetTemplateChild("PART_HoursTextBox") as TextBox;
            this.m_minutesTextBox = this.GetTemplateChild("PART_MinutesTextBox") as TextBox;
            this.m_secondsTextBox = this.GetTemplateChild("PART_SecondsTextBox") as TextBox;
            this.m_hoursTextBox.Tag = TAG_HOURS;
            this.m_minutesTextBox.Tag = TAG_MINUTES;
            this.m_secondsTextBox.Tag = TAG_SECONDS;
            StackPanel panel = this.m_12TimeSystem1;
            for(int i = 0;i < 24;i++) {
                switch(i) {
                    case 6:
                        panel = this.m_12TimeSystem2;
                        break;
                    case 12:
                        panel = this.m_24TimeSystem1;
                        break;
                    case 18:
                        panel = this.m_24TimeSystem2;
                        break;
                    default:
                        break;
                }
                panel.Children.Add(new TextBlock() { Text = i.ToString("00"), DataContext = i, Tag = TAG_HOURS });
            }


        }
    }
}
