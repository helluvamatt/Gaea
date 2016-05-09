using Prism.Mvvm;
using System;
using System.Runtime.CompilerServices;

namespace Gaea.Api.Infrastructure
{
	public class DisposableAwareBindableBase : BindableBase
	{
		/// <summary>
		/// Set a property, notifying property change listeners. If the property is of a type that implements IDisposable, Dispose() will be called on the old outgoing value if it is not null.
		/// </summary>
		/// <typeparam name="T">Type of property</typeparam>
		/// <param name="storage">Backing storage for property, passed by reference</param>
		/// <param name="value">New value of the property</param>
		/// <param name="propertyName">Property name</param>
		/// <returns>True if the property was set succesfully, false otherwise</returns>
		protected virtual bool SetAndDisposeProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			T old = storage;
			if (!SetProperty(ref storage, value, propertyName)) return false;
			if (old != null && old is IDisposable)
			{
				(old as IDisposable).Dispose();
			}
			return true;
		}
	}
}
