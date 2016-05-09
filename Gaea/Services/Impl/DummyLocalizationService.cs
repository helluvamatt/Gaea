using System.Globalization;

namespace Gaea.Services.Impl
{
	internal class DummyLocalizationService : ILocalizationService
	{
		public CultureInfo CurrentCulture
		{
			get
			{
				return CultureInfo.GetCultureInfo("en-US");
			}

			set
			{
				// Do nothing
			}
		}

		public string GetLocalizedString(string key, params object[] parms)
		{
			return string.Format(key, parms);
		}
	}
}
