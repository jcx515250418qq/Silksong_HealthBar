using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetNextInventoryPane : FsmStateAction
	{
		public FsmOwnerDefault PaneList;

		public FsmInt Direction;

		[UIHint(UIHint.Variable)]
		public FsmGameObject CurrentPane;

		[UIHint(UIHint.Variable)]
		public FsmInt CurrentPaneIndex;

		[UIHint(UIHint.Variable)]
		public FsmGameObject PreviousPane;

		public override void Reset()
		{
			PaneList = null;
			Direction = new FsmInt(1);
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
				if ((bool)component && CurrentPane.Value != null)
				{
					InventoryPane component2 = CurrentPane.Value.GetComponent<InventoryPane>();
					if ((bool)component2)
					{
						PreviousPane.Value = CurrentPane.Value;
						component2 = component.SetNextPane(Direction.Value, component2);
						CurrentPane.Value = component2.gameObject;
						CurrentPaneIndex.Value = component.GetPaneIndex(component2);
					}
				}
			}
			Finish();
		}
	}
}
