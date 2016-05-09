using Gaea.Services;
using System.Drawing;
using System.Windows;

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

		public ConfigWindowViewModel(ILocalizationService l10nService, IConfiguration configuration, IWallpaperService wallpaperService) : base(l10nService)
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
	}
}
