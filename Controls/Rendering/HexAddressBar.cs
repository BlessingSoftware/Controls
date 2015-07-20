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

namespace BlessingSoftware.Controls.Rendering
{
    internal class HexAddressBar : FrameworkElement, IScrollable
    {
        #region DependencyProperty
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
                return (FontFamily)base.GetValue(HexAddressBar.FontFamilyProperty);
            }
            set
            {
                base.SetValue(HexAddressBar.FontFamilyProperty, value);
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)base.GetValue(HexAddressBar.FontStyleProperty);
            }
            set
            {
                base.SetValue(HexAddressBar.FontStyleProperty, value);
            }
        }

        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)base.GetValue(HexAddressBar.FontWeightProperty);
            }
            set
            {
                base.SetValue(HexAddressBar.FontWeightProperty, value);
            }
        }

        public FontStretch FontStretch
        {
            get
            {
                return (FontStretch)base.GetValue(HexAddressBar.FontStretchProperty);
            }
            set
            {
                base.SetValue(HexAddressBar.FontStretchProperty, value);
            }
        }

        [TypeConverter(typeof(FontSizeConverter)), Localizability(LocalizationCategory.None)]
        public double FontSize
        {
            get
            {
                return (double)base.GetValue(HexAddressBar.FontSizeProperty);
            }
            set
            {
                base.SetValue(HexAddressBar.FontSizeProperty, value);
            }
        }

        public Brush Foreground
        {
            get
            {
                return (Brush)base.GetValue(HexAddressBar.ForegroundProperty);
            }
            set
            {
                base.SetValue(HexAddressBar.ForegroundProperty, value);
            }
        }

        public Brush Background
        {
            get
            {
                return (Brush)base.GetValue(HexAddressBar.BackgroundProperty);
            }
            set
            {
                base.SetValue(HexAddressBar.BackgroundProperty, value);
            }
        }

        #endregion

        static HexAddressBar()
        {
            HexAddressBar.FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(HexAddressBar));
            HexAddressBar.FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(HexAddressBar));
            HexAddressBar.FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(HexAddressBar));
            HexAddressBar.FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(HexAddressBar));
            HexAddressBar.FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(HexAddressBar));
            HexAddressBar.ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(HexAddressBar));

            HexAddressBar.BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(HexAddressBar),
                                        new FrameworkPropertyMetadata(SystemColors.ControlBrush));

        }

        public Orientation Orientation
        {
            get
            {
                return Orientation.Vertical;
            }
        }

        int offset;
        public int Offset
        {
            get
            {
                return offset;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (offset != value)
                {
                    offset = value;
                    InvalidateVisual();
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            FormattedText ft =   new FormattedText(
                        "000000000",
                        CultureInfo.GetCultureInfo("en-US"),
                        base.FlowDirection,
                        new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                        this.FontSize,
                        this.Foreground
                    );
            availableSize.Width = ft.Width;
            return (availableSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(this.Background, null, new Rect(this.RenderSize));

            Point pos = new Point();

            int off = this.offset;

            Typeface tf = new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch);
            CultureInfo info = CultureInfo.GetCultureInfo("en-US");
            FormattedText temp;
            double height = this.RenderSize.Height;
            while (pos.Y < height)
            {
                temp = new FormattedText(
                        (off++).ToString("00000000"),
                        info,
                        FlowDirection.LeftToRight,
                        tf,
                        this.FontSize,
                        this.Foreground
                        );
                pos.Y += temp.Height;
                drawingContext.DrawText(
                    temp,
                    pos
                    );
            }
        }

    }
}
