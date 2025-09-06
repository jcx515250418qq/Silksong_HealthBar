using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
	public class SendMessageV2 : FsmStateAction
	{
		public enum MessageType
		{
			SendMessage = 0,
			SendMessageUpwards = 1,
			BroadcastMessage = 2
		}

		[RequiredField]
		[Tooltip("GameObject that sends the message.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Where to send the message.\nSee Unity docs.")]
		public MessageType delivery;

		[Tooltip("Send options.\nSee Unity docs.")]
		public SendMessageOptions options;

		[RequiredField]
		public FunctionCall functionCall;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			delivery = MessageType.SendMessage;
			options = SendMessageOptions.DontRequireReceiver;
			functionCall = null;
		}

		public override void OnEnter()
		{
			DoSendMessage();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSendMessage();
		}

		private void DoSendMessage()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				object obj = null;
				switch (functionCall.ParameterType)
				{
				case "bool":
					obj = functionCall.BoolParameter.Value;
					break;
				case "int":
					obj = functionCall.IntParameter.Value;
					break;
				case "float":
					obj = functionCall.FloatParameter.Value;
					break;
				case "string":
					obj = functionCall.StringParameter.Value;
					break;
				case "Vector2":
					obj = functionCall.Vector2Parameter.Value;
					break;
				case "Vector3":
					obj = functionCall.Vector3Parameter.Value;
					break;
				case "Rect":
					obj = functionCall.RectParamater.Value;
					break;
				case "GameObject":
					obj = functionCall.GameObjectParameter.Value;
					break;
				case "Material":
					obj = functionCall.MaterialParameter.Value;
					break;
				case "Texture":
					obj = functionCall.TextureParameter.Value;
					break;
				case "Color":
					obj = functionCall.ColorParameter.Value;
					break;
				case "Quaternion":
					obj = functionCall.QuaternionParameter.Value;
					break;
				case "Object":
					obj = functionCall.ObjectParameter.Value;
					break;
				}
				switch (delivery)
				{
				case MessageType.SendMessage:
					ownerDefaultTarget.SendMessage(functionCall.FunctionName, obj, options);
					break;
				case MessageType.SendMessageUpwards:
					ownerDefaultTarget.SendMessageUpwards(functionCall.FunctionName, obj, options);
					break;
				case MessageType.BroadcastMessage:
					ownerDefaultTarget.BroadcastMessage(functionCall.FunctionName, obj, options);
					break;
				}
			}
		}
	}
}
