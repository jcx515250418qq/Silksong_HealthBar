using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetHPEveryFrame : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;

		private HealthManager healthManager;

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
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				healthManager = safe.GetComponent<HealthManager>();
			}
		}

		public override void OnUpdate()
		{
			if (healthManager != null && !storeValue.IsNone)
			{
				storeValue.Value = healthManager.hp;
			}
		}
	}
}
