using Gaea.Services.Data;
using Gaea.UI.Domain;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Gaea.Services
{
	internal interface IWallpaperService : IDisposable
	{
		void Initialize();
		void NextWallpaper();
		void OpenWallpaperLink();

		ICommand NextWallpaperCommand { get; }
		ICommand OpenWallpaperLinkCommand { get; }
		IEnumerable<SourceItem> Sources { get; }
		string SelectedSource { get; set; }
		bool CanConfigureSource { get; }

		event EventHandler<PostProcessCompletedEventArgs> PostProcessComplete;
	}
}
