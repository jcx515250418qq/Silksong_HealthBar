using System;

namespace InControl
{
	public static class Logger
	{
		public static event Action<LogMessage> OnLogMessage;

		public static void LogInfo(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage logMessage = default(LogMessage);
				logMessage.Text = text;
				logMessage.Type = LogMessageType.Info;
				LogMessage obj = logMessage;
				Logger.OnLogMessage(obj);
			}
		}

		public static void LogWarning(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage logMessage = default(LogMessage);
				logMessage.Text = text;
				logMessage.Type = LogMessageType.Warning;
				LogMessage obj = logMessage;
				Logger.OnLogMessage(obj);
			}
		}

		public static void LogError(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage logMessage = default(LogMessage);
				logMessage.Text = text;
				logMessage.Type = LogMessageType.Error;
				LogMessage obj = logMessage;
				Logger.OnLogMessage(obj);
			}
		}
	}
}
