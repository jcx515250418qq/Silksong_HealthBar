using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetCurrentInventoryPaneCustom : FsmStateAction
	{
		public FsmOwnerDefault PaneList;

		public FsmGameObject NewPane;

		[UIHint(UIHint.Variable)]
		public FsmGameObject CurrentPane;

		[UIHint(UIHint.Variable)]
		public FsmInt CurrentPaneIndex;

		[UIHint(UIHint.Variable)]
		public FsmGameObject PreviousPane;

		public override void Reset()
		{
			PaneList = null;
			NewPane = null;
			PreviousPane = null;
			CurrentPane = null;
			CurrentPaneIndex = null;
		}

		public override void OnEnter()
		{
			GameObject value = NewPane.Value;
			GameObject safe = PaneList.GetSafe(this);
			if ((bool)safe && (bool)value)
			{
				InventoryPane component = value.GetComponent<InventoryPane>();
				InventoryPaneList component2 = safe.GetComponent<InventoryPaneList>();
				if ((bool)component2 && (bool)component)
				{
					PreviousPane.Value = CurrentPane.Value;
					InventoryPane inventoryPane = component2.BeginPane(component, 0);
					CurrentPane.Value = inventoryPane.gameObject;
					CurrentPaneIndex.Value = component2.GetPaneIndex(inventoryPane);
				}
			}
			Finish();
		}
	}
}
