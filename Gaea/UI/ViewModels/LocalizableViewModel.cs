using Gaea.Api.Infrastructure;
using Gaea.Services;

namespace Gaea.UI.ViewModels
{
	internal class LocalizableViewModel : DisposableAwareBindableBase
	{
		public LocalizableViewModel(ILocalizationService l10nService)
		{
			LocalizationService = l10nService;
		}

		public ILocalizationService LocalizationService { get; private set; }
	}
}
