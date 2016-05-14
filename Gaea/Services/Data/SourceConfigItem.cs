using Gaea.Api.Configuration;
using System;

namespace Gaea.Services.Data
{
	internal class SourceConfigItem : IComparable
	{
		public string Name { get; set; }

		public ConfigurationItemAttribute Attribute { get; set; }

		public object Value { get; set; }

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
