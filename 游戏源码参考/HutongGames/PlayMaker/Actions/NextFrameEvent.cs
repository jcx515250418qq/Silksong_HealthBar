using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event in the next frame. Useful if you want to loop states every frame.")]
	public class NextFrameEvent : FsmStateAction
	{
		[Tooltip("The Event to send.")]
		public FsmEvent sendEvent;

		private int enterFrameCount;

		public override void Reset()
		{
			sendEvent = null;
		}

		public override void OnEnter()
		{
			enterFrameCount = Time.frameCount;
		}

		public override void OnUpdate()
		{
			if (Time.frameCount != enterFrameCount)
			{
				Finish();
				base.Fsm.Event(sendEvent);
			}
		}
	}
}
