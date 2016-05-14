using System;
using System.Windows;
using System.Windows.Controls;
using Gaea.Api.Configuration;
using Gaea.Services.Data;

namespace Gaea.UI.Domain
{
	internal class SourceConfigTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) return null;
			FrameworkElement element = container as FrameworkElement;
			SourceConfigItem configItem = item as SourceConfigItem;
			if (configItem.Attribute is SwitchConfigurationItemAttribute)
				return element.FindResource("SwitchDataTemplate") as DataTemplate;
			else if (configItem.Attribute is StringConfigurationItemAttribute)
				return element.FindResource("StringDataTemplate") as DataTemplate;
			else if (configItem.Attribute is NumericConfigurationItemAttribute)
				return element.FindResource("NumericDataTemplate") as DataTemplate;
			else if (configItem.Attribute is ChoiceConfigurationItemAttribute)
				return element.FindResource("ChoiceDataTemplate") as DataTemplate;
			else if (configItem.Attribute is MultiChoiceConfigurationItemAttribute)
				return element.FindResource("MultiChoiceDataTemplate") as DataTemplate;
			else
				throw new NotSupportedException("Type not supported: " + configItem.Attribute.GetType().Name);
		}
	}
}
