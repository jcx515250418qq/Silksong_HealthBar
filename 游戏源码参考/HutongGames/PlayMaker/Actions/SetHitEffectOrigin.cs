using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetHitEffectOrigin : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmVector3 effectOrigin;

		public bool everyFrame;

		private GameObject targetObj;

		private EnemyHitEffectsRegular enemyHitEffects;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			effectOrigin = new FsmVector3();
		}

		public override void OnEnter()
		{
			targetObj = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (targetObj != null)
			{
				enemyHitEffects = targetObj.GetComponent<EnemyHitEffectsRegular>();
				if (enemyHitEffects != null)
				{
					SetOrigin();
				}
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			SetOrigin();
		}

		private void SetOrigin()
		{
			if (enemyHitEffects == null)
			{
				Finish();
			}
			enemyHitEffects.SetEffectOrigin(effectOrigin.Value);
		}
	}
}
