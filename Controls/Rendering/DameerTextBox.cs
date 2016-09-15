using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BlessingSoftware.Controls.Rendering {
    public class DameerTextBox :TextBox {

        public enum DateTimeField {
            Year, Month, Day, Hour, Minute, Second, Milliseconds
        }

        [DebuggerDisplay("Text = {Text},Field = {Field},Range = ({StartOffset}, {EndOffset})")]
        protected internal class DameerBlock {
            public string Text { get; set; }
            public DateTimeField Field { get; set; }
            public int Index { get; set; }
            public int Length { get; set; }
            public int StartOffset { get; set; }
            public int EndOffset { get; set; }
        }

        protected internal List<DameerBlock> m_blocks;
        protected internal int m_currentPosition;

        private string[] m_supportedFormats = new string[] {
                "yyyy", "MMMM", "dddd",
                "yyy", "MMM", "ddd",
                "yy", "MM", "dd",
                "y", "M", "d",
                "HH", "H", "hh", "h",
                "mm", "m",
                "ss", "s",
                "tt", "t",
                "fff", "ff", "f",
                "K", "g"};

        private DameerBlock _PartFormat(string format, char ch, ref int pos) {
            int temp = pos;
            DameerBlock block = new DameerBlock() {
                StartOffset = pos,
            };
            while(format[++pos] == ch) {

            }
            block.EndOffset = pos--;
            block.Text = format.Substring(temp, pos - temp + 1);
            return block;
        }

        private DameerBlock _PartNextChar(string format, char ch, ref int pos) {
            int temp = pos;
            DameerBlock block = new DameerBlock() {
                StartOffset = pos,
            };
            if(format[++pos] == ch) {
                block.EndOffset = pos;
            } else {
                block.EndOffset = pos--;
            }
            block.Text = format.Substring(temp, pos - temp + 1);
            return block;
        }

        protected internal void ParseFormat(string format) {
            int length = format.Length, pos = 0;
            format += " ";
            char ch;
            DameerBlock block;
            this.m_blocks = new List<DameerBlock>();
            this.m_currentPosition = -1;
            while(pos < length) {
                ch = format[pos];
                switch(ch) {
                    case 'y':
                        block = this._PartFormat(format, ch, ref pos);
                        block.Field = DateTimeField.Year;
                        this.m_blocks.Add(block);
                        break;
                    case 'M':
                        block = this._PartFormat(format, ch, ref pos);
                        block.Field = DateTimeField.Month;
                        this.m_blocks.Add(block);
                        break;
                    case 'd':
                        block = this._PartFormat(format, ch, ref pos);
                        block.Field = DateTimeField.Day;
                        this.m_blocks.Add(block);
                        break;
                    case 't':
                        block = this._PartNextChar(format, ch, ref pos);
                        block.Field = DateTimeField.Day;
                        this.m_blocks.Add(block);
                        break;
                    case 'H':
                    case 'h':
                        block = this._PartNextChar(format, ch, ref pos);
                        block.Field = DateTimeField.Hour;
                        this.m_blocks.Add(block);
                        break;
                    case 'm':
                        block = this._PartNextChar(format, ch, ref pos);
                        block.Field = DateTimeField.Minute;
                        this.m_blocks.Add(block);
                        break;
                    case 's':
                        block = this._PartNextChar(format, ch, ref pos);
                        block.Field = DateTimeField.Second;
                        this.m_blocks.Add(block);
                        break;
                    default:
                        break;
                }
                pos++;
            }

        }

        protected internal void ChangeFormat(string format) {
            this.ParseFormat(format);

            this.ArrangeBlocks(this.SelectedValue);

            Debug.WriteLine(this.Text);

        }

        static DameerTextBox() {
            //FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DameerTextBox), new FrameworkPropertyMetadata(typeof(DameerTextBox)));
            FrameworkElement.CursorProperty.OverrideMetadata(typeof(Dameer), new FrameworkPropertyMetadata(Cursors.Hand));
            Control.BorderThicknessProperty.OverrideMetadata(typeof(Dameer), new FrameworkPropertyMetadata(new Thickness()));
            TextBoxBase.IsReadOnlyProperty.OverrideMetadata(typeof(Dameer), new FrameworkPropertyMetadata(true));
            TextBoxBase.IsReadOnlyCaretVisibleProperty.OverrideMetadata(typeof(Dameer), new FrameworkPropertyMetadata(false));

            SelectedValueChangedEvent = Dameer.SelectedValueChangedEvent.AddOwner(typeof(DameerTextBox));
        }

        public string Format {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
            Dameer.FormatProperty.AddOwner(typeof(DameerTextBox),
                new FrameworkPropertyMetadata(
                "MM/dd/yyyy hh:mm:ss",
                new PropertyChangedCallback(OnPropertyValueChanged)));

        public DateTime? SelectedValue {
            get { return (DateTime?)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueProperty = Dameer.SelectedValueProperty.AddOwner(typeof(DameerTextBox),
            new FrameworkPropertyMetadata(
                DateTime.Now,
                new PropertyChangedCallback(OnPropertyValueChanged)));

        static void OnPropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            if(e.Property == Dameer.SelectedValueProperty) {

                (d as DameerTextBox).OnSelectedValueChanged(e.OldValue as DateTime?, e.NewValue as DateTime?);
            } else if(e.Property == Dameer.FormatProperty) {
                var dameer = (DameerTextBox)d;
                dameer.ChangeFormat(e.NewValue as string);
            }
        }

        protected void OnSelectedValueChanged(DateTime? oldValue, DateTime? newValue) {
            Debug.WriteLine("SelectedValueChanged: DameerTextBox");

            this.ArrangeBlocks(newValue);

            base.RaiseEvent(new DateTimeChangedEventArgs(oldValue, newValue) {
                RoutedEvent = DameerTextBox.SelectedValueChangedEvent
            });
            base.InvalidateVisual();
        }

        public static readonly RoutedEvent SelectedValueChangedEvent;

        public event DateTimeChangedEventHandler SelectedValueChanged {
            add {
                base.AddHandler(DameerTextBox.SelectedValueChangedEvent, value, false);
            }
            remove {
                base.RemoveHandler(DameerTextBox.SelectedValueChangedEvent, value);
            }
        }


        public DameerTextBox() {

            RoutedUICommand[] commands = new RoutedUICommand[] {
                ComponentCommands.MoveDown,
                ComponentCommands.MoveUp,
                ComponentCommands.MoveLeft,
                ComponentCommands.MoveRight,
                ComponentCommands.MoveToHome,
                ComponentCommands.MoveToEnd
            };

            var bindings = base.CommandBindings;
            var keyBindings = base.InputBindings;
            foreach(var command in commands) {
                bindings.Add(new CommandBinding(command, this.OnMove));
                foreach(InputGesture gesture in command.InputGestures) {
                    if(gesture is KeyGesture) {
                        keyBindings.Add(new KeyBinding(command, (KeyGesture)gesture));
                    }
                }
            }
            //bindings.AddRange(
            //    new CommandBinding[] {
            //        new CommandBinding(ComponentCommands.MoveDown, OnMove),
            //        new CommandBinding(ComponentCommands.MoveUp, OnMove),
            //        new CommandBinding(ComponentCommands.MoveLeft, OnMove),
            //        new CommandBinding(ComponentCommands.MoveRight, OnMove),
            //        new CommandBinding(ComponentCommands.MoveToHome, OnMove),
            //        new CommandBinding(ComponentCommands.MoveToEnd, OnMove)
            //    });
            //bindings.Add(new CommandBinding(ComponentCommands.MoveDown, OnMove));
            //bindings.Add(new CommandBinding(ComponentCommands.MoveUp, OnMove));
            //bindings.Add(new CommandBinding(ComponentCommands.MoveLeft, OnMove));
            //bindings.Add(new CommandBinding(ComponentCommands.MoveRight, OnMove));
            //bindings.Add(new CommandBinding(ComponentCommands.MoveToHome, OnMove));
            //bindings.Add(new CommandBinding(ComponentCommands.MoveToEnd, OnMove));



            ChangeFormat(this.Format);

        }

        protected void OnMove(object sender, ExecutedRoutedEventArgs e) {
            if(!this.SelectedValue.HasValue) {
                return;
            }
            var cmd = e.Command;
            Debug.WriteLine($"OnMove: {cmd}, {(cmd as RoutedUICommand)?.Text}");

            DeltaValue(cmd);
            if(cmd == ComponentCommands.MoveToHome) {
                this.m_currentPosition = 0;
            } else if(cmd == ComponentCommands.MoveToEnd) {
                this.m_currentPosition = this.m_blocks.Count - 1;
            } else if(cmd == ComponentCommands.MoveLeft) {
                this.m_currentPosition--;
                this.m_currentPosition = Math.Max(0, this.m_currentPosition);
            } else if(cmd == ComponentCommands.MoveRight) {
                this.m_currentPosition++;
                this.m_currentPosition = Math.Min(this.m_blocks.Count - 1, this.m_currentPosition);
            }
            this.Cursor = Cursors.ScrollNS;
            this.Select(this.m_blocks[this.m_currentPosition]);
        }

        private void DeltaValue(ICommand cmd) {
            if(this.m_currentPosition > -1 && this.m_currentPosition < this.m_blocks.Count) {
                DameerBlock block = this.m_blocks[this.m_currentPosition];

                int delta;
                if(cmd == ComponentCommands.MoveUp) {
                    delta = 1;
                } else if(cmd == ComponentCommands.MoveDown) {
                    delta = -1;
                } else {
                    delta = 0;
                    return;
                }
                if(delta != 0) {
                    switch(block.Field) {
                        case DateTimeField.Year:
                            this.SelectedValue = this.SelectedValue.Value.AddYears(delta);
                            break;
                        case DateTimeField.Month:
                            this.SelectedValue = this.SelectedValue.Value.AddMonths(delta);
                            break;
                        case DateTimeField.Day:
                            this.SelectedValue = this.SelectedValue.Value.AddDays(delta);
                            break;
                        case DateTimeField.Hour:
                            this.SelectedValue = this.SelectedValue.Value.AddHours(delta);
                            break;
                        case DateTimeField.Minute:
                            this.SelectedValue = this.SelectedValue.Value.AddMinutes(delta);
                            break;
                        case DateTimeField.Second:
                            this.SelectedValue = this.SelectedValue.Value.AddSeconds(delta);
                            break;
                        case DateTimeField.Milliseconds:
                            this.SelectedValue = this.SelectedValue.Value.AddMilliseconds(delta);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected void Select(DameerBlock block) {
            var val = this.SelectedValue;
            if(val.HasValue) {
                base.Select(block.Index, block.Length);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e) {
            this.m_currentPosition = -1;
            this.Cursor = Cursors.Hand;
            base.OnLostFocus(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {

            int selectionStart = base.SelectionStart;
            Debug.WriteLine(selectionStart);
            DameerBlock block;
            for(int i = 0;i < this.m_blocks.Count;i++) {
                block = this.m_blocks[i];
                if(block.Index > selectionStart) {
                    return;
                } else if(block.Index + block.Length > selectionStart) {
                    this.m_currentPosition = i;
                    this.Cursor = Cursors.ScrollNS;
                    this.Select(block);
                    Debug.WriteLine($"Select: {i}");
                    return;
                }

            }

            base.OnPreviewMouseLeftButtonUp(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            if(this.m_currentPosition != -1) {
                Debug.WriteLine(e.Delta);
                DeltaValue(e.Delta > 0 ? ComponentCommands.MoveUp : ComponentCommands.MoveDown);
                this.Select(this.m_blocks[this.m_currentPosition]);
                e.Handled = true;
            }
            base.OnMouseWheel(e);
        }

        protected void ArrangeBlocks(DateTime? value) {
            var format = this.Format;
            if(value.HasValue) {

                var rawValue = value.Value;
                foreach(var block in this.m_blocks) {
                    if(block.StartOffset == 0) {
                        block.Index = 0;
                    } else {
                        block.Index = rawValue.ToString("," + format.Substring(0, block.StartOffset)).Length - 1;
                    }
                    //block.Length = rawValue.ToString("," + format.Substring(block.StartOffset, block.EndOffset - block.StartOffset)).Length - 1;
                    block.Length = rawValue.ToString("," + block.Text).Length - 1;
                }
                this.Text = rawValue.ToString(format);
            }
        }

    }
}
