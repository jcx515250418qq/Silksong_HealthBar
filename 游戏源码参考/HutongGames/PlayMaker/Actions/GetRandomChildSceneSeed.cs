using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Random Child of a Game Object.")]
	public class GetRandomChildSceneSeed : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Parent;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreResult;

		public override void Reset()
		{
			Parent = null;
			StoreResult = null;
		}

		public override void OnEnter()
		{
			DoGetRandomChild();
			Finish();
		}

		private void DoGetRandomChild()
		{
			GameObject safe = Parent.GetSafe(this);
			if (!(safe == null))
			{
				int childCount = safe.transform.childCount;
				if (childCount != 0)
				{
					int index = GameManager.instance.SceneSeededRandom.Next(childCount);
					StoreResult.Value = safe.transform.GetChild(index).gameObject;
				}
			}
		}
	}
}
