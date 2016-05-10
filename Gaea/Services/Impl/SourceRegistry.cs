using Gaea.Api;
using Microsoft.Practices.Unity;
using System;

namespace Gaea.Services.Impl
{
	public class SourceRegistry : ISourceRegistry
	{
		private IUnityContainer _Container;

		public SourceRegistry(IUnityContainer container)
		{
			_Container = container;
		}

		public void Register(ISource source)
		{
			_Container.RegisterInstance(source.GetName(), source);
		}
	}
}
