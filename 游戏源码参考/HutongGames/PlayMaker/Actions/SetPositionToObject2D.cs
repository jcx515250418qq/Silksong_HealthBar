using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Position of a Game Object to another Game Object's position")]
	public class SetPositionToObject2D : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		public FsmGameObject targetObject;

		public FsmFloat xOffset;

		public FsmFloat yOffset;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			xOffset = null;
			yOffset = null;
		}

		public override void OnEnter()
		{
			DoSetPosition();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (everyFrame)
			{
				DoSetPosition();
			}
		}

		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && !targetObject.IsNone && !(targetObject.Value == null))
			{
				Vector3 vector = targetObject.Value.transform.position;
				if (!xOffset.IsNone)
				{
					vector = new Vector3(vector.x + xOffset.Value, vector.y, vector.z);
				}
				if (!yOffset.IsNone)
				{
					vector = new Vector3(vector.x, vector.y + yOffset.Value, vector.z);
				}
				vector = new Vector3(vector.x, vector.y, ownerDefaultTarget.transform.position.z);
				ownerDefaultTarget.transform.position = vector;
			}
		}
	}
}
