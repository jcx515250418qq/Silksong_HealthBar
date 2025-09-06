using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Scale of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetScale : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the scale in a Vector3 variable.")]
		public FsmVector3 vector;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X scale in a Float variable.")]
		public FsmFloat xScale;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y scale in a Float variable.")]
		public FsmFloat yScale;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Z scale in a Float variable.")]
		public FsmFloat zScale;

		[Tooltip("The coordinate space to get the rotation in.")]
		public Space space;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			xScale = null;
			yScale = null;
			zScale = null;
			space = Space.World;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetScale();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetScale();
		}

		private void DoGetScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 value = ((space == Space.World) ? ownerDefaultTarget.transform.lossyScale : ownerDefaultTarget.transform.localScale);
				vector.Value = value;
				xScale.Value = value.x;
				yScale.Value = value.y;
				zScale.Value = value.z;
			}
		}
	}
}
