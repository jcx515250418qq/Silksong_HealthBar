using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the mid point between two objects")]
	public class GetMidPoint : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmGameObject target;

		[UIHint(UIHint.Variable)]
		public FsmVector3 midPoint;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			midPoint = null;
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
			if (!(ownerDefaultTarget == null) && !(target.Value == null))
			{
				Vector3 position = ownerDefaultTarget.transform.position;
				Vector3 position2 = target.Value.transform.position;
				Vector3 value = new Vector3(position.x + (position2.x - position.x) / 2f, position.y + (position2.y - position.y) / 2f, position.z + (position2.z - position.z) / 2f);
				midPoint.Value = value;
			}
		}
	}
}
