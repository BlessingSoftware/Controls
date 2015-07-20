using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BlessingSoftware.Controls
{
   public interface IScrollable
    {
        Orientation Orientation { get; }

        int Offset { get; set; }

        Size RenderSize { get; set; }
    }
}
