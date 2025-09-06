using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D. If the specified bool is true, ignore.")]
	public class SetVelocity2dIfFalse : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmVector2 vector;

		public FsmFloat x;

		public FsmFloat y;

		public FsmBool checkBool;

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
			checkBool = new FsmBool
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetVelocity()
		{
			if (!(rb2d == null) && !checkBool.Value)
			{
				Vector2 linearVelocity = ((!vector.IsNone) ? vector.Value : rb2d.linearVelocity);
				if (!x.IsNone)
				{
					linearVelocity.x = x.Value;
				}
				if (!y.IsNone)
				{
					linearVelocity.y = y.Value;
				}
				rb2d.linearVelocity = linearVelocity;
				rb2d.angularVelocity = 3f;
			}
		}
	}
}
