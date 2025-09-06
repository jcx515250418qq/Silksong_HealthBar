using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	public class GetActiveChild : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			gameObject = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			DoGetChild();
			Finish();
		}

		private void DoGetChild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null || ownerDefaultTarget.transform.childCount == 0)
			{
				return;
			}
			foreach (Transform item in ownerDefaultTarget.transform)
			{
				if (item.gameObject.activeSelf)
				{
					storeResult.Value = item.gameObject;
				}
			}
		}
	}
}
