using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Gaea.UI.Domain
{
	class LogEntry
	{
		public string Message { get; set; }
		public string Timestamp { get; set; }
		public string Level { get; set; }
	}

	class LogLevelToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string level = value as string;
			switch(level)
			{
				case "Debug":
					return new SolidColorBrush(Utils.FromHex("#C8E6C9"));
				case "Info":
					return new SolidColorBrush(Utils.FromHex("#BBDEFB"));
				case "Warn":
					return new SolidColorBrush(Utils.FromHex("#FFE0B2"));
				case "Error":
					return new SolidColorBrush(Utils.FromHex("#FFCDD2"));
				default:
					return DependencyProperty.UnsetValue;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
