using Gaea.Api.Configuration;
using System;

namespace ExampleSource
{
	internal class ExampleSourceConfig
	{
		[SwitchConfigurationItem("Skip Third Image", true, Order = 0)]
		public bool SkipThirdImage { get; set; }

		[TimeSpanConfigurationItem("Fetch Delay", 0, 5000, 0, Order = 1)]
		public TimeSpan FetchDelay { get; set; }
	}
}
