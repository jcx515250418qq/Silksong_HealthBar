using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetEnemyDeathEffectsProfile : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[ObjectType(typeof(EnemyDeathEffectsProfile))]
		public FsmObject Profile;

		public override void Reset()
		{
			Target = null;
			Profile = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				EnemyDeathEffectsRegular component = safe.GetComponent<EnemyDeathEffectsRegular>();
				if ((bool)component)
				{
					component.Profile = Profile.Value as EnemyDeathEffectsProfile;
				}
			}
			Finish();
		}
	}
}
