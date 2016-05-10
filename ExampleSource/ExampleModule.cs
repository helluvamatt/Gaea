using Gaea.Api;
using Prism.Modularity;

namespace ExampleSource
{
	[Module]
	public class ExampleModule : BaseSourceModule
	{
		public ExampleModule(ISourceRegistry registry) : base(registry) { }

		protected override void RegisterSource()
		{
			Registry.Register(new ExampleGaeaSource());
		}
	}
}
