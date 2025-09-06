using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetCurrentInventoryPane : FsmStateAction
	{
		public FsmOwnerDefault PaneList;

		public FsmInt PaneIndex;

		[UIHint(UIHint.Variable)]
		public FsmGameObject CurrentPane;

		[UIHint(UIHint.Variable)]
		public FsmInt CurrentPaneIndex;

		[UIHint(UIHint.Variable)]
		public FsmGameObject PreviousPane;

		public override void Reset()
		{
			PaneList = null;
			PaneIndex = null;
			PreviousPane = null;
			CurrentPane = null;
			CurrentPaneIndex = null;
		}

		public override void OnEnter()
		{
			GameObject safe = PaneList.GetSafe(this);
			if ((bool)safe)
			{
				InventoryPaneList component = safe.GetComponent<InventoryPaneList>();
				if ((bool)component)
				{
					int num = PaneIndex.Value;
					if (num < 0)
					{
						string nextPaneOpen = InventoryPaneList.NextPaneOpen;
						num = component.GetPaneIndex(nextPaneOpen);
						if (num < 0)
						{
							num = CurrentPaneIndex.Value;
						}
					}
					InventoryPane currentPane = (CurrentPane.Value ? CurrentPane.Value.GetComponent<InventoryPane>() : null);
					PreviousPane.Value = CurrentPane.Value;
					InventoryPane inventoryPane = component.SetCurrentPane(num, currentPane);
					CurrentPane.Value = inventoryPane.gameObject;
					CurrentPaneIndex.Value = component.GetPaneIndex(inventoryPane);
				}
			}
			Finish();
		}
	}
}
