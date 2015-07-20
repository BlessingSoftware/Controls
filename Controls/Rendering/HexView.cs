using BlessingSoftware.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlessingSoftware.Controls.Rendering
{
    public class HexView : FrameworkElement, IScrollInfo
    {
        static CultureInfo s_hexCulture;

        public static readonly DependencyProperty BufferSizeProperty =
            DependencyProperty.Register("BufferSize", typeof(int), typeof(HexView), new PropertyMetadata(BufferedBytes.DEFAULT_Capacity, OnBufferSizeChanged));

        public int BufferSize
        {
            get { return (int)GetValue(BufferSizeProperty); }
            set { SetValue(BufferSizeProperty, value); }
        }

        static void OnBufferSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HexView).OnBufferSizeChanged((int)e.NewValue);
        }

        void OnBufferSizeChanged(int newValue)
        {
            _buffer.Capacity = newValue;
        }

        BufferedBytes _buffer;

        public BufferedBytes Buffer { get { return _buffer; } set { _buffer = value; } }

        public Stream BaseStream
        {
            get
            {
                return _buffer.BaseStream;
            }
            set
            {
                _buffer.ChangeStream(value);
            }
        }

        static HexView()
        {
            HexView.BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(HexView), new FrameworkPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.None));
            HexView.ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(HexView), new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits));
            HexView.FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(HexView), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits));
            HexView.FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(HexView), new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits));
            HexView.FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(HexView), new FrameworkPropertyMetadata(TextElement.FontStretchProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits));
            HexView.FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(HexView), new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle, FrameworkPropertyMetadataOptions.Inherits));
            HexView.FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(HexView), new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight, FrameworkPropertyMetadataOptions.Inherits));

            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(HexView), new FrameworkPropertyMetadata(typeof(HexView)));

            s_hexCulture = CultureInfo.GetCultureInfo("en-US");
        }
        public HexView()
        {
            _buffer = new BufferedBytes(this.BufferSize);

        }

        public HexView(Stream stream)
        {
            _buffer = new BufferedBytes(this.BufferSize, stream);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //TODO: LineCount
            Size sz = new Size(
                Math.Max(availableSize.Width, this.DefaultLineWidth),
                defaultLineHeight * 129.0d
            );
            SetScrollData(availableSize, sz, scrollOffset);
            if (availableSize.Width == double.PositiveInfinity)
                availableSize.Width = SystemParameters.PrimaryScreenWidth;
            if (availableSize.Height == double.PositiveInfinity)
                availableSize.Height = SystemParameters.PrimaryScreenHeight;
            return availableSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawHex(drawingContext);
        }
        void DrawHex(DrawingContext drawingContext)
        {

            Typeface tf = this.GetTypeface();
            CalculateLineLayout();
            Point pos = new Point(-this.scrollOffset.X, -this.scrollOffset.Y);

            //System.Diagnostics.Debug.WriteLine(pos);

            //TODO: Length
            for (int i = 0; i < 0x80; i++)
            {
                if (pos.Y < -0.1d)
                {
                    pos.Y += defaultLineHeight;
                    continue;
                }

                drawingContext.DrawText(
                    new FormattedText(
                        _buffer.GetBytesLine(i).JoinHex(' '),
                                      s_hexCulture,
                                      base.FlowDirection,
                                      tf,
                                      this.FontSize,
                                      this.Foreground),
                    pos);
                pos.Y += defaultLineHeight;
                if (pos.Y > this.Height)
                    break;
            }
        }


        #region DependencyProperties

        public static readonly DependencyProperty FontFamilyProperty;

        public static readonly DependencyProperty FontStyleProperty;

        public static readonly DependencyProperty FontWeightProperty;

        public static readonly DependencyProperty FontStretchProperty;

        public static readonly DependencyProperty FontSizeProperty;

        public static readonly DependencyProperty ForegroundProperty;

        public static readonly DependencyProperty BackgroundProperty;

        [Localizability(LocalizationCategory.Font)]
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)base.GetValue(HexArea.FontFamilyProperty);
            }
            set
            {
                base.SetValue(HexArea.FontFamilyProperty, value);
            }
        }
        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)base.GetValue(HexArea.FontStyleProperty);
            }
            set
            {
                base.SetValue(HexArea.FontStyleProperty, value);
            }
        }
        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)base.GetValue(HexArea.FontWeightProperty);
            }
            set
            {
                base.SetValue(HexArea.FontWeightProperty, value);
            }
        }
        public FontStretch FontStretch
        {
            get
            {
                return (FontStretch)base.GetValue(HexArea.FontStretchProperty);
            }
            set
            {
                base.SetValue(HexArea.FontStretchProperty, value);
            }
        }
        [TypeConverter(typeof(FontSizeConverter)), Localizability(LocalizationCategory.None)]
        public double FontSize
        {
            get
            {
                return (double)base.GetValue(HexArea.FontSizeProperty);
            }
            set
            {
                base.SetValue(HexArea.FontSizeProperty, value);
            }
        }
        public Brush Foreground
        {
            get
            {
                return (Brush)base.GetValue(HexArea.ForegroundProperty);
            }
            set
            {
                base.SetValue(HexArea.ForegroundProperty, value);
            }
        }
        public Brush Background
        {
            get
            {
                return (Brush)base.GetValue(HexArea.BackgroundProperty);
            }
            set
            {
                base.SetValue(HexArea.BackgroundProperty, value);
            }
        }
        #endregion

        Typeface GetTypeface()
        {
            return new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch);
        }

        #region IScrollInfo implementation

        /// <summary>
        /// Size of the document, in pixels.
        /// </summary>
        Size scrollExtent;

        /// <summary>
        /// Offset of the scroll position.
        /// </summary>
        Vector scrollOffset;

        /// <summary>
        /// Size of the viewport.
        /// </summary>
        Size scrollViewport;

        //void ClearScrollData()
        //{
        //    SetScrollData(new Size(), new Size(), new Vector());
        //}

        bool SetScrollData(Size viewport, Size extent, Vector offset)
        {
            if (!(viewport.IsClose(this.scrollViewport)
                  && extent.IsClose(this.scrollExtent)
                  && offset.IsClose(this.scrollOffset)))
            {
                this.scrollViewport = viewport;
                this.scrollExtent = extent;
                SetScrollOffset(offset);
                this.OnScrollChange();
                return true;
            }
            return false;
        }

        void OnScrollChange()
        {
            ScrollViewer scrollOwner = ((IScrollInfo)this).ScrollOwner;
            if (scrollOwner != null)
            {
                scrollOwner.InvalidateScrollInfo();
            }
        }

        bool canVerticallyScroll;
        bool IScrollInfo.CanVerticallyScroll
        {
            get { return canVerticallyScroll; }
            set
            {
                if (canVerticallyScroll != value)
                {
                    canVerticallyScroll = value;
                    InvalidateMeasure(DispatcherPriority.Normal);
                }
            }
        }
        bool canHorizontallyScroll;
        bool IScrollInfo.CanHorizontallyScroll
        {
            get { return canHorizontallyScroll; }
            set
            {
                if (canHorizontallyScroll != value)
                {
                    canHorizontallyScroll = value;
                    InvalidateMeasure(DispatcherPriority.Normal);
                }
            }
        }

        public double ExtentWidth
        {
            get
            {
                return scrollExtent.Width;
            }
        }

        public double ExtentHeight
        {
            get
            {
                return scrollExtent.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                return scrollViewport.Width;
            }
        }

        public double ViewportHeight
        {
            get
            {
                return scrollViewport.Height;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return scrollOffset.X;
            }
        }

        public double VerticalOffset
        {
            get
            {
                return scrollOffset.Y;
            }
        }
        ScrollViewer _scrollOwner;
        public ScrollViewer ScrollOwner
        {
            get
            {
                return _scrollOwner;
            }
            set
            {
                _scrollOwner = value;
            }
        }

        public void LineUp()
        {
            (this as IScrollInfo).SetVerticalOffset(scrollOffset.Y - defaultLineHeight);
        }

        public void LineDown()
        {
            (this as IScrollInfo).SetVerticalOffset(scrollOffset.Y + defaultLineHeight);
        }

        public void LineLeft()
        {
            (this as IScrollInfo).SetHorizontalOffset(scrollOffset.X - 10.0d);
        }

        public void LineRight()
        {
            (this as IScrollInfo).SetHorizontalOffset(scrollOffset.X + 10.0d);
        }

        public void PageUp()
        {
            (this as IScrollInfo).SetVerticalOffset(
                scrollOffset.Y + defaultLineHeight - this.scrollViewport.Height
            );
        }

        public void PageDown()
        {
            (this as IScrollInfo).SetVerticalOffset(
                scrollOffset.Y - defaultLineHeight + this.scrollViewport.Height
            );
        }

        public void PageLeft()
        {
            (this as IScrollInfo).SetHorizontalOffset(0.0d);
        }

        public void PageRight()
        {
            (this as IScrollInfo).SetHorizontalOffset(double.MaxValue);
        }

        void IScrollInfo.MouseWheelUp()
        {
            ((IScrollInfo)this).SetVerticalOffset(
                scrollOffset.Y - (SystemParameters.WheelScrollLines * defaultLineHeight));
            OnScrollChange();
        }

        void IScrollInfo.MouseWheelDown()
        {
            ((IScrollInfo)this).SetVerticalOffset(
                scrollOffset.Y + (SystemParameters.WheelScrollLines * defaultLineHeight));
            OnScrollChange();
        }

        void IScrollInfo.MouseWheelLeft()
        {
            //((IScrollInfo)this).SetHorizontalOffset(
            //    scrollOffset.X - (SystemParameters.WheelScrollLines * wideSpaceWidth));
        }

        void IScrollInfo.MouseWheelRight()
        {
            //((IScrollInfo)this).SetHorizontalOffset(
            //    scrollOffset.X + (SystemParameters.WheelScrollLines * wideSpaceWidth));
        }

        /// <summary>
        /// Occurs when the scroll offset has changed.
        /// </summary>
        public event EventHandler ScrollOffsetChanged;

        internal void SetScrollOffset(Vector vector)
        {
            if (!canHorizontallyScroll)
                vector.X = 0;
            if (!canVerticallyScroll)
                vector.Y = 0;

            vector.X = Math.Min(
                vector.X,
                scrollExtent.Width - scrollViewport.Width
            );
            double t = Math.Round
                (
                    (scrollExtent.Height - scrollViewport.Height) / DefaultLineHeight,
                    0
                );

            vector.Y = Math.Min(vector.Y, t * defaultLineHeight);

            if (!scrollOffset.IsClose(vector))
            {
                scrollOffset = vector;
                this.InvalidateVisual();
                if (ScrollOffsetChanged != null)
                    ScrollOffsetChanged(this, EventArgs.Empty);
            }
        }

        static double ValidateVisualOffset(double offset)
        {
            if (double.IsNaN(offset))
                throw new ArgumentException("offset must not be NaN");
            if (offset < 0)
                return 0;
            else
                return offset;
        }

        void IScrollInfo.SetHorizontalOffset(double offset)
        {
            offset = ValidateVisualOffset(offset);
            if (!scrollOffset.X.IsClose(offset))
            {
                SetScrollOffset(new Vector(offset, scrollOffset.Y));
                InvalidateVisual();
                //				textLayer.InvalidateVisual();
            }
        }

        void IScrollInfo.SetVerticalOffset(double offset)
        {
            offset = ValidateVisualOffset(offset);
            if (!scrollOffset.Y.IsClose(offset))
            {
                offset = Math.Round((offset / defaultLineHeight), 0) * defaultLineHeight;
                SetScrollOffset(new Vector(scrollOffset.X, offset));
                InvalidateMeasure(DispatcherPriority.Normal);
                InvalidateVisual();
            }
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (rectangle.IsEmpty || visual == null || visual == this || !this.IsAncestorOf(visual))
            {
                return Rect.Empty;
            }
            return rectangle;
        }


        double defaultLineHeight;
        double defaultLineWidth;
        public double DefaultLineHeight
        {
            get
            {
                CalculateLineLayout();
                return defaultLineHeight;
            }
        }

        public double DefaultLineWidth
        {
            get
            {
                CalculateLineLayout();
                return defaultLineWidth;
            }
        }

        //double addressWidth;
        //public double AddressWidth
        //{
        //    get
        //    {
        //        CalculateAddressLayout();
        //        return addressWidth;
        //    }
        //}

        internal const string C_COLUMNHEADER = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F";

        void CalculateLineLayout()
        {
            FormattedText ft = new FormattedText(
                    C_COLUMNHEADER,
                    s_hexCulture,
                    base.FlowDirection,
                    this.GetTypeface(),
                    this.FontSize,
                    this.Foreground
                );
            defaultLineHeight = ft.Height;
            defaultLineWidth = ft.Width;
            ft = null;
        }

        //void CalculateAddressLayout()
        //{
        //    FormattedText ft = new FormattedText(
        //            "00000000",
        //            s_hexCulture,
        //            base.FlowDirection,
        //            this.GetTypeface(),
        //            this.FontSize,
        //            this.Foreground
        //        );
        //    addressWidth = ft.Width + 3.0d;
        //    ft = null;
        //}


        #endregion

        #region InvalidateMeasure(DispatcherPriority)
        DispatcherOperation invalidateMeasureOperation;

        void InvalidateMeasure(DispatcherPriority priority)
        {
            if (priority >= DispatcherPriority.Render)
            {
                if (invalidateMeasureOperation != null)
                {
                    invalidateMeasureOperation.Abort();
                    invalidateMeasureOperation = null;
                }
                base.InvalidateMeasure();
            }
            else
            {
                if (invalidateMeasureOperation != null)
                {
                    invalidateMeasureOperation.Priority = priority;
                }
                else
                {
                    invalidateMeasureOperation = Dispatcher.BeginInvoke(
                        priority,
                        new Action(
                            delegate
                            {
                                invalidateMeasureOperation = null;
                                base.InvalidateMeasure();
                            }
                        )
                    );
                }
            }
        }
        #endregion

    }
}
