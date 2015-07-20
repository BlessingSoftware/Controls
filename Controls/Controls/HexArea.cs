using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using BlessingSoftware.Utils;
using BlessingSoftware.Controls.Rendering;
using System.IO;

namespace BlessingSoftware.Controls
{
    [TemplatePart(Name = "PART_ColumnHeader", Type = typeof(IScrollable)), TemplatePart(Name = "PART_HexView", Type = typeof(HexView))]
    [TemplatePart(Name = "PART_AddrBar", Type = typeof(IScrollable))]
    public class HexArea : Control, IScrollInfo
    {
        //		public InlineUIContainer[] Lines{get;private set;}

        //internal const string C_COLUMNHEADER = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F";

        //static CultureInfo s_hexCulture;

        //FormattedText formattedText;
        //byte[] buff;
        public HexArea()
        {
            Typeface tf = this.GetTypeface();
            //formattedText = new FormattedText(
            //    C_COLUMNHEADER,
            //    s_hexCulture,
            //    base.FlowDirection,
            //    tf,
            //    this.FontSize,
            //    this.Foreground
            //);
            //defaultLineHeight = formattedText.Height;
            //wideSpaceWidth = this.DefaultLineWidth / C_COLUMNHEADER.Length;

            //buff = new byte[0x810];
            //Random rnd = new Random();
            //rnd.NextBytes(buff);

            m_buffer = new BufferedBytes();//new System.IO.MemoryStream(buff)
            m_linePos = 0;
        }
        BufferedBytes m_buffer;
        int m_linePos;

        IScrollable colHeader;
        public IScrollable ColumnHeader
        {
            get
            {
                return colHeader;
            }
        }

        IScrollable addrBar;
        public IScrollable AddressBar
        {
            get
            {
                return addrBar;
            }
        }

        HexView hexView;

        public HexView HexView
        {
            get
            {
                return hexView;
            }
        }

        double _lineHeight, _charWidth;
        int _linesInpage;

        public double LineHeight { get { return _lineHeight; } }

        public double CharWidth { get { return _charWidth; } }

        public int LinesInpage
        {
            get
            {
                return _linesInpage;
            }
        }

        static HexArea()
        {

            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(HexArea), new FrameworkPropertyMetadata(typeof(HexArea)));

        }

        #region DependencyProperties

