using UnityEngine;

namespace TeamCherry.PS5
{
	public static class PlaystationLogHandler
	{
		private static IMessagePrinter printer;

		public static IMessagePrinter Printer
		{
			get
			{
				if (printer == null)
				{
					printer = new DummyPrint();
				}
				return printer;
			}
			set
			{
				printer = value;
			}
		}

		public static void Log(object message)
		{
			LogMessage(message, Color.white);
		}

		public static void LogWarning(object message)
		{
			LogMessage(message, Color.yellow, Message.MessageType.Warning);
		}

		public static void LogError(object message)
		{
			LogMessage(message, Color.red, Message.MessageType.Error);
		}

		public static void Log(object message, Object context)
		{
			LogMessage(message, Color.white, Message.MessageType.Log, context);
		}

		public static void LogWarning(object message, Object context)
		{
			LogMessage(message, Color.yellow, Message.MessageType.Warning, context);
		}

		public static void LogError(object message, Object context)
		{
			LogMessage(message, Color.red, Message.MessageType.Error, context);
		}

		public static void LogMessage(object message, Color color, Message.MessageType messageType = Message.MessageType.Log)
		{
			PrintMessageWithStackTrace(new Message(message.ToString(), color, messageType));
		}

		public static void LogMessage(object message, Color color, Message.MessageType messageType, Object context)
		{
			if (UnityThreadContext.IsUnityMainThread)
			{
				PrintMessageWithStackTrace(new Message(message.ToString(), color, messageType, context));
				return;
			}
			CoreLoop.InvokeSafe(delegate
			{
				PrintMessage(new Message(message.ToString(), color, messageType, context));
			});
		}

		private static void PrintMessage(Message message)
		{
			Printer.PrintMessage(message);
		}

		private static void PrintMessageWithStackTrace(Message message)
		{
			Printer.PrintMessage(message);
			switch (message.messageType)
			{
			case Message.MessageType.Log:
				if (message.hasContext)
				{
					Debug.Log(message.message, message.context);
				}
				else
				{
					Debug.Log(message.message);
				}
				break;
			case Message.MessageType.Warning:
				if (message.hasContext)
				{
					Debug.LogWarning(message.message, message.context);
				}
				else
				{
					Debug.LogWarning(message.message);
				}
				break;
			case Message.MessageType.Error:
				if (message.hasContext)
				{
					Debug.LogError(message.message, message.context);
				}
				else
				{
					Debug.LogError(message.message);
				}
				break;
			}
		}
	}
}
