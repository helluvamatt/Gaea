using Gaea.UI.Domain;
using Gaea.UI.ViewModels;
using System.Windows;

namespace Gaea.UI
{
	/// <summary>
	/// Interaction logic for SourceConfigWindow.xaml
	/// </summary>
	internal partial class SourceConfigWindow : Window
	{
		public SourceConfigWindow(SourceConfigWindowViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			viewModel.DismissDialog += ViewModel_DismissDialog;
		}

		private void ViewModel_DismissDialog(object sender, DismissDialogEventArgs e)
		{
			DialogResult = e.Result;
			Close();
		}
	}
}
