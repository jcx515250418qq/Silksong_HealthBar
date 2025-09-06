using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	public class ClampSpeed : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		public FsmFloat speedMin;

		public FsmFloat speedMax;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			speedMin = new FsmFloat
			{
				UseVariable = true
			};
			speedMax = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoClampSpeed();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoClampSpeed();
		}

		private void DoClampSpeed()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				Vector2 linearVelocity = base.rigidbody2d.linearVelocity;
				if (!speedMin.IsNone && linearVelocity.magnitude < speedMin.Value)
				{
					linearVelocity = linearVelocity.normalized * speedMin.Value;
				}
				if (!speedMax.IsNone && linearVelocity.magnitude > speedMax.Value)
				{
					linearVelocity = linearVelocity.normalized * speedMax.Value;
				}
				base.rigidbody2d.linearVelocity = linearVelocity;
			}
		}
	}
}
