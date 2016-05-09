using Gaea.Services.Data;
using Prism.Events;
using Gaea.UI.Domain;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Gaea.Services
{
	public class NLogEventEmitter
	{
		[Dependency]
		public IEventAggregator EventAggregator
		{
			set
			{
				lock (_lockObject)
				{
					_LogEvent = value.GetEvent<LogEvent>();
					while (_PreContainerQueue.Count > 0)
					{
						_LogEvent.Publish(_PreContainerQueue.Dequeue());
					}
				}
			}
		}

		public NLogEventEmitter() { }

		private static LogEvent _LogEvent;
		private static object _lockObject = new { };

		private static Queue<LogEntry> _PreContainerQueue = new Queue<LogEntry>();

		public static void HandleLogEvent(string date, string level, string message)
		{
			LogEntry entry = new LogEntry { Level = level, Timestamp = date, Message = message };
			if (_LogEvent != null)
			{
				_LogEvent.Publish(entry);
			}
			else
			{
				_PreContainerQueue.Enqueue(entry);
			}
			
		}
	}
}
