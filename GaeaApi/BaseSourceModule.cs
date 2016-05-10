using Prism.Modularity;

namespace Gaea.Api
{
	public abstract class BaseSourceModule : IModule
	{
		protected ISourceRegistry Registry { get; private set; }

		public BaseSourceModule(ISourceRegistry registry)
		{
			Registry = registry;
		}

		public void Initialize()
		{
			// Register source type
			RegisterSource();
		}

		protected virtual void RegisterSource() { }
	}
}
