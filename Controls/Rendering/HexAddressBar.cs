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
	public class HexAddressBar : FrameworkElement, IScrollable
	{
		#region DependencyProperty
		
		public static readonly DependencyProperty AddressWidthProperty =
			HexArea.AddressWidthProperty.AddOwner(typeof(HexAddressBar));
		
		/// <summary>
		/// 地址栏的宽度4-16之间，默认值为8
		/// </summary>
		[Category("Layout")]
		public int AddressWidth
		{
			get { return (int)GetValue(AddressWidthProperty); }
			set { SetValue(AddressWidthProperty, value); }
		}
		
		public static readonly DependencyProperty LineHeightProperty =
			HexArea.LineHeightProperty.AddOwner(typeof(HexAddressBar));
		
		/// <summary>
		/// 行高
		/// </summary>
		public double LineHeight{
			get{ return (double)GetValue(LineHeightProperty);}
			set{ SetValue(LineHeightProperty,value);}
		}
		
		public static readonly DependencyProperty ColumnHeightProperty =
			HexArea.ColumnHeightProperty.AddOwner(typeof(HexAddressBar));
		
		/// <summary>
		/// 获取或设置标题栏高度
		/// </summary>
		public double ColumnHeight{
			get{ return (double)GetValue(ColumnHeightProperty);}
			set{ SetValue(ColumnHeightProperty,value);}
		}
		
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

		DrawingVisual _child;
		int _childoffset;
		DrawingVisual _header;
		public HexAddressBar()
		{
			this.offset = 0;
			_child =new DrawingVisual();
//			var dc = _child.RenderOpen();
//			dc.DrawLine(new Pen(Brushes.Gold,1.0d),new Point(),new Point(20d,20d));
//			dc.Close();
			
			this.AddVisualChild(_child);
			_header=new DrawingVisual();
			this.AddVisualChild(_header);
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

		protected override int VisualChildrenCount {
			get { return 2; }
		}
		
		protected override Visual GetVisualChild(int index)
		{
			switch (index) {
				case 0:
					return _child;
				default:
					return _header;
			}
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			string temp =new string('0',this.AddressWidth + 1);
			FormattedText ft = new FormattedText(
				temp,
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

			Typeface tf = new Typeface(this.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
			CultureInfo info = CultureInfo.GetCultureInfo("en-US");
			RenderHeader(tf,info);
			RenderChild(tf,info);
			
//			double height2=this.LineHeight;
//			
//			FormattedText temp;
//			double height = this.RenderSize.Height;
//
//			string tmp = new string('0',this.AddressWidth);
//			
//			while (pos.Y < height)
//			{
//				temp = new FormattedText(
//					(off++).ToString(tmp),
//					info,
//					FlowDirection.LeftToRight,
//					tf,
//					this.FontSize,
//					this.Foreground
//				);
//				pos.Y += height2;
//				drawingContext.DrawText(
//					temp,
//					pos
//				);
//			}
		}

		void RenderHeader(Typeface tf,CultureInfo info)
		{
			using (var dc = _header.RenderOpen()) {
				var temp2 = new FormattedText("Address", info, FlowDirection.LeftToRight, tf, this.FontSize, this.Foreground);
				dc.DrawRectangle(this.Background, null, new Rect(0.0d, 0.0d, this.RenderSize.Width, this.ColumnHeight));
				dc.DrawText(temp2, new Point(0, 0.5d * (this.ColumnHeight - temp2.Height)));
				dc.Close();
//				_header.Offset = new Vector(0, 0.5d * (this.ColumnHeight - temp2.Height));
			}
		}
		
		void RenderChild(Typeface tf,CultureInfo info)
		{			
			Point pos = new Point();
			int off = 0;
			using (var dc = _child.RenderOpen()) {
				double height2=this.LineHeight;
				
				FormattedText temp;
				double height = SystemParameters.FullPrimaryScreenHeight;// this.RenderSize.Height;

				string tmp = new string('0',this.AddressWidth);
				
				while (pos.Y < height)
				{
					temp = new FormattedText(
						(off++).ToString(tmp),
						info,
						FlowDirection.LeftToRight,
						tf,
						this.FontSize,
						this.Foreground
					);
					pos.Y += height2;
					dc.DrawText(temp,pos);
				}
				dc.Close();
				_child.Offset =new Vector(0.0d,height2 * (_childoffset - this.offset));
			}
		}

	}
}
