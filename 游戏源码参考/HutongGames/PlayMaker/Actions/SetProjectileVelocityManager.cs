using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetProjectileVelocityManager : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool SetEnabled;

		public override void Reset()
		{
			Target = null;
			SetEnabled = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				ProjectileVelocityManager component = safe.GetComponent<ProjectileVelocityManager>();
				if ((bool)component)
				{
					component.enabled = SetEnabled.Value;
				}
			}
			Finish();
		}
	}
}
