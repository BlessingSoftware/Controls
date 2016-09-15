﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace BlessingSoftware.Utils {
    static class ExtensionMethods {
        #region Epsilon / IsClose / CoerceValue
        /// <summary>
        /// Epsilon used for <c>IsClose()</c> implementations.
        /// We can use up quite a few digits in front of the decimal point (due to visual positions being relative to document origin),
        /// and there's no need to be too accurate (we're dealing with pixels here),
        /// so we will use the value 0.01.
        /// Previosly we used 1e-8 but that was causing issues:
        /// http://community.sharpdevelop.net/forums/t/16048.aspx
        /// </summary>
        public const double Epsilon = 0.01;

        /// <summary>
        /// Returns true if the doubles are close (difference smaller than 0.01).
        /// </summary>
        public static bool IsClose(this double d1, double d2) {
            if(d1 == d2) // required for infinities
                return true;
            return Math.Abs(d1 - d2) < Epsilon;
        }

        /// <summary>
        /// Returns true if the doubles are close (difference smaller than 0.01).
        /// </summary>
        public static bool IsClose(this Size d1, Size d2) {
            return IsClose(d1.Width, d2.Width) && IsClose(d1.Height, d2.Height);
        }

        /// <summary>
        /// Returns true if the doubles are close (difference smaller than 0.01).
        /// </summary>
        public static bool IsClose(this Vector d1, Vector d2) {
            return IsClose(d1.X, d2.X) && IsClose(d1.Y, d2.Y);
        }

        /// <summary>
        /// Forces the value to stay between mininum and maximum.
        /// </summary>
        /// <returns>minimum, if value is less than minimum.
        /// Maximum, if value is greater than maximum.
        /// Otherwise, value.</returns>
        public static double CoerceValue(this double value, double minimum, double maximum) {
            return Math.Max(Math.Min(value, maximum), minimum);
        }

        /// <summary>
        /// Forces the value to stay between mininum and maximum.
        /// </summary>
        /// <returns>minimum, if value is less than minimum.
        /// Maximum, if value is greater than maximum.
        /// Otherwise, value.</returns>
        public static int CoerceValue(this int value, int minimum, int maximum) {
            return Math.Max(Math.Min(value, maximum), minimum);
        }
        #endregion

        #region CreateTypeface
        /// <summary>
        /// Creates typeface from the framework element.
        /// </summary>
        public static Typeface CreateTypeface(this FrameworkElement fe) {
            return new Typeface((FontFamily)fe.GetValue(TextBlock.FontFamilyProperty),
                                (FontStyle)fe.GetValue(TextBlock.FontStyleProperty),
                                (FontWeight)fe.GetValue(TextBlock.FontWeightProperty),
                                (FontStretch)fe.GetValue(TextBlock.FontStretchProperty));
        }
        #endregion

        #region AddRange / Sequence
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements) {
            foreach(T e in elements)
                collection.Add(e);
        }

        /// <summary>
        /// Creates an IEnumerable with a single value.
        /// </summary>
        public static IEnumerable<T> Sequence<T>(T value) {
            yield return value;
        }
        #endregion

        #region XML reading
        /// <summary>
        /// Gets the value of the attribute, or null if the attribute does not exist.
        /// </summary>
        public static string GetAttributeOrNull(this XmlElement element, string attributeName) {
            XmlAttribute attr = element.GetAttributeNode(attributeName);
            return attr != null ? attr.Value : null;
        }

        /// <summary>
        /// Gets the value of the attribute as boolean, or null if the attribute does not exist.
        /// </summary>
        public static bool? GetBoolAttribute(this XmlElement element, string attributeName) {
            XmlAttribute attr = element.GetAttributeNode(attributeName);
            return attr != null ? (bool?)XmlConvert.ToBoolean(attr.Value) : null;
        }

        /// <summary>
        /// Gets the value of the attribute as boolean, or null if the attribute does not exist.
        /// </summary>
        public static bool? GetBoolAttribute(this XmlReader reader, string attributeName) {
            string attributeValue = reader.GetAttribute(attributeName);
            if(attributeValue == null)
                return null;
            else
                return XmlConvert.ToBoolean(attributeValue);
        }
        #endregion

        #region DPI independence
        public static Rect TransformToDevice(this Rect rect, Visual visual) {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return Rect.Transform(rect, matrix);
        }

        public static Rect TransformFromDevice(this Rect rect, Visual visual) {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return Rect.Transform(rect, matrix);
        }

        public static Size TransformToDevice(this Size size, Visual visual) {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
        }

        public static Size TransformFromDevice(this Size size, Visual visual) {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
        }

        public static Point TransformToDevice(this Point point, Visual visual) {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Point(point.X * matrix.M11, point.Y * matrix.M22);
        }

        public static Point TransformFromDevice(this Point point, Visual visual) {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return new Point(point.X * matrix.M11, point.Y * matrix.M22);
        }
        #endregion

        #region System.Drawing <-> WPF conversions
        /*
		public static System.Drawing.Point ToSystemDrawing(this Point p)
		{
			return new System.Drawing.Point((int)p.X, (int)p.Y);
		}
		
		public static Point ToWpf(this System.Drawing.Point p)
		{
			return new Point(p.X, p.Y);
		}
		
		public static Size ToWpf(this System.Drawing.Size s)
		{
			return new Size(s.Width, s.Height);
		}
		
		public static Rect ToWpf(this System.Drawing.Rectangle rect)
		{
			return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
		}
		 */
        #endregion

        public static string JoinHex(this byte[] buff, char separator) {
            return buff.JoinHex(separator, 0, buff.Length);
        }

        public static string JoinHex(this byte[] buff, char separator, int index, int length) {
            if((length < 0) || (index < 0) || (index + length) > buff.Length) {
                throw new ArgumentOutOfRangeException();
            }
            StringBuilder sb = new StringBuilder(length * 3);
            byte temp = buff[index++];
            int tmpI;
            tmpI = (temp >> 4) & 0x0F;
            if(tmpI > 0x09)
                tmpI += 0x37;
            else
                tmpI += 0x30;
            sb.Append((char)tmpI);

            tmpI = temp & 0x0F;
            if(tmpI > 0x09)
                tmpI += 0x37;
            else
                tmpI += 0x30;
            sb.Append((char)tmpI);
            length--;

            while(length > 0) {
                sb.Append(separator);
                temp = buff[index++];
                tmpI = (temp >> 4) & 0x0F;
                if(tmpI > 0x09)
                    tmpI += 0x37;
                else
                    tmpI += 0x30;
                sb.Append((char)tmpI);

                tmpI = temp & 0x0F;
                if(tmpI > 0x09)
                    tmpI += 0x37;
                else
                    tmpI += 0x30;
                sb.Append((char)tmpI);
                length--;
            }

            return sb.ToString();
        }

        public static string JoinHex(this IEnumerable<byte> buff, char separator) {
            string result;
            using(IEnumerator<byte> enumerator = buff.GetEnumerator()) {
                StringBuilder stringBuilder = new StringBuilder();
                byte temp;
                int tmpI;
                if(!enumerator.MoveNext()) {
                    result = string.Empty;
                } else {
                    temp = enumerator.Current;
                    tmpI = (temp >> 4) & 0x0F;
                    if(tmpI > 0x09)
                        tmpI += 0x37;
                    else
                        tmpI += 0x30;
                    stringBuilder.Append((char)tmpI);

                    tmpI = temp & 0x0F;
                    if(tmpI > 0x09)
                        tmpI += 0x37;
                    else
                        tmpI += 0x30;
                    stringBuilder.Append((char)tmpI);

                }
                while(enumerator.MoveNext()) {
                    stringBuilder.Append(separator);
                    temp = enumerator.Current;
                    tmpI = (temp >> 4) & 0x0F;
                    if(tmpI > 0x09)
                        tmpI += 0x37;
                    else
                        tmpI += 0x30;
                    stringBuilder.Append((char)tmpI);

                    tmpI = temp & 0x0F;
                    if(tmpI > 0x09)
                        tmpI += 0x37;
                    else
                        tmpI += 0x30;
                    stringBuilder.Append((char)tmpI);
                }
                result = stringBuilder.ToString();
            }
            return result;
        }

        public static IEnumerable<DependencyObject> VisualAncestorsAndSelf(this DependencyObject obj) {
            while(obj != null) {
                yield return obj;
                obj = VisualTreeHelper.GetParent(obj);
            }
        }

        [Conditional("DEBUG")]
        public static void CheckIsFrozen(Freezable f) {
            if(f != null && !f.IsFrozen)
                Debug.WriteLine("Performance warning: Not frozen: " + f.ToString());
        }

        [Conditional("DEBUG")]
        public static void Log(bool condition, string format, params object[] args) {
            if(condition) {
                string output = DateTime.Now.ToString("hh:MM:ss") + ": " + string.Format(format, args) + Environment.NewLine + Environment.StackTrace;
                Console.WriteLine(output);
                Debug.WriteLine(output);
            }
        }
    }

}
