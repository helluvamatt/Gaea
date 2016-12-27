﻿using Gaea.Api.Configuration;
using System;
using System.Collections.Generic;

namespace ExampleSource
{
	internal class ExampleSourceConfig
	{
		[SwitchConfigurationItem("Skip Third Image", true, Order = 0)]
		public bool SkipThirdImage { get; set; }

		[TimeSpanConfigurationItem("Fetch Delay", 0, 5000, 0, Order = 1)]
		public TimeSpan FetchDelay { get; set; }

		[StringConfigurationItem("Test String Item", "", Order = 2, MaxLength = 16, Required = true )]
		public string TestStringItem { get; set; }

		[MultiChoiceConfigurationItem("Test Multi Choice Item", Order = 3, AllowOtherChoices = true)]
		[ChoiceConfigurationItemChoice("Item 1")]
		[ChoiceConfigurationItemChoice("Item 2")]
		[ChoiceConfigurationItemChoice("Item 3")]
		[ChoiceConfigurationItemChoice("Item 4")]
		public IEnumerable<string> TestMultiChoiceItem { get; set; }
	}
}
