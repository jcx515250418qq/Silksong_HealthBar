using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable.")]
	public class GetDistanceV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Measure distance from this GameObject.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("Target GameObject.")]
		public FsmGameObject target;

		public FsmFloat targetOffsetX;

		public FsmFloat targetOffsetY;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance in a float variable.")]
		public FsmFloat storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			storeResult = null;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			DoGetDistance();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetDistance();
		}

		private void DoGetDistance()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && !(target.Value == null) && storeResult != null)
			{
				Vector3 b = new Vector3(target.Value.transform.position.x + targetOffsetX.Value, target.Value.transform.position.y + targetOffsetY.Value, target.Value.transform.position.z);
				storeResult.Value = Vector3.Distance(ownerDefaultTarget.transform.position, b);
			}
		}
	}
}
