using Gaea.Services;
using Prism.Commands;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;

namespace Gaea.UI.ViewModels
{
	internal class ConfigWindowViewModel : LocalizableViewModel
	{

		#region Properties

		public IConfiguration Configuration { get; private set; }
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

		#endregion

		public ConfigWindowViewModel(IConfiguration configuration, IWallpaperService wallpaperService)
		{
			Configuration = configuration;
			WallpaperService = wallpaperService;
			WallpaperService.PostProcessComplete += WallpaperService_PostProcessComplete;
		}

		private void WallpaperService_PostProcessComplete(object sender, Services.Data.PostProcessCompletedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() => {
				PreviewImage = e.Image.ProcessedImage;
			});
		}

		public ICommand SourceConfigurationCommand
		{
			get
			{
				return new DelegateCommand(DoSourceConfiguration);
			}
		}

		private void DoSourceConfiguration()
		{
			var sourceConfigWindow = Container.Resolve<SourceConfigWindow>();
			sourceConfigWindow.ShowDialog();
		}
	}
}
