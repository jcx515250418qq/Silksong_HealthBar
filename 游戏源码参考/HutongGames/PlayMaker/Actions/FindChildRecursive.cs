using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	public class FindChildRecursive : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmString childName;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			gameObject = null;
			childName = "";
			storeResult = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Transform transform = DoFindChildRecursive(ownerDefaultTarget.transform, childName.Value);
				storeResult.Value = ((transform != null) ? transform.gameObject : null);
				Finish();
			}
		}

		private Transform DoFindChildRecursive(Transform parent, string childName)
		{
			_ = parent.childCount;
			foreach (Transform item in parent)
			{
				if (item.gameObject.name == childName)
				{
					return item;
				}
			}
			foreach (Transform item2 in parent)
			{
				Transform transform2 = DoFindChildRecursive(item2, childName);
				if ((bool)transform2)
				{
					return transform2;
				}
			}
			return null;
		}
	}
}
