using TeamCherry.Localization;
using UnityEngine;

public class InspectDoor : InteractableBase
{
	[Space]
	[SerializeField]
	private LocalisedString promptText;

	[Space]
	[SerializeField]
	private InteractableBase door;

	[SerializeField]
	private PersistentBoolItem promptPersistent;

	private bool hasEnteredBefore;

	protected override void Awake()
	{
		base.Awake();
		door.Deactivate(allowQueueing: false);
		if ((bool)promptPersistent)
		{
			promptPersistent.OnGetSaveState += delegate(out bool value)
			{
				value = hasEnteredBefore;
			};
			promptPersistent.OnSetSaveState += delegate(bool value)
			{
				hasEnteredBefore = value;
			};
		}
	}

	public override void Interact()
	{
		DisableInteraction();
		if (hasEnteredBefore && (bool)promptPersistent)
		{
			door.Interact();
			return;
		}
		DialogueYesNoBox.Open(delegate
		{
			hasEnteredBefore = true;
			door.Interact();
		}, base.EnableInteraction, returnHud: true, promptText);
	}
}
