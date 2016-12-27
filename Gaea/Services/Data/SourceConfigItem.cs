using Gaea.Api.Configuration;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Gaea.Services.Data
{
	internal class SourceConfigItem : BindableBase, IComparable
	{
		public SourceConfigItem(string name, ConfigurationItemAttribute attr)
		{
			Name = name;
			Attribute = attr;
			DeleteItemCommand = new DelegateCommand<string>(DeleteItem);
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

		public ICommand DeleteItemCommand { get; private set; }

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

		private void DeleteItem(string item)
		{
			var collection = (ICollection<string>)Value;
			collection.Remove(item);
		}

	}
}
