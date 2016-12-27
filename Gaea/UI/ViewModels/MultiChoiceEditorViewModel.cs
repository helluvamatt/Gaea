using Gaea.Api.Configuration;
using Gaea.Services.Data;
using Gaea.UI.Domain;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gaea.UI.ViewModels
{
	internal class MultiChoiceEditorViewModel
	{
		#region Private members

		private SourceConfigItem _MultiChoiceItem;

		#endregion

		public MultiChoiceEditorViewModel(SourceConfigItem multiChoiceItem)
		{
			_MultiChoiceItem = multiChoiceItem;
			Items = new List<MultiChoiceItem>();
			// TODO Populate Items using the Choices collection 
			var attr = (MultiChoiceConfigurationItemAttribute)multiChoiceItem.Attribute;

			AcceptCommand = new DelegateCommand(DoAccept);
		}

		#region Properties

		public ICollection<MultiChoiceItem> Items { get; private set; }

		public ICommand AcceptCommand { get; private set; }

		#endregion

		#region Events

		public event EventHandler<DismissDialogEventArgs> DismissDialog;

		private void RaiseDismissDialog(bool result)
		{
			DismissDialog?.Invoke(this, new DismissDialogEventArgs { Result = result });
		}

		#endregion

		#region Methods

		private void DoAccept()
		{
			// TODO Persist selections back to the model
			RaiseDismissDialog(true);
		}

		#endregion

		#region ViewModel helper classes

		public class MultiChoiceItem
		{
			public MultiChoiceItem(string text, bool isChecked)
			{
				Text = text;
				IsChecked = IsChecked;
			}

			public bool IsChecked { get; set; }

			public string Text { get; private set; }
		}

		#endregion
	}
}
