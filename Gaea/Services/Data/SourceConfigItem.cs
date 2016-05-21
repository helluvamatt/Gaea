using Gaea.Api.Configuration;
using Prism.Mvvm;
using System;

namespace Gaea.Services.Data
{
	internal class SourceConfigItem : BindableBase, IComparable
	{
		public SourceConfigItem(string name, ConfigurationItemAttribute attr)
		{
			Name = name;
			Attribute = attr;
		}

		public string Name { get; private set; }

		public ConfigurationItemAttribute Attribute { get; private set; }

		private object _Value;
		public object Value
		{
			get
			{
				return _Value;
			}
			set
			{
				SetProperty(ref _Value, value);
			}
		}

		public int CompareTo(object obj)
		{
			var other = obj as SourceConfigItem;
			int result = Attribute.Order.CompareTo(other.Attribute.Order);
			if (result == 0)
			{
				result = Attribute.DisplayLabel.CompareTo(other.Attribute.DisplayLabel);
			}
			return result;
		}


	}
}
