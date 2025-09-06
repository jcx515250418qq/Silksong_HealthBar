using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetDesiredProjectileVelocity : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmVector2 DesiredVelocity;

		private ProjectileVelocityManager manager;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				ProjectileVelocityManager component = safe.GetComponent<ProjectileVelocityManager>();
				if ((bool)component)
				{
					component.DesiredVelocity = DesiredVelocity.Value;
				}
			}
			Finish();
		}
	}
}
