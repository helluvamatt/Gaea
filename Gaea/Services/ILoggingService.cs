using Prism.Logging;
using System;

namespace Gaea.Services
{
	public interface ILoggingService : ILoggerFacade
	{
		void Error(string message, params object[] parms);
		void Warning(string message, params object[] parms);
		void Info(string message, params object[] parms);
		void Debug(string message, params object[] parms);
		void Exception(Exception ex, string message, params object[] parms);
	}
}
