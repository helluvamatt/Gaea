using Gaea.Api.Infrastructure;
using Gaea.Services;
using Microsoft.Practices.Unity;

namespace Gaea.UI.ViewModels
{
	internal class LocalizableViewModel : DisposableAwareBindableBase
	{
		[Dependency]
		public ILocalizationService LocalizationService { get; set; }

		[Dependency]
		public IUnityContainer Container { get; set; }

		/// <summary>
		/// Get a localized string from resources
		/// </summary>
		/// <param name="key">Name of the localized string</param>
		/// <param name="parms">Parameters that may be formatted into the localized string using string.Format</param>
		/// <returns>Formatted localized string</returns>
		protected string GetString(string key, params object[] parms)
		{
			if (LocalizationService == null) return "!!" + key + "!!";
			return LocalizationService.GetLocalizedString(key, parms);
		}
	}
}
