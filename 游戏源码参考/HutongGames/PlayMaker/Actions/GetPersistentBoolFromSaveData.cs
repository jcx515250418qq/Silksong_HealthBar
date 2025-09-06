using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetPersistentBoolFromSaveData : FsmStateAction
	{
		[CheckForComponent(typeof(PersistentBoolItem))]
		public FsmOwnerDefault Target;

		[HideIf("ShouldHideDirect")]
		public FsmString SceneName;

		[HideIf("ShouldHideDirect")]
		public FsmString ID;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public bool ShouldHideDirect()
		{
			if (Target.OwnerOption != 0)
			{
				if (Target.GameObject != null)
				{
					return Target.GameObject.Value != null;
				}
				return false;
			}
			return true;
		}

		public override void Reset()
		{
			Target = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.SpecifyGameObject,
				GameObject = null
			};
			SceneName = null;
			ID = null;
			StoreValue = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			string sceneName;
			string id;
			if (safe != null)
			{
				PersistentItemData<bool> itemData = safe.GetComponent<PersistentBoolItem>().ItemData;
				sceneName = itemData.SceneName;
				id = itemData.ID;
			}
			else
			{
				sceneName = SceneName.Value;
				id = ID.Value;
			}
			if (SceneData.instance.PersistentBools.TryGetValue(sceneName, id, out var value))
			{
				StoreValue.Value = value.Value;
			}
			else
			{
				StoreValue.Value = false;
			}
			Finish();
		}
	}
}
