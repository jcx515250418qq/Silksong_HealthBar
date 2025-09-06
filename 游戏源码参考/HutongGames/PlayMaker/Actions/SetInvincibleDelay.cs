using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetInvincibleDelay : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmBool Invincible;

		public FsmInt InvincibleFromDirection;

		public FsmFloat Delay;

		public bool resetOnStateExit;

		private float timer;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			Invincible = null;
			Delay = null;
			InvincibleFromDirection = null;
			resetOnStateExit = false;
		}

		public override void OnEnter()
		{
			timer = 0f;
		}

		public override void OnUpdate()
		{
			if (timer < Delay.Value)
			{
				timer += Time.deltaTime;
				return;
			}
			DoSetInvincible();
			Finish();
		}

		private void DoSetInvincible()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					if (!Invincible.IsNone)
					{
						component.IsInvincible = Invincible.Value;
					}
					if (!InvincibleFromDirection.IsNone)
					{
						component.InvincibleFromDirection = InvincibleFromDirection.Value;
					}
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (!resetOnStateExit)
			{
				return;
			}
			HealthManager component = target.GetSafe(this).GetComponent<HealthManager>();
			if (component != null)
			{
				if (!Invincible.IsNone)
				{
					component.IsInvincible = !Invincible.Value;
				}
				if (!InvincibleFromDirection.IsNone)
				{
					component.InvincibleFromDirection = 0;
				}
			}
		}
	}
}
