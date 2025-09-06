using HutongGames.PlayMaker;
using UnityEngine;

public class InventorySubMenuButton : InventoryItemSelectableDirectional
{
	[Space]
	[SerializeField]
	private Color cursorColor;

	[SerializeField]
	private InventoryPane targetPane;

	private InventoryPaneList paneList;

	private PlayMakerFSM invFsm;

	private FsmGameObject invFsmPaneVar;

	public override Color? CursorColor => cursorColor;

	protected override void Awake()
	{
		base.Awake();
		paneList = GetComponentInParent<InventoryPaneList>();
		invFsm = FSMUtility.LocateFSM(paneList.gameObject, "Inventory Control");
		invFsmPaneVar = invFsm.FsmVariables.FindFsmGameObject("Target Pane");
	}

	public override bool Submit()
	{
		invFsmPaneVar.Value = targetPane.gameObject;
		invFsm.SendEvent("FADE TO TARGET");
		return true;
	}
}
