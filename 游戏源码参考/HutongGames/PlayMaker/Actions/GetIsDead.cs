using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetIsDead : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			storeValue = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !storeValue.IsNone)
				{
					storeValue.Value = component.GetIsDead();
				}
			}
			Finish();
		}
	}
}
