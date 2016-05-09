using Gaea.Api.Data;
using Gaea.UI.Domain;
using Gaea.UI.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace Gaea.UI
{
	/// <summary>
	/// Interaction logic for TrayIcon.xaml
	/// </summary>
	internal partial class TrayIcon : TaskbarIcon
	{
		public TrayIcon(TrayIconViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
			viewModel.ShowCustomPopup += ViewModel_ShowCustomPopup;
			viewModel.WallpaperService.Initialize();
		}

		private void ViewModel_ShowCustomPopup(WallpaperServiceError obj)
		{
			Application.Current.Dispatcher.Invoke(() => {
				Balloon balloon = new Balloon(obj.Subject, obj.Message, MaterialDesignThemes.Wpf.PackIconKind.AlertCircle);
				balloon.BalloonClicked += Balloon_BalloonClicked;
				balloon.BalloonClosed += Balloon_BalloonClosed;
				ShowCustomBalloon(balloon, System.Windows.Controls.Primitives.PopupAnimation.Fade, null);
			});
		}

		private void Balloon_BalloonClosed(object sender, System.EventArgs e)
		{
			CloseBalloon();
		}

		private void Balloon_BalloonClicked(object sender, System.EventArgs e)
		{
			GlobalCommands.ShowConfigCommand.Execute(null);
			CloseBalloon();
		}
	}
}
