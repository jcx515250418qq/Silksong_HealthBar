using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroClampFallVelocity : FsmStateAction
	{
		public FsmOwnerDefault Hero;

		private HeroController hc;

		private Rigidbody2D body;

		public override void Reset()
		{
			Hero = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Hero.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			hc = safe.GetComponent<HeroController>();
			body = hc.Body;
		}

		public override void OnExit()
		{
			hc = null;
			body = null;
		}

		public override void OnUpdate()
		{
			Vector2 linearVelocity = body.linearVelocity;
			float num = 0f - hc.GetMaxFallVelocity();
			if (linearVelocity.y < num)
			{
				body.linearVelocity = new Vector2(linearVelocity.x, num);
			}
		}
	}
}
