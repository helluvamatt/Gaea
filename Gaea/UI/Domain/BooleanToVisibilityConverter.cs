using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Gaea.UI.Domain
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	internal class BooleanToVisibilityConverter : IValueConverter
	{
		public BooleanToVisibilityConverter()
		{
			ShownVisibility = Visibility.Visible;
			HiddenVisibility = Visibility.Hidden;
		}

		public Visibility ShownVisibility { get; set; }
		public Visibility HiddenVisibility { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool shown = (bool)value;
			return shown ? ShownVisibility : HiddenVisibility;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
