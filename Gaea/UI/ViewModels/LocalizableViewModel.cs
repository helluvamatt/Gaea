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
	}
}
