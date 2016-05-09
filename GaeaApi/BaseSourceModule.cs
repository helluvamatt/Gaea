using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Gaea.Api
{
	public abstract class BaseSourceModule : IModule
	{
		protected IUnityContainer Container { get; private set; }

		public BaseSourceModule(IUnityContainer container)
		{
			Container = container;
		}

		public void Initialize()
		{
			// Register source type with IUnityContainer
			RegisterSource();
		}

		protected virtual void RegisterSource() { }
	}
}
