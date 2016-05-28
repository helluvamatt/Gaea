using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gaea.UI.Domain
{
	internal class StringConfigurationItemValidationRule : ValidationRule
	{
		public StringConfigurationItemValidationRuleConfiguration Configuration { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			string val = (value ?? "").ToString();

			if (Configuration.Required && string.IsNullOrWhiteSpace(val))
			{
				return new ValidationResult(false, "Field is required.");
			}

			if (Configuration.MaxLength > 0 && val.Length > Configuration.MaxLength)
			{
				return new ValidationResult(false, string.Format("Value is longer than {0} characters.", Configuration.MaxLength));
			}

			return ValidationResult.ValidResult;
		}
	}

	internal class StringConfigurationItemValidationRuleConfiguration : DependencyObject
	{
		public static readonly DependencyProperty MaxLengthDependencyProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(StringConfigurationItemValidationRuleConfiguration), new FrameworkPropertyMetadata(0));
		public static readonly DependencyProperty RequiredDependencyProperty = DependencyProperty.Register("Required", typeof(bool), typeof(StringConfigurationItemValidationRuleConfiguration), new FrameworkPropertyMetadata(false));

		public int MaxLength
		{
			get
			{
				return (int)GetValue(MaxLengthDependencyProperty);
			}
			set
			{
				SetValue(MaxLengthDependencyProperty, value);
			}
		}

		public bool Required
		{
			get
			{
				return (bool)GetValue(RequiredDependencyProperty);
			}
			set
			{
				SetValue(RequiredDependencyProperty, value);
			}
		}
	}
}
