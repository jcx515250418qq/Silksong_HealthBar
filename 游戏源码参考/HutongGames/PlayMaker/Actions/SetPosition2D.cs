using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class SetPosition2D : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault GameObject;

		public FsmVector2 Vector;

		[HideIf("UsingVector")]
		public FsmFloat X;

		[HideIf("UsingVector")]
		public FsmFloat Y;

		public Space Space;

		public bool EveryFrame;

		public bool UsingVector()
		{
			return !Vector.IsNone;
		}

		public override void Reset()
		{
			GameObject = null;
			Vector = new FsmVector2
			{
				UseVariable = true
			};
			X = null;
			Y = null;
			Space = Space.World;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoSetPosition();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetPosition();
		}

		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(GameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector2 vector = ((Space == Space.World) ? ownerDefaultTarget.transform.position : ownerDefaultTarget.transform.localPosition);
				Vector2 position = (UsingVector() ? Vector.Value : new Vector2(X.IsNone ? vector.x : X.Value, Y.IsNone ? vector.y : Y.Value));
				if (Space == Space.World)
				{
					ownerDefaultTarget.transform.SetPosition2D(position);
				}
				else
				{
					ownerDefaultTarget.transform.SetLocalPosition2D(position);
				}
			}
		}
	}
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the 2d Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetPosition2d : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored Vector2 position, and/or set individual axis below.")]
		public FsmVector2 vector;

		[Tooltip("Set the X position.")]
		public FsmFloat x;

		[Tooltip("Set the Y position.")]
		public FsmFloat y;

		[Tooltip("Use local or world space.")]
		public Space space;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

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
			space = Space.Self;
			everyFrame = false;
			lateUpdate = false;
		}

		public override void OnPreprocess()
		{
			if (lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		public override void OnEnter()
		{
			if (!everyFrame && !lateUpdate)
			{
				DoSetPosition();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoSetPosition();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoSetPosition();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector2 vector = ((!this.vector.IsNone) ? this.vector.Value : ((Vector2)((space == Space.World) ? ownerDefaultTarget.transform.position : ownerDefaultTarget.transform.localPosition)));
				if (!x.IsNone)
				{
					vector.x = x.Value;
				}
				if (!y.IsNone)
				{
					vector.y = y.Value;
				}
				if (space == Space.World)
				{
					ownerDefaultTarget.transform.position = new Vector3(vector.x, vector.y, ownerDefaultTarget.transform.position.z);
				}
				else
				{
					ownerDefaultTarget.transform.localPosition = new Vector3(vector.x, vector.y, ownerDefaultTarget.transform.localPosition.z);
				}
			}
		}
	}
}