        public static readonly DependencyProperty BaseStreamProperty =
            DependencyProperty.Register("BaseStream", typeof(Stream), typeof(HexArea),
                                        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBaseStreamChanged)));

        public Stream BaseStream
        {
            get { return (Stream)GetValue(BaseStreamProperty); }
            set { SetValue(BaseStreamProperty, value); }
        }

        static void OnBaseStreamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HexArea)d).OnBaseStreamChanged((Stream)e.NewValue);
        }

        void OnBaseStreamChanged(Stream stream)
        {
            this.m_buffer.ChangeStream(stream);
            this.ClearScollData();
        }

        void ClearScollData()
        {
            (this.hexView as IScrollInfo).ScrollOwner.InvalidateScrollInfo();
        }

        //public static readonly DependencyProperty ShowColumnProperty =
        //    DependencyProperty.Register("ShowColumn", typeof(bool), typeof(HexArea),
        //                                new FrameworkPropertyMetadata(true));
        //[Category("Layout")]
        //public bool ShowColumn
        //{
        //    get { return (bool)GetValue(ShowColumnProperty); }
        //    set { SetValue(ShowColumnProperty, value); }
        //}

        public static readonly DependencyProperty ShowAddressProperty =
            DependencyProperty.Register("ShowAddress", typeof(bool), typeof(HexArea),
                                        new FrameworkPropertyMetadata(true));
        [Category("Layout")]
        public bool ShowAddress
        {
            get { return (bool)GetValue(ShowAddressProperty); }
            set { SetValue(ShowAddressProperty, value); }
        }

        public static readonly DependencyProperty ColumnBackgroundProperty =
            DependencyProperty.Register("ColumnBackground", typeof(Brush), typeof(HexArea),
                                        new FrameworkPropertyMetadata(SystemColors.ControlBrush));
        [Category("Appearance")]
        public Brush ColumnBackground
        {
            get { return (Brush)GetValue(ColumnBackgroundProperty); }
            set { SetValue(ColumnBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ColumnForegroundProperty =
            DependencyProperty.Register("ColumnForeground", typeof(Brush), typeof(HexArea),
                                        new FrameworkPropertyMetadata(Brushes.Black));
        [Category("Appearance")]
        public Brush ColumnForeground
        {
            get { return (Brush)GetValue(ColumnForegroundProperty); }
            set { SetValue(ColumnForegroundProperty, value); }
        }

        /// <summary>
        /// The <see cref="SelectionBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionBrushProperty =
                    DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(HexArea));

        /// <summary>
        /// Gets/Sets the background brush used for the selection.
        /// </summary>
        public Brush SelectionBrush
        {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        /// <summary>
        /// The <see cref="SelectionForeground"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionForegroundProperty =
                    DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(HexArea));

        /// <summary>
        /// Gets/Sets the foreground brush used selected text.
        /// </summary>
        public Brush SelectionForeground
        {
            get { return (Brush)GetValue(SelectionForegroundProperty); }
            set { SetValue(SelectionForegroundProperty, value); }
        }

        /// <summary>
        /// The <see cref="SelectionBorder"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionBorderProperty =
                    DependencyProperty.Register("SelectionBorder", typeof(Pen), typeof(HexArea));

        /// <summary>
        /// Gets/Sets the background brush used for the selection.
        /// </summary>
        public Pen SelectionBorder
        {
            get { return (Pen)GetValue(SelectionBorderProperty); }
            set { SetValue(SelectionBorderProperty, value); }
        }

        #endregion

        #region Override Methods

        Typeface GetTypeface()
        {
            return new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch);
        }

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    availableSize = base.MeasureOverride(availableSize);
        //    //double w = this.DefaultLineWidth;
        //    //if (this.ShowAddress)
        //    //    w += AddressWidth;
        //    //Size sz = new Size(
        //    //    Math.Max(availableSize.Width, w),
        //    //    defaultLineHeight * 129.0d
        //    //);
        //    //SetScrollData(availableSize, sz, scrollOffset);
        //    if (availableSize.Width == double.PositiveInfinity)
        //        availableSize.Width = SystemParameters.PrimaryScreenWidth;
        //    if (availableSize.Height == double.PositiveInfinity)
        //        availableSize.Height = SystemParameters.PrimaryScreenHeight;
        //    return availableSize;
        //}


        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);

        //    Point pos = new Point(
        //        -scrollOffset.X,
        //        -scrollOffset.Y
        //    );

        //    Typeface tf = this.GetTypeface();

        //    if (!ShowColumn)
        //    {
        //        pos.Y -= DefaultLineHeight;
        //    }

        //    if (ShowAddress)
        //    {
        //        pos.X += this.AddressWidth;
        //    }

        //    DrawHex(pos, tf, drawingContext);

        //    pos.Y = -scrollOffset.Y - defaultLineHeight;

        //    if (ShowColumn)
        //    {
        //        drawingContext.DrawRectangle(
        //            ColumnBackground,
        //            null,
        //            new Rect(0d, 0d, scrollViewport.Width, defaultLineHeight)
        //        );

        //        drawingContext.DrawText(
        //            new FormattedText(
        //            C_COLUMNHEADER,
        //            s_hexCulture,
        //            base.FlowDirection,
        //            tf,
        //            this.FontSize,
        //            this.Foreground
        //        ), new Point(pos.X, 0d));
        //        pos.Y += defaultLineHeight;
        //    }

        //    if (ShowAddress)
        //    {
        //        drawingContext.DrawRectangle(
        //            ColumnBackground,
        //            null,
        //            new Rect(0d, 0d, AddressWidth, scrollViewport.Height)
        //        );
        //        pos = new Point(0, -scrollOffset.Y);

        //        for (int i = 0; i < 0x80; i++)
        //        {
        //            pos.Y += defaultLineHeight;
        //            if (pos.Y < 0.1d)
        //            {
        //                continue;
        //            }
        //            else if (pos.Y > this.Height)
        //            {
        //                break;
        //            }

        //            drawingContext.DrawText(
        //                new FormattedText(
        //                    (i << 4).ToString("X8"),
        //                    s_hexCulture,
        //                    base.FlowDirection,
        //                    tf,
        //                    this.FontSize,
        //                    this.Foreground
        //                ),
        //                pos);
        //        }
        //    }
        //}

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            arrangeBounds = base.ArrangeOverride(arrangeBounds);
            return arrangeBounds;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            colHeader = base.GetTemplateChild("PART_ColumnHeader") as IScrollable;
            addrBar = base.GetTemplateChild("PART_AddrBar") as IScrollable;
            hexView = base.GetTemplateChild("PART_HexView") as HexView;
            if (hexView != null)
            {
                hexView.Buffer = this.m_buffer;
            }
            ApplyScrollInfo();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((e.Property == HexArea.FontFamilyProperty)
                || (e.Property == HexArea.FontSizeProperty)
                || (e.Property == HexArea.FontStretchProperty)
                || (e.Property == HexArea.FontStyleProperty)
                || (e.Property == HexArea.FontWeightProperty)
                )
            {
                MeasureFont();
            }

            base.OnPropertyChanged(e);
        }

        void MeasureFont()
        {
            Typeface tf = this.GetTypeface();
            FormattedText fText = new FormattedText(new string('8', 10),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                tf,
                FontSize, Foreground
                );

            _charWidth = fText.Width / 10;
            _lineHeight = fText.Height;

            if (hexView != null)
            {
                _linesInpage = (int)Math.Round(hexView.RenderSize.Height / _lineHeight, 0);
            }
            else
            {
                _linesInpage = 0;
            }
        }

        //void DrawHex(Point pos, Typeface tf, DrawingContext drawingContext)
        //{
        //    for (int i = 0; i < 0x80; i++)
        //    {
        //        pos.Y += defaultLineHeight;
        //        if (pos.Y < 0.1)
        //        {
        //            continue;
        //        }
        //        else if (pos.Y > this.Height)
        //        {
        //            break;
        //        }
        //        drawingContext.DrawText(
        //            new FormattedText(
        //                m_buffer.GetBytesLine(i).JoinHex(' '),
        //                              //buff.JoinHex(' ', i << 4, 16),
        //                              s_hexCulture,
        //                              base.FlowDirection,
        //                              tf,
        //                              this.FontSize,
        //                              this.Foreground),
        //            pos);
        //    }
        //}

        #endregion

        #region IScrollInfo implementation
        ScrollViewer scrollOwner;
        bool canVerticallyScroll, canHorizontallyScroll;

        void ApplyScrollInfo()
        {
            if (hexView != null)
            {
                hexView.ScrollOwner = scrollOwner;
                (hexView as IScrollInfo).CanVerticallyScroll = canVerticallyScroll;
                (hexView as IScrollInfo).CanHorizontallyScroll = canHorizontallyScroll;
                scrollOwner = null;
            }
        }

        bool IScrollInfo.CanVerticallyScroll
        {
            get { return hexView != null ? (hexView as IScrollInfo).CanVerticallyScroll : false; }
            set
            {
                canVerticallyScroll = value;
                if (hexView != null)
                    (hexView as IScrollInfo).CanVerticallyScroll = value;
            }
        }

        bool IScrollInfo.CanHorizontallyScroll
        {
            get { return hexView != null ? (hexView as IScrollInfo).CanHorizontallyScroll : false; }
            set
            {
                canHorizontallyScroll = value;
                if (hexView != null)
                    (hexView as IScrollInfo).CanHorizontallyScroll = value;
            }
        }

        double IScrollInfo.ExtentWidth
        {
            get
            {
                if (hexView == null)
                    return 0.0d;
                double r = hexView.ExtentWidth;
                if (ShowAddress && (addrBar != null))
                {
                    r += addrBar.RenderSize.Width;
                }
                return r;
            }
        }

        double IScrollInfo.ExtentHeight
        {
            get
            {
                if (hexView == null)
                    return 0.0d;
                double r = hexView.ExtentHeight;
                if (colHeader != null)
                {
                    r += colHeader.RenderSize.Height;
                }
                return r;
            }
        }

        double IScrollInfo.ViewportWidth
        {
            get
            {

                if (hexView == null)
                    return 0.0d;
                double r = hexView.ViewportWidth;
                if (ShowAddress && (addrBar != null))
                {
                    r += addrBar.RenderSize.Width;
                }
                return r;
            }
        }

        double IScrollInfo.ViewportHeight
        {
            get
            {

                if (hexView == null)
                    return 0.0d;
                double r = hexView.ViewportHeight;
                if (colHeader != null)
                {
                    r += colHeader.RenderSize.Height;
                }
                return r;
            }
        }

        double IScrollInfo.HorizontalOffset
        {
            get { return hexView != null ? hexView.HorizontalOffset : 0; }
        }

        double IScrollInfo.VerticalOffset
        {
            get { return hexView != null ? hexView.VerticalOffset : 0; }
        }

        ScrollViewer IScrollInfo.ScrollOwner
        {
            get { return hexView != null ? hexView.ScrollOwner : null; }
            set
            {
                if (hexView != null)
                    hexView.ScrollOwner = value;
                else
                    scrollOwner = value;
            }
        }

        void IScrollInfo.LineUp()
        {
            if (hexView != null) hexView.LineUp();
            if (addrBar != null)
                addrBar.Offset = addrBar.Offset - 1;

        }

        void IScrollInfo.LineDown()
        {
            if (hexView != null) hexView.LineDown();
            if (addrBar != null)
                addrBar.Offset = addrBar.Offset + 1;
        }

        void IScrollInfo.LineLeft()
        {
            if (hexView != null)
                (hexView as IScrollInfo).SetHorizontalOffset(hexView.HorizontalOffset - _charWidth);

            if (colHeader != null)
                colHeader.Offset = colHeader.Offset - 1;
        }

        void IScrollInfo.LineRight()
        {
            if (hexView != null)
                (hexView as IScrollInfo).SetHorizontalOffset(hexView.HorizontalOffset + _charWidth);
            if (colHeader != null)
                colHeader.Offset = colHeader.Offset + 1;
        }

        void IScrollInfo.PageUp()
        {
            if (hexView != null) hexView.PageUp();
        }

        void IScrollInfo.PageDown()
        {
            if (hexView != null) hexView.PageDown();
        }

        void IScrollInfo.PageLeft()
        {
            if (hexView != null) hexView.PageLeft();
        }

        void IScrollInfo.PageRight()
        {
            if (hexView != null) hexView.PageRight();
        }

        void IScrollInfo.MouseWheelUp()
        {
            if (hexView != null) (hexView as IScrollInfo).MouseWheelUp();
        }

        void IScrollInfo.MouseWheelDown()
        {
            if (hexView != null) (hexView as IScrollInfo).MouseWheelDown();
        }

        void IScrollInfo.MouseWheelLeft()
        {
            if (hexView != null) (hexView as IScrollInfo).MouseWheelLeft();
        }

        void IScrollInfo.MouseWheelRight()
        {
            if (hexView != null) (hexView as IScrollInfo).MouseWheelRight();
        }

        void IScrollInfo.SetHorizontalOffset(double offset)
        {
            if (hexView != null) (hexView as IScrollInfo).SetHorizontalOffset(offset);
            //if (colHeader != null) colHeader.SetHorizontalOffset(offset);
        }

        void IScrollInfo.SetVerticalOffset(double offset)
        {
            if (hexView != null) (hexView as IScrollInfo).SetVerticalOffset(offset);
        }

        Rect IScrollInfo.MakeVisible(System.Windows.Media.Visual visual, Rect rectangle)
        {
            if (hexView != null)
                return hexView.MakeVisible(visual, rectangle);
            else
                return Rect.Empty;
        }
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
