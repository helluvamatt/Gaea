using Gaea.Api;
using Gaea.Api.Configuration;
using Gaea.Services.Data;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Gaea.Services
{
	internal interface IWallpaperService : IDisposable
	{
		/// <summary>
		/// Initialize the wallpaper service
		/// </summary>
		void Initialize();

		/// <summary>
		/// Load the next image
		/// </summary>
		void NextWallpaper();

		/// <summary>
		/// Open the current image's URL in a browser
		/// </summary>
		void OpenWallpaperLink();

		/// <summary>
		/// Configure the currently selected source with the given config object
		/// </summary>
		/// <param name="configuration">Configuration object</param>
		void ConfigureCurrentSource(object configuration);

		/// <summary>
		/// Command that will load and process the next wallpaper
		/// </summary>
		ICommand NextWallpaperCommand { get; }

		/// <summary>
		/// Command that will open the current image's URL in a browser
		/// </summary>
		ICommand OpenWallpaperLinkCommand { get; }

		/// <summary>
		/// Collection representing the sources loaded
		/// </summary>
		IEnumerable<SourceItem> Sources { get; }

		/// <summary>
		/// Currently selected source
		/// </summary>
		ISource CurrentSource { get; }

		/// <summary>
		/// Model representing the configuration of the current source
		/// </summary>
		ConfigurationMetaModel CurrentSourceConfigurationMetaModel { get; }

		/// <summary>
		/// Does the source have configuration?
		/// </summary>
		bool CanConfigureSource { get; }

		/// <summary>
		/// Name of the currently selected source
		/// </summary>
		string SelectedSource { get; set; }

		/// <summary>
		/// Image post processing is complete, and the image is ready for display
		/// </summary>
		event EventHandler<PostProcessCompletedEventArgs> PostProcessComplete;
	}
}
