using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when a Key is released.")]
	public class GetKeyUp : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The key to detect.")]
		public KeyCode key;

		[Tooltip("The Event to send when the key is released.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool Variable. True if released, otherwise False.")]
		public FsmBool storeResult;

		public override void Reset()
		{
			sendEvent = null;
			key = KeyCode.None;
			storeResult = null;
		}

		public override void OnUpdate()
		{
			bool keyUp = Input.GetKeyUp(key);
			storeResult.Value = keyUp;
			if (keyUp)
			{
				base.Fsm.Event(sendEvent);
			}
		}
	}
}
