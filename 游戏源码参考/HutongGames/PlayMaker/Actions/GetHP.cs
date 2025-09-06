using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetHP : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			storeValue = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			storeValue.Value = 0;
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					storeValue.Value = component.hp;
				}
			}
			Finish();
		}
	}
}
