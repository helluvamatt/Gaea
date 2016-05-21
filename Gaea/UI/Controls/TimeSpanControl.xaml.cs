using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gaea.UI.Controls
{
	/// <summary>
	/// Interaction logic for TimeSpanControl.xaml
	/// </summary>
	public partial class TimeSpanControl : UserControl
	{
		

		private static readonly long[] multipliers = new long[] { 1, 1000, 60000, 360000, 8640000 }; // ms, sec, min, hour, day

		public TimeSpanControl()
		{
			InitializeComponent();
		}

		#region Event handlers

		private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				string text = (string)e.DataObject.GetData(typeof(string));
				if (!IsNumeric(text))
				{
					e.CancelCommand();
				}
			}
			else
			{
				e.CancelCommand();
			}
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsNumeric(e.Text);
		}

		private static void OnValuePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is TimeSpan == false) return;
			TimeSpanControl control = source as TimeSpanControl;
			TimeSpan newValue = (TimeSpan)e.NewValue;
			control.Multiplier = FindHighestWholeMultiplier(newValue);
			// The change handler on Multiplier will set the BaseValue based on the newly changed Value
		}

		private static object OnCoerceValueProperty(DependencyObject source, object data)
		{
			TimeSpanControl control = source as TimeSpanControl;
			TimeSpan newValue = (TimeSpan)data;
			if (newValue > control.MaxValue) newValue = control.MaxValue;
			if (newValue < control.MinValue) newValue = control.MinValue;
			return newValue;
		}

		private static void OnBaseValuePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is long == false) return;
			TimeSpanControl control = source as TimeSpanControl;
			control.Value = TimeSpan.FromMilliseconds((long)e.NewValue * multipliers[control.Multiplier]);
		}

		private static void OnMultiplierPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is int == false) return;
			TimeSpanControl control = source as TimeSpanControl;
			long valueMs = control.Value.Ticks / TimeSpan.TicksPerMillisecond;
			control.BaseValue = valueMs / multipliers[(int)e.NewValue];
		}

		private static void OnMinValuePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			TimeSpanControl control = source as TimeSpanControl;
			TimeSpan newMinValue = (TimeSpan)e.NewValue;
			if (control.Value < newMinValue) control.Value = newMinValue;
		}

		private static void OnMaxValuePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			TimeSpanControl control = source as TimeSpanControl;
			TimeSpan newMaxValue = (TimeSpan)e.NewValue;
			if (control.Value > newMaxValue) control.Value = newMaxValue;
			foreach (var item in control.cbxMultipliers.Items)
			{
				(item as ComboBoxItem).IsEnabled = true;
			}

			int mult = FindHighestWholeMultiplier(newMaxValue);
			for (int i = mult + 1; i < multipliers.Length; i++)
			{
				(control.cbxMultipliers.Items[i] as ComboBoxItem).IsEnabled = false;
			}
		}

		#endregion

		#region Utility methods

		private bool IsNumeric(string text)
		{
			long val;
			return IsNumeric(text, out val);
		}

		private bool IsNumeric(string text, out long val)
		{
			return long.TryParse(text, out val);
		}

		private static int FindHighestWholeMultiplier(TimeSpan rawValue)
		{
			// Need to find the largest unit which still has the value as a whole number
			// The smallest unit possible is the millisecond (even though TimeSpan supports 1/10000ths of a second, aka ticks)
			long value = rawValue.Ticks / TimeSpan.TicksPerMillisecond;
			int i;
			long baseValue = value;
			for (i = 0; i < multipliers.Length; i++)
			{
				baseValue = value / multipliers[i];
				if ((i + 1) < multipliers.Length && value % multipliers[i + 1] != 0)
				{
					break;
				}
			}
			if (i >= multipliers.Length) i = 0;
			return i;
		}

		#endregion

		#region Dependency properties

		public static readonly DependencyProperty ValueDependencyProperty = DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeSpanControl), new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(0), OnValuePropertyChanged, OnCoerceValueProperty) { BindsTwoWayByDefault = true });
		public static readonly DependencyProperty MinValueDependencyProperty = DependencyProperty.Register("MinValue", typeof(TimeSpan), typeof(TimeSpanControl), new FrameworkPropertyMetadata(TimeSpan.MinValue));
		public static readonly DependencyProperty MaxValueDependencyProperty = DependencyProperty.Register("MaxValue", typeof(TimeSpan), typeof(TimeSpanControl), new FrameworkPropertyMetadata(TimeSpan.MaxValue, OnMaxValuePropertyChanged));
		protected static readonly DependencyProperty BaseValueDependencyProperty = DependencyProperty.Register("BaseValue", typeof(long), typeof(TimeSpanControl), new FrameworkPropertyMetadata(OnBaseValuePropertyChanged));
		protected static readonly DependencyProperty MultiplierDependencyProperty = DependencyProperty.Register("Multiplier", typeof(int), typeof(TimeSpanControl), new FrameworkPropertyMetadata(OnMultiplierPropertyChanged));

		public TimeSpan Value
		{
			get
			{
				return (TimeSpan)GetValue(ValueDependencyProperty);
			}
			set
			{
				SetValue(ValueDependencyProperty, value);
			}
		}

		public TimeSpan MinValue
		{
			get
			{
				return (TimeSpan)GetValue(MinValueDependencyProperty);
			}
			set
			{
				SetValue(MinValueDependencyProperty, value);
			}
		}

		public TimeSpan MaxValue
		{
			get
			{
				return (TimeSpan)GetValue(MaxValueDependencyProperty);
			}
			set
			{
				SetValue(MaxValueDependencyProperty, value);
			}
		}

		protected long BaseValue
		{
			get
			{
				return (long)GetValue(BaseValueDependencyProperty);
			}
			set
			{
				SetValue(BaseValueDependencyProperty, value);
			}
		}

		protected int Multiplier
		{
			get
			{
				return (int)GetValue(MultiplierDependencyProperty);
			}
			set
			{
				SetValue(MultiplierDependencyProperty, value);
			}
		}

		#endregion
	}
}
