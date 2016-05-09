using System.Globalization;

namespace Gaea.Services
{
	internal interface ILocalizationService
	{
		string GetLocalizedString(string key, params object[] parms);

		CultureInfo CurrentCulture { get; set; }
	}
}
