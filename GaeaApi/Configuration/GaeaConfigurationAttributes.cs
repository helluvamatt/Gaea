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
		public ConfigurationItemAttribute(string displayLabel)
		{
			DisplayLabel = displayLabel;
		}

		/// <summary>
		/// Displayed label for the item
		/// </summary>
		public string DisplayLabel { get; set; }

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
		public SwitchConfigurationItemAttribute(string displayLabel) : base(displayLabel) { }

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
		public StringConfigurationItemAttribute(string displayLabel) : base(displayLabel) { }

		/// <summary>
		/// Max length of the string, set to -1 if no limit
		/// </summary>
		public int MaxLength { get; set; }

		/// <summary>
		/// Min length of the string
		/// </summary>
		public int MinLength { get; set; }

		/// <summary>
		/// Default value for the item
		/// </summary>
		public string DefaultValue { get; set; }
	}


	/// <summary>
	/// Configruation item represented as an integer
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IntegerConfigurationItemAttribute : ConfigurationItemAttribute
	{
		public IntegerConfigurationItemAttribute(string displayLabel) : base(displayLabel) { }

		/// <summary>
		/// Default value for the item
		/// </summary>
		public int DefaultValue { get; set; }
	}

	/// <summary>
	/// Configuration item represented as an integer with a bounded range
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IntegerRangeConfigurationItemAttribute : IntegerConfigurationItemAttribute
	{
		public IntegerRangeConfigurationItemAttribute(string displayLabel, int minValue, int maxValue) : base(displayLabel)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}

		/// <summary>
		/// Maximum value of the item
		/// </summary>
		public int MaxValue { get; set; }

		/// <summary>
		/// Minimum value of the item
		/// </summary>
		public int MinValue { get; set; }
	}

	/// <summary>
	/// Configuration item represented as a choice between items
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ChoiceConfigurationItemAttribute : ConfigurationItemAttribute
	{
		public ChoiceConfigurationItemAttribute(string displayLabel) : base(displayLabel) { }

		/// <summary>
		/// The item's choices
		/// </summary>
		public List<Choice> Choices { get; set; }

		/// <summary>
		/// Default choice name for the item
		/// </summary>
		public string DefaultValue { get; set; }

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
	}

	/// <summary>
	/// Configuration item represented as a list of check boxes and (optionally) a place to write in options
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class MultiChoiceConfigurationItemAttribute : ChoiceConfigurationItemAttribute
	{
		public MultiChoiceConfigurationItemAttribute(string displayLabel) : base(displayLabel) { }

		/// <summary>
		/// Allows the user to "write in" choices.
		/// </summary>
		public bool AllowOtherChoices { get; set; }
	}

}
