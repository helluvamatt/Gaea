using Gaea.UI.ViewModels;
using System.ComponentModel;
using System.Windows;
using Microsoft.Practices.Unity;

namespace Gaea.UI
{
	internal partial class ConfigWindow : Window
	{
		public ConfigWindow(ConfigWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			viewModel.SourceConfiguration += ViewModel_SourceConfiguration;
		}

		#region Event handlers

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		private void ViewModel_SourceConfiguration(object sender, System.EventArgs e)
		{
			(DataContext as ConfigWindowViewModel).Container.Resolve<SourceConfigWindow>().ShowDialog();
		}

		#endregion
	}
}
