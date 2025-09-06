using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	public class SimpleTiltByVelocityX : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat tiltFactor;

		public FsmBool reverseIfXScaleNegative;

		private Transform go_transform;

		private Rigidbody2D go_rb2d;

		public override void Reset()
		{
			gameObject = null;
			tiltFactor = null;
			reverseIfXScaleNegative = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				go_transform = ownerDefaultTarget.transform;
				go_rb2d = ownerDefaultTarget.GetComponent<Rigidbody2D>();
				DoTilt();
			}
		}

		public override void OnUpdate()
		{
			DoTilt();
		}

		private void DoTilt()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				float num = go_rb2d.linearVelocity.x * tiltFactor.Value;
				if (reverseIfXScaleNegative.Value && go_transform.lossyScale.x < 0f)
				{
					num *= -1f;
				}
				go_transform.localEulerAngles = new Vector3(go_transform.localEulerAngles.x, go_transform.localEulerAngles.y, num);
			}
		}
	}
}
