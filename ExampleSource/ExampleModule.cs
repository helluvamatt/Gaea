using Gaea.Api;
using Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ExampleSource
{
	[Module]
	public class ExampleModule : BaseSourceModule
	{
		public ExampleModule(IUnityContainer container) : base(container) { }

		protected override void RegisterSource()
		{
			Container.RegisterType<ISource, ExampleGaeaSource>(ExampleGaeaSource.NAME);
		}
	}
}
