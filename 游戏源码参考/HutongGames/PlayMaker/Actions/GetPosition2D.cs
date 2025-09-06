using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the 2D Position of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable")]
	public class GetPosition2D : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmVector2 vector;

		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		public Space space;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			x = null;
			y = null;
			space = Space.World;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetPosition();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetPosition();
		}

		private void DoGetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 vector = ((space == Space.World) ? ownerDefaultTarget.transform.position : ownerDefaultTarget.transform.localPosition);
				this.vector.Value = vector;
				x.Value = vector.x;
				y.Value = vector.y;
			}
		}
	}
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the 2D Position of a GameObject and stores it in a Vector2 Variable or each Axis in a Float Variable")]
	public class GetPosition2d : ComponentAction<Transform>
	{
		[RequiredField]
		[Tooltip("The game object to examine.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Title("Vector2")]
		[Tooltip("Store the position in a Vector2 Variable.")]
		public FsmVector2 vector_2d;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X coordinate in a Float Variable.")]
		public FsmFloat x;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y coordinate in a Float Variable.")]
		public FsmFloat y;

		[Tooltip("Use world or local coordinates.")]
		public Space space;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			vector_2d = null;
			x = null;
			y = null;
			space = Space.World;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetPosition();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetPosition();
		}

		private void DoGetPosition()
		{
			if (UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Vector3 vector = ((space == Space.World) ? base.cachedTransform.position : base.cachedTransform.localPosition);
				vector_2d.Value = vector;
				x.Value = vector.x;
				y.Value = vector.y;
			}
		}
	}
}
