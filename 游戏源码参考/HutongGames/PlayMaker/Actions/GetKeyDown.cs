using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when a Key is pressed.")]
	public class GetKeyDown : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The key to detect.")]
		public KeyCode key;

		[Tooltip("The Event to send when the key is pressed.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool Variable. True if pressed, otherwise False.")]
		public FsmBool storeResult;

		public override void Reset()
		{
			sendEvent = null;
			key = KeyCode.None;
			storeResult = null;
		}

		public override void OnUpdate()
		{
			bool keyDown = Input.GetKeyDown(key);
			storeResult.Value = keyDown;
			if (keyDown)
			{
				base.Fsm.Event(sendEvent);
			}
		}
	}
}
