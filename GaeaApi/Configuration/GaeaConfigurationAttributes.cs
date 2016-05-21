using System;
using System.Collections.Generic;

namespace Gaea.Api.Configuration
{
	/// <summary>
	/// Marker attribute for properties that should be considered configurable in the configuration object provided by the source
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ConfigurationItemAttribute : Attribute
	{
		public ConfigurationItemAttribute(string displayLabel, Type allowedType) : this(displayLabel, new[] {  allowedType }) { }

		public ConfigurationItemAttribute(string displayLabel, Type[] allowedTypes)
		{
			DisplayLabel = displayLabel;
			AllowedTypes = allowedTypes;
		}

		/// <summary>
		/// Displayed label for the item
		/// </summary>
		public string DisplayLabel { get; set; }

		/// <summary>
		/// Allowed types for the value
		/// </summary>
		public Type[] AllowedTypes { get; set; }

		/// <summary>
		/// Index used for ordering configuration items in the UI. You can ignore this and the properties will be sorted by DisplayLabel.
		/// </summary>
		public int Order { get; set; }
	}

	/// <summary>
	/// Configuration item represented as a switch having two states: on or off
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SwitchConfigurationItemAttribute : ConfigurationItemAttribute
	{
		public SwitchConfigurationItemAttribute(string displayLabel, bool defaultValue) : base(displayLabel, typeof(bool))
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Default state for this configuration item
		/// </summary>
		public bool DefaultValue { get; set; }
	}

	/// <summary>
	/// Configuration item represented as a string of arbitrary length
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class StringConfigurationItemAttribute : ConfigurationItemAttribute
	{
		public StringConfigurationItemAttribute(string displayLabel, string defaultValue) : base(displayLabel, typeof(string))
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Max length of the string, set to -1 if no limit
		/// </summary>
		public int MaxLength { get; set; }

		/// <summary>
		/// Default value for the item
		/// </summary>
		public string DefaultValue { get; set; }
	}

	/// <summary>
	/// Configuration item represented as a numeric slider with a bounded range
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class TimeSpanConfigurationItemAttribute : ConfigurationItemAttribute
	{
		public TimeSpanConfigurationItemAttribute(string displayLabel, long minValue, long maxValue, long defaultValue) : base(displayLabel, new[] { typeof(TimeSpan), typeof(long), typeof(ulong) })
		{
			MinValue = TimeSpan.FromMilliseconds(minValue);
			MaxValue = TimeSpan.FromMilliseconds(maxValue);
			DefaultValue = TimeSpan.FromMilliseconds(defaultValue);
			if (MinValue > MaxValue) throw new InvalidOperationException(string.Format("Invalid range: [{0}, {0}]", minValue, maxValue));
		}

		/// <summary>
		/// Maximum value of the item
		/// </summary>
		public TimeSpan MaxValue { get; set; }

		/// <summary>
		/// Minimum value of the item
		/// </summary>
		public TimeSpan MinValue { get; set; }

		/// <summary>
		/// Default value for the item
		/// </summary>
		public TimeSpan DefaultValue { get; set; }
	}

	/// <summary>
	/// Configuration item represented as a list of check boxes and (optionally) a place to write in options
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class MultiChoiceConfigurationItemAttribute : ConfigurationItemAttribute
	{
		public MultiChoiceConfigurationItemAttribute(string displayLabel) : base(displayLabel, new[] { typeof(IEnumerable<string>) }) { }

		/// <summary>
		/// Allows the user to "write in" choices.
		/// </summary>
		public bool AllowOtherChoices { get; set; }

		/// <summary>
		/// The item's choices
		/// </summary>
		public Choice[] Choices { get; set; }

		/// <summary>
		/// Default choice name for the item
		/// </summary>
		public string DefaultValue { get; set; }
	}

	/// <summary>
	/// Specifies a choice 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class ChoiceConfigurationItemChoiceAttribute : Attribute
	{
		public ChoiceConfigurationItemChoiceAttribute(string choiceName, string choiceDisplayLabel)
		{
			Choice = new Choice { Name = choiceName, DisplayLabel = choiceDisplayLabel };
		}

		/// <summary>
		/// The choice for this attribute
		/// </summary>
		public Choice Choice { get; set; }
	}

	/// <summary>
	/// Object representing a choice for a ChoiceConfigurationItem
	/// </summary>
	public class Choice
	{
		/// <summary>
		/// Name of the choice, used as a key in persisting the value of the choice
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Displayed text associated with the choice
		/// </summary>
		public string DisplayLabel { get; set; }
	}

	/// <summary>
	/// Thrown when the type of a given property on the configuration object is not allowed by the attribute on that property.
	/// </summary>
	/// <example>
	/// The following would throw this exception when processed:
	/// 
	/// <code>
	/// class SourceConfiguration
	/// {
	///		[NumericConfigurationItemAttribute("Some Configuration Parameter")]
	///		public string SomeConfigurationParameter { get; set; }
	/// }
	/// </code>
	/// </example>
	public class TypeNotAllowedException : Exception { }

}
