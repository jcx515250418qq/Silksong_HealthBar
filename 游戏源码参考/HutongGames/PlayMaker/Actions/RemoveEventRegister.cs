using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class RemoveEventRegister : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString EventName;

		public override void Reset()
		{
			Target = null;
			EventName = null;
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(EventName.Value))
			{
				GameObject safe = Target.GetSafe(this);
				if ((bool)safe)
				{
					EventRegister.RemoveRegister(safe, EventName.Value);
				}
			}
			Finish();
		}
	}
}
