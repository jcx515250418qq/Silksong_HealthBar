using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SendEventToGameObjectOptimized : FsmStateAction
	{
		public FsmOwnerDefault target;

		[RequiredField]
		public FsmString sendEvent;

		public bool everyFrame;

		public override void Reset()
		{
			target = null;
			sendEvent = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				FSMUtility.SendEventToGameObject(safe, sendEvent.Value);
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				FSMUtility.SendEventToGameObject(safe, sendEvent.Value);
			}
		}
	}
}
