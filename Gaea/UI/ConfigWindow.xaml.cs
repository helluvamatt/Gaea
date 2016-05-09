using Gaea.UI.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace Gaea.UI
{
	internal partial class ConfigWindow : Window
	{
		public ConfigWindow(ConfigWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}

		#region Event handlers

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		#endregion
	}
}
