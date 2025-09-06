using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class AddEventRegister : FsmStateAction
	{
		public FsmOwnerDefault target;

		public FsmString eventName;

		public override void Reset()
		{
			eventName = new FsmString();
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(eventName.Value))
			{
				GameObject safe = target.GetSafe(this);
				if ((bool)safe)
				{
					EventRegister.GetRegisterGuaranteed(safe, eventName.Value);
				}
			}
			Finish();
		}
	}
}
