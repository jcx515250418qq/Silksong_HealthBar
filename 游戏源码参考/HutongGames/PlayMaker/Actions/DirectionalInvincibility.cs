using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class DirectionalInvincibility : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt xPositiveDirection;

		public FsmInt xNegativeDirection;

		private bool shieldingPositive;

		private GameObject targetGo;

		private Transform tform;

		private HealthManager healthManager;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			xPositiveDirection = null;
			xNegativeDirection = null;
		}

		public override void OnEnter()
		{
			targetGo = target.GetSafe(this);
			if (targetGo != null)
			{
				tform = targetGo.transform;
				healthManager = targetGo.GetComponent<HealthManager>();
				if (healthManager != null)
				{
					healthManager.IsInvincible = true;
				}
			}
			if (tform.localScale.x > 0f)
			{
				healthManager.InvincibleFromDirection = xPositiveDirection.Value;
				shieldingPositive = true;
			}
			else
			{
				healthManager.InvincibleFromDirection = xNegativeDirection.Value;
				shieldingPositive = false;
			}
		}

		public override void OnUpdate()
		{
			float x = tform.localScale.x;
			if (x > 0f && !shieldingPositive)
			{
				healthManager.InvincibleFromDirection = xPositiveDirection.Value;
				shieldingPositive = true;
			}
			else if (x < 0f && shieldingPositive)
			{
				healthManager.InvincibleFromDirection = xNegativeDirection.Value;
				shieldingPositive = false;
			}
		}

		public override void OnExit()
		{
			if (healthManager != null)
			{
				healthManager.InvincibleFromDirection = 0;
				healthManager.IsInvincible = false;
			}
		}
	}
}
