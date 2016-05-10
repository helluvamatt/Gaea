using Gaea.Services;
using Gaea.Services.Data;
using Gaea.UI.Domain;
using Prism.Events;
using System.Collections.ObjectModel;

namespace Gaea.UI.ViewModels
{
	internal class LogWindowViewModel : LocalizableViewModel
	{
		private IEventAggregator _EventAggregator;

		public LogWindowViewModel(IEventAggregator eventAggregator)
		{
			LogEntries = new ObservableCollection<LogEntry>();
			_EventAggregator = eventAggregator;
			_EventAggregator.GetEvent<LogEvent>().Subscribe((e) => {
				App.Current.Dispatcher.Invoke(() => {
					LogEntries.Add(e);
				});
			});
			ScrollToBottom = true;
		}

		public ObservableCollection<LogEntry> LogEntries { get; private set; }

		public bool ScrollToBottom { get; set; }
		
	}
}
