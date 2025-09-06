using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SendEventToRegisterDelay : FsmStateAction
	{
		public FsmString EventName;

		public FsmFloat delay;

		public FsmOwnerDefault ExcludeTarget;

		private int eventNameHash;

		private float timer;

		public override void Reset()
		{
			EventName = new FsmString();
			ExcludeTarget = new FsmOwnerDefault();
		}

		public override void Awake()
		{
			eventNameHash = ((!EventName.UsesVariable) ? EventRegister.GetEventHashCode(EventName.Value) : 0);
		}

		public override void OnEnter()
		{
			timer = delay.Value;
		}

		public override void OnUpdate()
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
				return;
			}
			SendToRegister();
			Finish();
		}

		private void SendToRegister()
		{
			if (eventNameHash != 0)
			{
				EventRegister.SendEvent(eventNameHash, ExcludeTarget.GetSafe(this));
			}
			else
			{
				EventRegister.SendEvent(EventName.Value, ExcludeTarget.GetSafe(this));
			}
		}
	}
}
