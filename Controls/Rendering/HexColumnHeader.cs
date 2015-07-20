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
    internal class HexColumnHeader : FrameworkElement, IScrollable
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
                return (FontFamily)base.GetValue(HexColumnHeader.FontFamilyProperty);
            }
            set
            {
                base.SetValue(HexColumnHeader.FontFamilyProperty, value);
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)base.GetValue(HexColumnHeader.FontStyleProperty);
            }
            set
            {
                base.SetValue(HexColumnHeader.FontStyleProperty, value);
            }
        }

        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)base.GetValue(HexColumnHeader.FontWeightProperty);
            }
            set
            {
                base.SetValue(HexColumnHeader.FontWeightProperty, value);
            }
        }

        public FontStretch FontStretch
        {
            get
            {
                return (FontStretch)base.GetValue(HexColumnHeader.FontStretchProperty);
            }
            set
            {
                base.SetValue(HexColumnHeader.FontStretchProperty, value);
            }
        }

        [TypeConverter(typeof(FontSizeConverter)), Localizability(LocalizationCategory.None)]
        public double FontSize
        {
            get
            {
                return (double)base.GetValue(HexColumnHeader.FontSizeProperty);
            }
            set
            {
                base.SetValue(HexColumnHeader.FontSizeProperty, value);
            }
        }

        public Brush Foreground
        {
            get
            {
                return (Brush)base.GetValue(HexColumnHeader.ForegroundProperty);
            }
            set
            {
                base.SetValue(HexColumnHeader.ForegroundProperty, value);
            }
        }

        public Brush Background
        {
            get
            {
                return (Brush)base.GetValue(HexColumnHeader.BackgroundProperty);
            }
            set
            {
                base.SetValue(HexColumnHeader.BackgroundProperty, value);
            }
        }

        #endregion

        static HexColumnHeader()
        {
            HexColumnHeader.FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(HexColumnHeader));
            HexColumnHeader.FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(HexColumnHeader));
            HexColumnHeader.FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(HexColumnHeader));
            HexColumnHeader.FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(HexColumnHeader));
            HexColumnHeader.FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(HexColumnHeader));
            HexColumnHeader.ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(HexColumnHeader));

            HexColumnHeader.BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(HexColumnHeader),
                                        new FrameworkPropertyMetadata(SystemColors.ControlBrush));

        }

        public Orientation Orientation
        {
            get
            {
                return Orientation.Horizontal;
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
            //System.Diagnostics.Debug.WriteLine(availableSize);
            FormattedText ft =
                                new FormattedText(
                                C_COLUMNHEADER,
                                CultureInfo.GetCultureInfo("en-US"),
                                base.FlowDirection,
                                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                                this.FontSize,
                                this.Foreground
                            );
            return new Size(Math.Max(ft.Width, availableSize.Width), ft.Height);
        }

        const string C_COLUMNHEADER = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F";
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(this.Background, null, new Rect(this.RenderSize));

            FormattedText ft =
                                new FormattedText(
                                C_COLUMNHEADER,
                                CultureInfo.GetCultureInfo("en-US"),
                                base.FlowDirection,
                                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                                this.FontSize,
                                this.Foreground
                            );
            double t = 0.0d;// offset * ft.Width / 47.0d;
            //double t2 = ft.Width - this.RenderSize.Width;
            //if (t<t2)
            //{
            //    t = t2;
            //    offset = Convert.ToInt32(t / (ft.Width / 47.0d));
            //}

            drawingContext.DrawText(ft, new Point(-t, 0.0d));
        }



    }
}
