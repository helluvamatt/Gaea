using Gaea.Api;
using Gaea.Services.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;

namespace Gaea.Services
{
	public interface IWallpaperService : IDisposable
	{
		void Initialize();
		void NextWallpaper();
		void OpenWallpaperLink();

		ICommand NextWallpaperCommand { get; }
		ICommand OpenWallpaperLinkCommand { get; }
		IEnumerable<ISource> Sources { get; }
		ISource SelectedSource { get; set; }
		bool CanConfigureSource { get; }

		event EventHandler<PostProcessCompletedEventArgs> PostProcessComplete;
	}
}
