using Gaea.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Gaea
{
	public static class Utils
	{
		[DllImport("gdi32")]
		static extern int DeleteObject(IntPtr o);

		public static BitmapSource ToBitmapSource(this Icon iconSource)
		{
			if (iconSource == null) return null;
			return iconSource.ToBitmap().ToBitmapSource();
		}

		public static BitmapSource ToBitmapSource(this Bitmap source)
		{
			if (source == null) return null;
			IntPtr ip = source.GetHbitmap();
			BitmapSource bs = null;
			try
			{
				bs = Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally
			{
				DeleteObject(ip);
			}
			return bs;
		}

		public static System.Windows.Media.Color FromHex(string hex)
		{
			if (!hex.StartsWith("#"))
			{
				hex = "#" + hex;
			}
			return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
		}

		/// <summary>
		/// Determines the dimensions for the given size if it were cropped so that it would fit without distortion if it were resized to the given screen coordinates.
		/// 
		/// The crops an equal amount from either the top/bottom or left/right, so the center of the content is visible.
		/// </summary>
		/// <param name="origSize">Original size of the content</param>
		/// <param name="screenWidth">Target width relative dimension</param>
		/// <param name="screenHeight">Target height relative dimension</param>
		/// <returns>RectangleF representing the crop that should be performed be resizing</returns>
		public static RectangleF GetCenteredCropRectangle(SizeF origSize, int screenWidth, int screenHeight)
		{
			float screenAspectRatio = (float)screenWidth / (float)screenHeight;
			float contentAspectRatio = origSize.Width / origSize.Height;
			float imageWidth, imageHeight, xOffset, yOffset;
			if (contentAspectRatio < screenAspectRatio)
			{
				// Match width
				imageWidth = origSize.Width;
				imageHeight = origSize.Width / screenAspectRatio;
				xOffset = 0;
				yOffset = (origSize.Height - imageHeight) / 2;
			}
			else
			{
				// Match height
				imageHeight = origSize.Height;
				imageWidth = origSize.Height * screenAspectRatio;
				yOffset = 0;
				xOffset = (origSize.Width - imageWidth) / 2;
			}
			return new RectangleF { X = xOffset, Y = yOffset, Width = imageWidth, Height = imageHeight };
		}

		/// <summary>
		/// Determines the dimensions of a rectangle that would best fit the content of a given size in a container of the given size
		/// </summary>
		/// <param name="containerSize">Size of the container</param>
		/// <param name="itemSize">Size of the content</param>
		/// <returns>RectangleF representing the target location to place the content</returns>
		public static RectangleF GetBestFitInside(SizeF containerSize, SizeF itemSize)
		{
			float fitScale = Math.Min(containerSize.Width / itemSize.Width, containerSize.Height / itemSize.Height);
			float finalWidth = itemSize.Width * fitScale;
			float finalHeight = itemSize.Height * fitScale;
			float offsetX = Math.Max(0, (containerSize.Width - finalWidth) / 2);
			float offsetY = Math.Max(0, (containerSize.Height - finalHeight) / 2);
			return new RectangleF { Width = finalWidth, Height = finalHeight, X = offsetX, Y = offsetY };
		}

		/// <summary>
		/// Get the name of a source
		/// </summary>
		/// <param name="source">The source</param>
		/// <returns>Name of the source</returns>
		public static string GetName(this ISource source)
		{
			if (source == null) return null;
			return source.GetType().FullName;
		}

		/// <summary>
		/// Linq-style foreach
		/// </summary>
		/// <typeparam name="T">Type of the item</typeparam>
		/// <param name="self">IEnumerable/></param>
		/// <param name="action">Action to perform on the item</param>
		public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
		{
			foreach (var item in self)
			{
				action(item);
			}
		}
	}

	[ValueConversion(typeof(Icon), typeof(BitmapSource))]
	public class IconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((Icon)value).ToBitmapSource();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Not implemented
			return null;
		}
	}

	[ValueConversion(typeof(Bitmap), typeof(BitmapSource))]
	public class BitmapConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((Bitmap)value).ToBitmapSource();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Not implemented
			return null;
		}
	}
}
