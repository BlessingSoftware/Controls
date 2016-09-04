/*
 * forked from http://datetimepickerwpf.codeplex.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace BlessingSoftware.Controls {
    public enum DateTimePickerFormat { Long, Short, Time, DateTime, Custom }

    [DefaultBindingProperty("Value")]
    [TemplatePart(Name = "PART_CheckBox", Type = typeof(CheckBox))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_TextBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_Calendar", Type = typeof(Calendar))]
    public class Dameer :Control {
        private CheckBox m_checkBox;
        internal TextBox m_textBox;
        private TextBlock m_textBlock;
        private Popup m_popUp;
        private Calendar m_calendar;
        private BlockManager m_blockManager;
        internal const string _defaultFormat = "MM/dd/yyyy hh:mm:ss tt";

        #region DependencyProperties

        public static readonly DependencyProperty CustomFormatProperty;

        [Category("Dameer")]
        public string CustomFormat {
            get { return (string)GetValue(CustomFormatProperty); }
            set { SetValue(CustomFormatProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty;

        [Category("Dameer")]
        public DateTimePickerFormat Format {
            get { return (DateTimePickerFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty;
        [Category("Dameer")]
        public bool? IsChecked {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty ShowCheckBoxProperty;

        [Category("Dameer")]
        public bool ShowCheckBox {
            get { return (bool)GetValue(ShowCheckBoxProperty); }
            set {
                SetValue(ShowCheckBoxProperty, value);
                if(value) {
                    this.IsChecked = true;
                }
            }
        }

        public static readonly DependencyProperty ShowDropDownProperty;

        [Category("Dameer")]
        public bool ShowDropDown {
            get { return (bool)GetValue(ShowDropDownProperty); }
            set { SetValue(ShowDropDownProperty, value); }
        }

        [Category("Dameer")]
        private string FormatString {
            get {
                switch(this.Format) {
                    case DateTimePickerFormat.Long:
                        return "dddd, MMMM dd, yyyy";
                    case DateTimePickerFormat.Short:
                        return "M/d/yyyy";
                    case DateTimePickerFormat.Time:
                        return "h:mm:ss tt";
                    case DateTimePickerFormat.DateTime:
                        return "MM/dd/yyyy hh:mm:ss";
                    case DateTimePickerFormat.Custom:
                        if(string.IsNullOrEmpty(this.CustomFormat))
                            return Dameer._defaultFormat;
                        else
                            return this.CustomFormat;
                    default:
                        return Dameer._defaultFormat;
                }
            }
        }

        [Category("Dameer")]
        public DateTime? Value {
            get {
                if(!this.IsChecked.HasValue) return null;
                return (DateTime?)GetValue(ValueProperty);
            }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime?), typeof(Dameer), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Dameer.OnValueChanged), new CoerceValueCallback(Dameer.CoerceValue), true, System.Windows.Data.UpdateSourceTrigger.PropertyChanged));

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            if(e.Property == Dameer.ValueProperty) {
                (d as Dameer).InvalidateVisual();
            } else if(e.Property == Dameer.FormatProperty) {
                Dameer dameer = (Dameer)d;
                dameer.m_blockManager = new BlockManager(dameer, dameer.FormatString);
            }
        }

        static object CoerceValue(DependencyObject d, object value) {
            return value;
        }

        internal DateTime InternalValue {
            get {
                DateTime? value = this.Value;
                if(value.HasValue) return value.Value;
                return DateTime.MinValue;
            }
        }
        #endregion
        static Dameer() {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Dameer), new FrameworkPropertyMetadata(typeof(Dameer)));

            CustomFormatProperty = DependencyProperty.Register("CustomFormat", typeof(string), typeof(Dameer), new PropertyMetadata(_defaultFormat));
            FormatProperty = DependencyProperty.Register("Format", typeof(DateTimePickerFormat), typeof(Dameer), new PropertyMetadata(DateTimePickerFormat.DateTime, new PropertyChangedCallback(Dameer.OnValueChanged)));

            IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(Dameer), new PropertyMetadata(false));

            ShowDropDownProperty = DependencyProperty.Register("ShowDropDown", typeof(bool), typeof(Dameer), new PropertyMetadata(true));
            ShowCheckBoxProperty = DependencyProperty.Register("ShowCheckBox", typeof(bool), typeof(Dameer), new PropertyMetadata(true));

        }

        public Dameer() {
            Debug.WriteLine("Dameer");
        }

        #region override methods

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            this.m_checkBox = (CheckBox)this.Template.FindName("PART_CheckBox", this);
            this.m_textBox = (TextBox)this.Template.FindName("PART_TextBox", this);
            this.m_textBlock = (TextBlock)this.Template.FindName("PART_TextBlock", this);
            this.m_popUp = (Popup)this.Template.FindName("PART_Popup", this);
            this.m_calendar = (Calendar)this.Template.FindName("PART_Calendar", this);

            if(this.m_textBox != null) {
                this.m_textBox.GotFocus += new RoutedEventHandler(m_textBox_GotFocus);
                this.m_textBox.PreviewMouseUp += new MouseButtonEventHandler(m_textBox_PreviewMouseUp);
                this.m_textBox.PreviewKeyDown += new KeyEventHandler(m_textBox_PreviewKeyDown);
                this.m_textBlock.MouseLeftButtonDown += new MouseButtonEventHandler(m_textBlock_MouseLeftButtonDown);
            }
            if(this.m_calendar != null) {
                this.m_calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);
            }

            this.m_blockManager = new BlockManager(this, this.FormatString);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            Debug.WriteLine("Dameer_MouseWheel");
            this.m_blockManager.Change(((e.Delta < 0) ? -1 : 1), true);
            base.OnMouseWheel(e);
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            this.m_blockManager.Render();
        }

        public override string ToString() {
            return this.InternalValue.ToString();
        }

        #endregion

        void m_textBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Debug.WriteLine("_button_Click");
            this.m_popUp.IsOpen = !(this.m_popUp.IsOpen);
        }

        void m_textBox_GotFocus(object sender, System.Windows.RoutedEventArgs e) {
            Debug.WriteLine("m_textBox_GotFocus");
            this.m_blockManager.ReSelect();
        }

        void m_textBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Debug.WriteLine("m_textBox_PreviewMouseUp");
            this.m_blockManager.ReSelect();
        }

        void m_textBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            Debug.WriteLine("m_textBox_PreviewKeyDown");
            byte b = (byte)e.Key;

            if(e.Key == System.Windows.Input.Key.Left)
                this.m_blockManager.Left();
            else if(e.Key == System.Windows.Input.Key.Right)
                this.m_blockManager.Right();
            else if(e.Key == System.Windows.Input.Key.Up)
                this.m_blockManager.Change(1, true);
            else if(e.Key == System.Windows.Input.Key.Down)
                this.m_blockManager.Change(-1, true);
            if(b >= 34 && b <= 43)
                this.m_blockManager.ChangeValue(b - 34);
            if(!(e.Key == System.Windows.Input.Key.Tab))
                e.Handled = true;
        }

        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
            this.IsChecked = true;
            this.m_popUp.IsOpen = false;
            DateTime selectedDate = (DateTime)e.AddedItems[0];
            this.Value = selectedDate.Add(new TimeSpan(this.InternalValue.Hour, this.InternalValue.Minute, this.InternalValue.Second));
            //this.Value = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, this.InternalValue.Hour, this.InternalValue.Minute, this.InternalValue.Second);
            //this.m_blockManager.Render();
            this.InvalidateVisual();
        }

    }

    internal class BlockManager {
        internal Dameer m_owner;
        private List<Block> m_blocks;
        private string m_format;
        private Block m_selectedBlock;
        private int m_selectedIndex;
        public event EventHandler NeglectProposed;
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

        public BlockManager(Dameer dameer, string format) {
            Debug.WriteLine("BlockManager");
            this.m_owner = dameer;
            this.m_format = format;
            this.m_owner.LostFocus += new RoutedEventHandler(_dameer_LostFocus);
            this.m_blocks = new List<Block>();
            this.InitBlocks();
        }

        private void InitBlocks() {
            Debug.WriteLine("InitBlocks");
            foreach(string f in this.m_supportedFormats)
                this.m_blocks.AddRange(this.GetBlocks(f));
            this.m_blocks = this.m_blocks.OrderBy((a) => a.Index).ToList();
            this.m_selectedBlock = this.m_blocks[0];
            this.Render();
        }

        internal void Render() {
            //Debug.WriteLine("BlockManager.Render");
            int accum = 0;
            StringBuilder sb = new StringBuilder(this.m_format);
            foreach(Block b in this.m_blocks)
                b.Render(ref accum, sb);
            this.m_format = sb.ToString();
            if(this.m_owner.m_textBox != null) {
                this.m_owner.m_textBox.Text = this.m_format;
            }
            this.Select(this.m_selectedBlock);
        }

        private List<Block> GetBlocks(string pattern) {
            Debug.WriteLine("GetBlocks");
            List<Block> bCol = new List<Block>();

            int index = -1;
            while((index = this.m_format.IndexOf(pattern, ++index)) > -1)
                bCol.Add(new Block(this, pattern, index));
            this.m_format = this.m_format.Replace(pattern, (0).ToString().PadRight(pattern.Length, '0'));
            return bCol;
        }

        internal void ChangeValue(int p) {
            //Debug.WriteLine("ChangeValue");
            this.m_selectedBlock.Proposed = p;
            this.Change(this.m_selectedBlock.Proposed, false);
        }

        internal void Change(int value, bool upDown) {
            //Debug.WriteLine("Change");
            this.m_owner.Value = this.m_selectedBlock.Change(this.m_owner.InternalValue, value, upDown);
            if(upDown)
                this.OnNeglectProposed();
            this.Render();
        }

        internal void Right() {
            Debug.WriteLine("Right");
            if(this.m_selectedIndex + 1 < this.m_blocks.Count)
                this.Select(this.m_selectedIndex + 1);
        }

        internal void Left() {
            Debug.WriteLine("Left");
            if(this.m_selectedIndex > 0)
                this.Select(this.m_selectedIndex - 1);
        }

        private void _dameer_LostFocus(object sender, RoutedEventArgs e) {
            this.OnNeglectProposed();
        }

        protected virtual void OnNeglectProposed() {
            this.NeglectProposed?.Invoke(this, EventArgs.Empty);
            //Debug.WriteLine("OnNeglectProposed");
            //EventHandler temp = this.NeglectProposed;
            //if(temp != null) {
            //    temp(this, EventArgs.Empty);
            //}
        }

        internal void ReSelect() {
            Debug.WriteLine("ReSelect");
            foreach(Block b in this.m_blocks)
                if((b.Index <= this.m_owner.m_textBox.SelectionStart) && ((b.Index + b.Length) >= this.m_owner.m_textBox.SelectionStart)) { this.Select(b); return; }
            Block bb = this.m_blocks.Where((a) => a.Index < this.m_owner.m_textBox.SelectionStart).LastOrDefault();
            if(bb == null) this.Select(0);
            else this.Select(bb);
        }

        private void Select(int blockIndex) {
            //Debug.WriteLine("Select(int blockIndex)");
            if(this.m_blocks.Count > blockIndex)
                this.Select(this.m_blocks[blockIndex]);
        }

        private void Select(Block block) {
            //Debug.WriteLine("Select(Block block)");
            if(!(this.m_selectedBlock == block))
                this.OnNeglectProposed();
            this.m_selectedIndex = this.m_blocks.IndexOf(block);
            this.m_selectedBlock = block;
            if(this.m_owner.m_textBox != null) {
                this.m_owner.m_textBox.Select(block.Index, block.Length);
            }
        }
    }

    internal class Block {
        private BlockManager m_blockManager;
        internal string Pattern { get; set; }
        internal int Index { get; set; }
        private int m_length;
        internal int Length {
            get {
                //Debug.WriteLine("Length Get");
                return this.m_length;
            }
            set {
                //Debug.WriteLine("Length Set");
                this.m_length = value;
            }
        }
        private int _maxLength;
        private string _proposed;
        internal int Proposed {
            get {
                Debug.WriteLine(string.Format("Proposed Get, {0}, {1}", this._proposed, this.Length));
                string p = this._proposed;
                return int.Parse(p.PadLeft(this.Length, '0'));
            }
            set {
                Debug.WriteLine(string.Format("Proposed Set, {0}, {1}", this._proposed, this.Length));
                if(!(this._proposed == null) && this._proposed.Length >= this._maxLength)
                    this._proposed = value.ToString();
                else
                    this._proposed = string.Format("{0}{1}", this._proposed, value);
            }
        }

        public Block(BlockManager blockManager, string pattern, int index) {
            Debug.WriteLine("Block");
            this.m_blockManager = blockManager;
            this.m_blockManager.NeglectProposed += new EventHandler(m_blockManager_NeglectProposed);
            this.Pattern = pattern;
            this.Index = index;
            this.Length = this.Pattern.Length;
            this._maxLength = this.GetMaxLength(this.Pattern);
        }

        private int GetMaxLength(string p) {
            switch(p) {
                case "y":
                case "M":
                case "d":
                case "h":
                case "m":
                case "s":
                case "H":
                    return 2;
                case "yyy":
                    return 4;
                default:
                    return p.Length;
            }
        }

        private void m_blockManager_NeglectProposed(object sender, EventArgs e) {
            Debug.WriteLine("m_blockManager_NeglectProposed");
            this._proposed = null;
        }

        internal DateTime Change(DateTime dateTime, int value, bool upDown) {
            Debug.WriteLine("Change(DateTime dateTime, int value, bool upDown)");
            if(!upDown && !this.CanChange()) return dateTime;
            int y, m, d, h, n, s;
            y = dateTime.Year;
            m = dateTime.Month;
            d = dateTime.Day;
            h = dateTime.Hour;
            n = dateTime.Minute;
            s = dateTime.Second;

            if(this.Pattern.Contains("y"))
                y = ((upDown) ? dateTime.Year + value : value);
            else if(this.Pattern.Contains("M"))
                m = ((upDown) ? dateTime.Month + value : value);
            else if(this.Pattern.Contains("d"))
                d = ((upDown) ? dateTime.Day + value : value);
            else if(this.Pattern.Contains("h") || this.Pattern.Contains("H"))
                h = ((upDown) ? dateTime.Hour + value : value);
            else if(this.Pattern.Contains("m"))
                n = ((upDown) ? dateTime.Minute + value : value);
            else if(this.Pattern.Contains("s"))
                s = ((upDown) ? dateTime.Second + value : value);
            else if(this.Pattern.Contains("t"))
                h = ((h < 12) ? (h + 12) : (h - 12));

            if(y > 9999) y = 1;
            if(y < 1) y = 9999;
            if(m > 12) m = 1;
            if(m < 1) m = 12;
            if(d > DateTime.DaysInMonth(y, m)) d = 1;
            if(d < 1) d = DateTime.DaysInMonth(y, m);
            if(h > 23) h = 0;
            if(h < 0) h = 23;
            if(n > 59) n = 0;
            if(n < 0) n = 59;
            if(s > 59) s = 0;
            if(s < 0) s = 59;

            return new DateTime(y, m, d, h, n, s);
        }

        private bool CanChange() {
            switch(this.Pattern) {
                case "MMMM":
                case "dddd":
                case "MMM":
                case "ddd":
                case "g":
                    return false;
                default:
                    return true;
            }
        }

        public override string ToString() {
            return string.Format("{0}, {1}", this.Pattern, this.Index);
        }

        internal void Render(ref int accum, StringBuilder sb) {
            //Debug.WriteLine("Block.Render");
            this.Index += accum;

            string f = this.m_blockManager.m_owner.InternalValue.ToString(this.Pattern + ",").TrimEnd(',');
            sb.Remove(this.Index, this.Length);
            sb.Insert(this.Index, f);
            accum += f.Length - this.Length;

            this.Length = f.Length;
        }
    }
}
