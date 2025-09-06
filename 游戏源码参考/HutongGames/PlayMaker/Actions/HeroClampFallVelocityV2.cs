using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroClampFallVelocityV2 : FsmStateAction
	{
		public FsmBool everyFrame;

		private HeroController hc;

		private Rigidbody2D body;

		public override void Reset()
		{
			everyFrame = null;
		}

		public override void OnEnter()
		{
			hc = HeroController.instance;
			if (!hc)
			{
				Finish();
				return;
			}
			hc = hc.GetComponent<HeroController>();
			body = hc.Body;
			OnUpdate();
			if (!everyFrame.Value)
			{
				Finish();
			}
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
