using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
	public class ClampVelocity2D : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		public FsmFloat xMin;

		public FsmFloat xMax;

		public FsmFloat yMin;

		public FsmFloat yMax;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			xMin = null;
			xMax = null;
			yMin = null;
			yMax = null;
			everyFrame = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			DoClampVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoClampVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoClampVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				Vector2 linearVelocity = base.rigidbody2d.linearVelocity;
				if (!xMin.IsNone && linearVelocity.x < xMin.Value)
				{
					linearVelocity.x = xMin.Value;
				}
				else if (!xMax.IsNone && linearVelocity.x > xMax.Value)
				{
					linearVelocity.x = xMax.Value;
				}
				if (!yMin.IsNone && linearVelocity.y < yMin.Value)
				{
					linearVelocity.y = yMin.Value;
				}
				else if (!yMax.IsNone && linearVelocity.y > yMax.Value)
				{
					linearVelocity.y = yMax.Value;
				}
				base.rigidbody2d.linearVelocity = linearVelocity;
			}
		}
	}
}
