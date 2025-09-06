using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForInventoryShortcut : FsmStateAction
	{
		public FsmEvent WasPressed;

		[ObjectType(typeof(InventoryShortcutButtons))]
		public FsmEnum StoreShortcut;

		public FsmInt CurrentPaneIndex;

		private GameManager gm;

		private InputHandler inputHandler;

		public override void Reset()
		{
			WasPressed = null;
			StoreShortcut = null;
			CurrentPaneIndex = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
		}

		public override void OnUpdate()
		{
			if (gm.isPaused)
			{
				return;
			}
			HeroActions inputActions = inputHandler.inputActions;
			InventoryPaneList.PaneTypes paneTypes = InventoryPaneInput.GetInventoryInputPressed(inputActions);
			switch (paneTypes)
			{
			case InventoryPaneList.PaneTypes.None:
				return;
			default:
				if (!CurrentPaneIndex.IsNone && paneTypes != (InventoryPaneList.PaneTypes)CurrentPaneIndex.Value)
				{
					return;
				}
				break;
			case InventoryPaneList.PaneTypes.Inv:
				break;
			}
			if (inputActions.Pause.WasPressed && PlayerData.instance.isInventoryOpen)
			{
				paneTypes = InventoryPaneList.PaneTypes.Inv;
			}
			FsmEnum storeShortcut = StoreShortcut;
			storeShortcut.Value = paneTypes switch
			{
				InventoryPaneList.PaneTypes.Inv => InventoryShortcutButtons.Inventory, 
				InventoryPaneList.PaneTypes.Tools => InventoryShortcutButtons.Tools, 
				InventoryPaneList.PaneTypes.Quests => InventoryShortcutButtons.Quests, 
				InventoryPaneList.PaneTypes.Journal => InventoryShortcutButtons.Journal, 
				InventoryPaneList.PaneTypes.Map => InventoryShortcutButtons.Map, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			base.Fsm.Event(WasPressed);
		}
	}
}
