using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Hollow Knight")]
	public class DamageEnemyRespond : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmEvent DamagedEvent;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreEnemy;

		private DamageEnemies damager;

		public override void Reset()
		{
			Target = null;
			DamagedEvent = null;
			StoreEnemy = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				damager = safe.GetComponent<DamageEnemies>();
				if ((bool)damager)
				{
					damager.DamagedEnemyGameObject += OnCausedDamage;
					damager.DamagedEnemy += OnCausedDamage;
					return;
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if ((bool)damager)
			{
				damager.DamagedEnemyGameObject -= OnCausedDamage;
				damager.DamagedEnemy -= OnCausedDamage;
			}
		}

		private void OnCausedDamage(GameObject enemy)
		{
			StoreEnemy.Value = enemy;
		}

		private void OnCausedDamage()
		{
			base.Fsm.Event(DamagedEvent);
		}
	}
}
