using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using BlessingSoftware.Utils;

namespace BlessingSoftware.Controls.Rendering {
    public class HexColumnHeader :FrameworkElement, IScrollable {
        #region DependencyProperty

        public static readonly DependencyProperty ColumnCountProperty =
            HexArea.ColumnCountProperty.AddOwner(typeof(HexColumnHeader));

        /// <summary>
        /// 列数，默认值为16
        /// </summary>
        [Category("Layout")]
        public int ColumnCount {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        public static readonly DependencyProperty ColumnWidthProperty =
            HexArea.ColumnWidthProperty.AddOwner(typeof(HexColumnHeader));

        /// <summary>
        /// 列宽
        /// </summary>
        public double ColumnWidth {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty ColumnHeightProperty =
            HexArea.ColumnHeightProperty.AddOwner(typeof(HexColumnHeader));

        /// <summary>
        /// 获取或设置标题栏高度
        /// </summary>
        public double ColumnHeight {
            get { return (double)GetValue(ColumnHeightProperty); }
            set { SetValue(ColumnHeightProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty;

        //		public static readonly DependencyProperty FontStyleProperty;
        //
        //		public static readonly DependencyProperty FontWeightProperty;
        //
        //		public static readonly DependencyProperty FontStretchProperty;

        public static readonly DependencyProperty FontSizeProperty;

        public static readonly DependencyProperty ForegroundProperty;

        public static readonly DependencyProperty BackgroundProperty;

        [Localizability(LocalizationCategory.Font)]
        public FontFamily FontFamily {
            get {
                return (FontFamily)base.GetValue(HexColumnHeader.FontFamilyProperty);
            }
            set {
                base.SetValue(HexColumnHeader.FontFamilyProperty, value);
            }
        }

        //		public FontStyle FontStyle
        //		{
        //			get
        //			{
        //				return (FontStyle)base.GetValue(HexColumnHeader.FontStyleProperty);
        //			}
        //			set
        //			{
        //				base.SetValue(HexColumnHeader.FontStyleProperty, value);
        //			}
        //		}
        //
        //		public FontWeight FontWeight
        //		{
        //			get
        //			{
        //				return (FontWeight)base.GetValue(HexColumnHeader.FontWeightProperty);
        //			}
        //			set
        //			{
        //				base.SetValue(HexColumnHeader.FontWeightProperty, value);
        //			}
        //		}
        //
        //		public FontStretch FontStretch
        //		{
        //			get
        //			{
        //				return (FontStretch)base.GetValue(HexColumnHeader.FontStretchProperty);
        //			}
        //			set
        //			{
        //				base.SetValue(HexColumnHeader.FontStretchProperty, value);
        //			}
        //		}

        [TypeConverter(typeof(FontSizeConverter)), Localizability(LocalizationCategory.None)]
        public double FontSize {
            get {
                return (double)base.GetValue(HexColumnHeader.FontSizeProperty);
            }
            set {
                base.SetValue(HexColumnHeader.FontSizeProperty, value);
            }
        }

        public Brush Foreground {
            get {
                return (Brush)base.GetValue(HexColumnHeader.ForegroundProperty);
            }
            set {
                base.SetValue(HexColumnHeader.ForegroundProperty, value);
            }
        }

        public Brush Background {
            get {
                return (Brush)base.GetValue(HexColumnHeader.BackgroundProperty);
            }
            set {
                base.SetValue(HexColumnHeader.BackgroundProperty, value);
            }
        }

        #endregion

        private VisualCollection _children;

        static HexColumnHeader() {
            HexColumnHeader.FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(HexColumnHeader));
            //			HexColumnHeader.FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(HexColumnHeader));
            //			HexColumnHeader.FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(HexColumnHeader));
            //			HexColumnHeader.FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(HexColumnHeader));
            HexColumnHeader.FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(HexColumnHeader));
            HexColumnHeader.ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(HexColumnHeader));

            HexColumnHeader.BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(HexColumnHeader),
                                                                                   new FrameworkPropertyMetadata(SystemColors.ControlBrush));

        }

        public HexColumnHeader() {
            offset = 0;

            _children = new VisualCollection(this);
            _child = new DrawingVisual();

            _children.Add(_child);
        }

        private DrawingVisual _child;

        public Orientation Orientation {
            get {
                return Orientation.Horizontal;
            }
        }

        int offset;
        public int Offset {
            get { return offset; }
            set {
                if(value < 0)
                    value = 0;
                if(offset != value) {
                    offset = value;
                    InvalidateMeasure();
                }
            }
        }

        #region Override Methods

        protected override Size MeasureOverride(Size availableSize) {
            //			base.MeasureOverride(availableSize);
            FormattedText ft = GetFormattedHeader();
            return new Size(Math.Max(ft.Width, availableSize.Width), this.ColumnHeight);
        }
        protected override Size ArrangeOverride(Size finalSize) {
            finalSize = base.ArrangeOverride(finalSize);
            FormattedText ft = GetFormattedHeader();
            //ft.Width *3.0d/this.ColumnCount;
            double x = -offset * this.ColumnWidth;

            //			if ((_child.ContentBounds.Width+x )>=0) {
            //				System.Diagnostics.Debug.WriteLine(_child.ContentBounds.Width - this.RenderSize.Width);
            //
            //				System.Diagnostics.Debug.WriteLine(x);
            //			}
            double y = 0.5d * (finalSize.Height - _child.ContentBounds.Height);
            _child.Offset = new Vector(x, y);

            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(this.Background, null, new Rect(this.RenderSize));

            var dc = _child.RenderOpen();
            {
                FormattedText ft = GetFormattedHeader();
                dc.DrawText(ft, new Point());
                dc.Close();
            }
            double x = -offset * this.ColumnWidth;
            double y = 0.5d * (this.RenderSize.Height - _child.ContentBounds.Height);
            _child.Offset = new Vector(x, y);
        }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount {
            get { return _children.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index) {
            if(index < 0 || index >= _children.Count) {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }

        #endregion

        string GetHeaderString() {
            byte[] temp = new byte[ColumnCount];
            for(byte i = 0;i < temp.Length;i++) {
                temp[i] = i;
            }
            return temp.JoinHex(' ', 0, ColumnCount);
        }

        FormattedText GetFormattedHeader() {
            return new FormattedText(
                GetHeaderString(),
                CultureInfo.GetCultureInfo("en-US"),
                base.FlowDirection,
                new Typeface(this.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                this.FontSize,
                this.Foreground
            );
            //FontStretches.Normal
        }

    }
}
