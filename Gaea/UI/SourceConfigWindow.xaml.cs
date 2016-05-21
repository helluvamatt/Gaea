using Gaea.Api.Configuration;
using Gaea.UI.Domain;
using Gaea.UI.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Gaea.UI
{
	/// <summary>
	/// Interaction logic for SourceConfigWindow.xaml
	/// </summary>
	internal partial class SourceConfigWindow : Window
	{
		public SourceConfigWindow(SourceConfigWindowViewModel viewModel)
		{
			viewModel.DismissDialog += ViewModel_DismissDialog;
			DataContext = viewModel;
			InitializeComponent();
		}

		#region Event Handlers

		private void ViewModel_DismissDialog(object sender, DismissDialogEventArgs e)
		{
			DialogResult = e.Result;
			Close();
		}

		#endregion

	}
}
