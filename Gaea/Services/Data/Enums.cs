using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gaea.Services.Data
{
	enum AutomaticMode
	{
		None, OnStartup, TimeInterval
	}

	internal class EnumMatchConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
				return false;
			string checkValue = value.ToString();
			string targetValue = parameter.ToString();
			return string.Equals(checkValue, targetValue, StringComparison.InvariantCultureIgnoreCase);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
				return null;
			bool useValue = (bool)value;
			if (useValue)
			{
				string targetValue = value.ToString();
				return Enum.Parse(targetType, targetValue);
			}
			return null;
		}
	}
}
