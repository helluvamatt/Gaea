using Gaea.Api.Data;
using Gaea.Services;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace Gaea.UI.ViewModels
{
	internal class TrayIconViewModel : LocalizableViewModel
	{
		public IWallpaperService WallpaperService { get; private set; }

		private IUnityContainer _Container;

		public TrayIconViewModel(IUnityContainer container, IWallpaperService wallpaperService, IEventAggregator eventAggregator)
		{
			_Container = container;
			WallpaperService = wallpaperService;
			eventAggregator.GetEvent<WallpaperServiceErrorEvent>().Subscribe(OnError);
		}

		private void OnError(WallpaperServiceError error)
		{
			if (ShowCustomPopup != null)
			{
				ShowCustomPopup(error);
			}
		}

		public event Action<WallpaperServiceError> ShowCustomPopup;
	}
}
