using System.Collections.Generic;
using System.Reflection;

namespace Gaea.Api.Configuration
{
	/// <summary>
	/// Model representing the configuration of a given source. The 
	/// </summary>
	public class ConfigurationMetaModel
	{
		public Dictionary<PropertyInfo, ConfigurationItemAttribute> Data { get; private set; }

		public ConfigurationMetaModel()
		{
			Data = new Dictionary<PropertyInfo, ConfigurationItemAttribute>();
		}

		public bool CanConfigureSource
		{
			get
			{
				return Data.Count > 0;
			}
		}
	}
}
