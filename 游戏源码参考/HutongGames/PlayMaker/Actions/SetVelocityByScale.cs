using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	public class SetVelocityByScale : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		public FsmFloat speed;

		public FsmFloat ySpeed;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			speed = null;
			ySpeed = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				Vector2 linearVelocity = base.rigidbody2d.linearVelocity;
				if (ownerDefaultTarget.transform.localScale.x > 0f)
				{
					linearVelocity.x = speed.Value;
				}
				else
				{
					linearVelocity.x = 0f - speed.Value;
				}
				if (!ySpeed.IsNone)
				{
					linearVelocity.y = ySpeed.Value;
				}
				base.rigidbody2d.linearVelocity = linearVelocity;
			}
		}
	}
}
