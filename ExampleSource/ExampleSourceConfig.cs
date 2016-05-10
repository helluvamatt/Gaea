using Gaea.Api.Configuration;

namespace ExampleSource
{
	internal class ExampleSourceConfig
	{
		[SwitchConfigurationItem("Skip Third Image", DefaultValue = true, Order = 0)]
		public bool SkipThirdImage { get; set; }

		[IntegerRangeConfigurationItem("Fetch Delay (ms)", 0, 5000, DefaultValue = 0, Order = 1)]
		public int FetchDelayMs { get; set; }
	}
}
