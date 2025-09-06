using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Translates a Game Object's RigidBody2d. Unlike Translate2d this will respect physics collisions.")]
	public class TranslatePosition2d : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector.")]
		public FsmVector2 vector;

		[Tooltip("Movement in x axis.")]
		public FsmFloat x;

		[Tooltip("Movement in y axis.")]
		public FsmFloat y;

		[Tooltip("Coordinate space to move in.")]
		public Space space;

		[Tooltip("Translate over one second")]
		public bool perSecond;

		[Tooltip("Keep running every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.World;
			perSecond = true;
			everyFrame = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			DoTranslatePosition2d();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoTranslatePosition2d()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				Vector2 vector = (this.vector.IsNone ? new Vector2(x.Value, y.Value) : this.vector.Value);
				if (!x.IsNone)
				{
					vector.x = x.Value;
				}
				if (!y.IsNone)
				{
					vector.y = y.Value;
				}
				if (perSecond)
				{
					vector *= Time.deltaTime;
				}
				if (space == Space.Self)
				{
					vector = base.cachedTransform.TransformVector(new Vector3(vector.x, vector.y, 0f));
				}
				base.rigidbody2d.MovePosition(base.rigidbody2d.position + vector);
			}
		}
	}
}
