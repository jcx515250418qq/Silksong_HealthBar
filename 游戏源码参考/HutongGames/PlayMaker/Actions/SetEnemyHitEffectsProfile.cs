using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetEnemyHitEffectsProfile : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[ObjectType(typeof(EnemyHitEffectsProfile))]
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
				EnemyHitEffectsRegular component = safe.GetComponent<EnemyHitEffectsRegular>();
				if ((bool)component)
				{
					component.Profile = Profile.Value as EnemyHitEffectsProfile;
				}
			}
			Finish();
		}
	}
}
