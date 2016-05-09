using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gaea.Api.Configuration
{
	/// <summary>
	/// Model representing the configuration of a given source. The 
	/// </summary>
	public class ConfigurationMetaModel
	{
		/// <summary>
		/// List of configuration items
		/// </summary>
		public Dictionary<PropertyInfo, ConfigurationItemAttribute> Items { get; private set; }

		public ConfigurationMetaModel()
		{
			Items = new Dictionary<PropertyInfo, ConfigurationItemAttribute>();
		}

		public bool CanConfigureSource
		{
			get
			{
				return Items.Count > 0;
			}
		}
	}
}
