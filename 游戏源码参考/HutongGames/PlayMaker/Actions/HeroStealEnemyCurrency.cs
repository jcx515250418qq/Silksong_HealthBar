using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroStealEnemyCurrency : FsmStateAction
	{
		public FsmOwnerDefault Enemy;

		public override void Reset()
		{
			Enemy = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Enemy.GetSafe(this);
			if ((bool)safe)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if ((bool)component)
				{
					component.DoStealHit();
				}
			}
			Finish();
		}
	}
}
