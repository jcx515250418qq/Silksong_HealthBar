using UnityEngine;

namespace TeamCherry.PS5
{
	public struct Message
	{
		public enum MessageType
		{
			Log = 0,
			Warning = 1,
			Error = 2
		}

		public string message;

		public Color color;

		public MessageType messageType;

		public Object context;

		public bool hasContext;

		public Message(string message, Color color, MessageType messageType = MessageType.Log)
		{
			this.message = message;
			this.color = color;
			this.messageType = messageType;
			context = null;
			hasContext = false;
		}

		public Message(string message, Color color, MessageType messageType, Object context)
		{
			this.message = message;
			this.color = color;
			this.messageType = messageType;
			this.context = context;
			hasContext = true;
		}
	}
}
