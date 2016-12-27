using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaea.Services.Data
{
	internal static class Extensions
	{
		/// <summary>
		/// Get a registry value as a strong type
		/// </summary>
		/// <remarks>
		/// This method performs no type-checking on the registry side. It assums the value is the correct type that can be casted to the target type. You may receive a TypeCastException if you don't heed this warning!
		/// </remarks>
		/// <typeparam name="T">Target type</typeparam>
		/// <param name="key">Open registry key</param>
		/// <param name="name">Name of the value</param>
		/// <returns>Strongly-typed value</returns>
		public static T GetValue<T>(this RegistryKey key, string name)
		{
			return key.GetValue<T>(name, default(T));
		}

		/// <summary>
		/// Get a registry value as a strong type
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <param name="key">Open registry key</param>
		/// <param name="name">Name of the value</param>
		/// <param name="defaultValue">Default value to use if the key cannot be found</param>
		/// <returns>Strongly-typed value</returns>
		public static T GetValue<T>(this RegistryKey key, string name, T defaultValue)
		{
			object value = key.GetValue(name);
			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				return (T)value;
			}
		}
	}
}
