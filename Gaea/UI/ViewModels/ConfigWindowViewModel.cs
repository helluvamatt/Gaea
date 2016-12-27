using Gaea.Services;
using Prism.Commands;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Prism.Events;
using Gaea.Api.Data;
using System;

namespace Gaea.UI.ViewModels
{
	internal class ConfigWindowViewModel : LocalizableViewModel
	{

		#region Properties

		public IConfigurationService Configuration { get; private set; }
		public IWallpaperService WallpaperService { get; private set; }

		private Bitmap _PreviewImage;
		public Bitmap PreviewImage
		{
			get
			{
				return _PreviewImage;
			}
			set
			{
				SetAndDisposeProperty(ref _PreviewImage, value);
			}
		}

		private bool _IsLoading;
		public bool IsLoading
		{
			get
			{
				return _IsLoading;
			}
			set
			{
				SetProperty(ref _IsLoading, value);
			}
		}

		#endregion

		public ConfigWindowViewModel(IConfigurationService configuration, IWallpaperService wallpaperService, IEventAggregator eventAggregator)
		{
			Configuration = configuration;
			WallpaperService = wallpaperService;
			WallpaperService.PostProcessComplete += WallpaperService_PostProcessComplete;
			SourceConfigurationCommand = new DelegateCommand(DoSourceConfiguration);
			eventAggregator.GetEvent<WallpaperChangingEvent>().Subscribe((manual) => { IsLoading = true; });
			eventAggregator.GetEvent<WallpaperServiceErrorEvent>().Subscribe((error) => { IsLoading = false; });
		}

		#region Event handlers

		private void WallpaperService_PostProcessComplete(object sender, Services.Data.PostProcessCompletedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() => {
				PreviewImage = e.Image.ProcessedImage;
				IsLoading = false;
			});
		}

		#endregion

		#region Events

		public event EventHandler SourceConfiguration;
		private void RaiseSourceConfiguration()
		{
			SourceConfiguration?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region Commands

		public ICommand SourceConfigurationCommand { get; private set; }

		private void DoSourceConfiguration()
		{
			RaiseSourceConfiguration();
		}

		#endregion
	}
}
