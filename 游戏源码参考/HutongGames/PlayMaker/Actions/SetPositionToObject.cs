using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Position of a Game Object to another Game Object's position")]
	public class SetPositionToObject : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		public FsmGameObject targetObject;

		public FsmFloat xOffset;

		public FsmFloat yOffset;

		public FsmFloat zOffset;

		public FsmFloat overrideZ;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			xOffset = null;
			yOffset = null;
			zOffset = null;
			overrideZ = new FsmFloat
			{
				UseVariable = true
			};
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
				Vector3 position = targetObject.Value.transform.position;
				if (!xOffset.IsNone)
				{
					position = new Vector3(position.x + xOffset.Value, position.y, position.z);
				}
				if (!yOffset.IsNone)
				{
					position = new Vector3(position.x, position.y + yOffset.Value, position.z);
				}
				if (!zOffset.IsNone)
				{
					position = new Vector3(position.x, position.y, position.z + zOffset.Value);
				}
				if (!overrideZ.IsNone && overrideZ != null)
				{
					position = new Vector3(position.x, position.y, overrideZ.Value);
				}
				ownerDefaultTarget.transform.position = position;
			}
		}
	}
}
