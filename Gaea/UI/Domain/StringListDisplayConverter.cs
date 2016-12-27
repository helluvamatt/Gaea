using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gaea.UI.Domain
{
	[ValueConversion(typeof(ICollection<string>), typeof(string))]
	internal class StringListDisplayConverter : IValueConverter
	{
		public string EmptyText { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(string))
				throw new InvalidOperationException("The target must be a String");

			var collection = (ICollection<string>)value;
			if (collection.Count > 0)
			{
				return string.Join(", ", (ICollection<string>)value);
			}
			else
			{
				return EmptyText;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(ICollection<string>), typeof(FontStyle))]
	internal class StringListEmptyItalicConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(FontStyle))
				throw new InvalidOperationException("The target must be FontStyle");

			var collection = (ICollection<string>)value;
			return collection.Count > 0 ? FontStyles.Normal : FontStyles.Italic;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
