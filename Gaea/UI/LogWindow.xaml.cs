using Gaea.UI.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gaea.UI
{
	/// <summary>
	/// Interaction logic for LogWindow.xaml
	/// </summary>
	internal partial class LogWindow : Window
	{
		public LogWindow(LogWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			viewModel.LogEntries.CollectionChanged += LogEntries_CollectionChanged;
		}

		private void LogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if ((DataContext as LogWindowViewModel).ScrollToBottom)
			{
				if (logViewDataGrid.Items.Count > 0)
				{
					var border = VisualTreeHelper.GetChild(logViewDataGrid, 0) as Decorator;
					if (border != null)
					{
						var scroll = border.Child as ScrollViewer;
						if (scroll != null) scroll.ScrollToEnd();
					}
				}
			}
		}

		private void Window_Closed(object sender, System.EventArgs e)
		{
			(DataContext as LogWindowViewModel).LogEntries.CollectionChanged -= LogEntries_CollectionChanged;
		}
	}
}
