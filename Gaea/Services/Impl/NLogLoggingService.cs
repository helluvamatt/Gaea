using System;
using NLog;
using Prism.Logging;

namespace Gaea.Services.Impl
{
	class NLogLoggingService : ILoggingService
	{
		private ILogger _Logger;

		public NLogLoggingService(ILogger logger)
		{
			_Logger = logger;
		}

		public void Debug(string message, params object[] parms)
		{
			_Logger.Debug(message, parms);
		}

		public void Error(string message, params object[] parms)
		{
			_Logger.Error(message, parms);
		}

		public void Exception(Exception ex, string message, params object[] parms)
		{
			_Logger.Error(ex, message, parms);
		}

		public void Info(string message, params object[] parms)
		{
			_Logger.Info(message, parms);
		}

		public void Log(string message, Category category, Priority priority)
		{
			message = string.Format("Priority=[{0,6}] {1}", priority, message);
			switch (category)
			{
				case Category.Debug:
					_Logger.Debug(message);
					break;
				case Category.Warn:
					_Logger.Warn(message);
					break;
				case Category.Exception:
					_Logger.Error(message);
					break;
				case Category.Info:
				default:
					_Logger.Info(message);
					break;
			}
		}

		public void Warning(string message, params object[] parms)
		{
			_Logger.Warn(message, parms);
		}
	}
}
