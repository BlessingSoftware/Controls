using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlessingSoftware.Controls
{
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    public class HexViewer : Control //ContentControl //
    {
        public HexViewer()
        {
            ViewArea = new HexArea();
            //HexArea.Name = "HexArea";

            //SetBinding(Controls.HexArea.ShowAddressProperty, new Binding("ShowColumn") { Source =this.HexArea,Mode= BindingMode.TwoWay});
        }

        public HexArea ViewArea { get; set; }

        static HexViewer()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer), new FrameworkPropertyMetadata(typeof(HexViewer)));
        }

        ScrollViewer scrollViewer;
        //		TextBlock textBlock;
        /// <summary>
        /// Is called after the template was applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollViewer = (ScrollViewer)base.GetTemplateChild("PART_ScrollViewer");
            IScrollInfo sc = ViewArea as IScrollInfo;
            if (sc != null)
            {
                sc.ScrollOwner = scrollViewer;
            }
            //			textBlock = (TextBlock)Template.FindName("PART_ColumnHeader", this);
            //
            //			textBlock.Text=s_colheader;
        }

        /// <summary>
        /// Gets the scroll viewer used by the text editor.
        /// This property can return null if the template has not been applied / does not contain a scroll viewer.
        /// </summary>
        internal ScrollViewer ScrollViewer
        {
            get { return scrollViewer; }
        }

        #region ScrollBarVisibility
        /// <summary>
        /// Dependency property for <see cref="HorizontalScrollBarVisibility"/>
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(typeof(HexViewer), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Gets/Sets the horizontal scroll bar visibility.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="VerticalScrollBarVisibility"/>
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(typeof(HexViewer), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Gets/Sets the vertical scroll bar visibility.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        #endregion
        //public static readonly DependencyProperty ShowColumnProperty = Controls.HexArea.ShowColumnProperty.AddOwner(typeof(HexViewer),
        //    new PropertyMetadata(true, OnShowColumnPropertyChanged));

        //[Category("Layout")]
        //public bool ShowColumn
        //{
        //    get { return (bool)GetValue(ShowColumnProperty); }
        //    set { SetValue(ShowColumnProperty, value); }
        //}

        //static void OnShowColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as HexViewer).OnShowColumnPropertyChanged((bool)e.NewValue);
        //}

        //void OnShowColumnPropertyChanged(bool newValue)
        //{
        //    if (this.HexArea != null)
        //        this.ShowColumn = newValue;
        //}

        #region DependencyProperties
        //public static readonly DependencyProperty BaseStreamProperty =
        //    Controls.HexArea.BaseStreamProperty.AddOwner(typeof(HexViewer), new PropertyMetadata(null));

        public Stream BaseStream
        {
            get { return (Stream)ViewArea.GetValue(HexArea.BaseStreamProperty); }
            set { ViewArea.SetValue(HexArea.BaseStreamProperty, value); }
        }

        //public static readonly DependencyProperty ShowAddressProperty =
        //    Controls.HexArea.ShowAddressProperty.AddOwner(typeof(HexViewer),
        //    new PropertyMetadata(true, OnShowAddressPropertyChanged));

        [Category("Layout")]
        public bool ShowAddress
        {
            get { return (bool)ViewArea.GetValue(HexArea.ShowAddressProperty); }
            set { ViewArea.SetValue(HexArea.ShowAddressProperty, value); }
        }

        static void OnShowAddressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HexViewer).OnShowAddressPropertyChanged((bool)e.NewValue);
        }

        void OnShowAddressPropertyChanged(bool newValue)
        {
            if (this.ViewArea != null)
                this.ShowAddress = newValue;
        }

        public static readonly DependencyProperty ColumnBackgroundProperty =
            Controls.HexArea.ColumnBackgroundProperty.AddOwner(typeof(HexViewer), new FrameworkPropertyMetadata(SystemColors.ControlBrush));

        [Category("Appearance")]
        public Brush ColumnBackground
        {
            get { return (Brush)GetValue(ColumnBackgroundProperty); }
            set { SetValue(ColumnBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ColumnForegroundProperty = Controls.HexArea.ColumnForegroundProperty.AddOwner(typeof(HexViewer), new FrameworkPropertyMetadata(Brushes.Black));

        [Category("Appearance")]
        public Brush ColumnForeground
        {
            get { return (Brush)GetValue(ColumnForegroundProperty); }
            set { SetValue(ColumnForegroundProperty, value); }
        }



        #endregion

        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                var fd = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (fd != null && File.Exists(fd[0]))
                {
                    this.BaseStream = File.OpenRead(fd[0]);
                }
            }
            base.OnDrop(e);
        }
    }


}
