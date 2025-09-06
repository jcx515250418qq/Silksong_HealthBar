using HutongGames.PlayMaker;
using TeamCherry.Localization;
using UnityEngine;

public class SetInventoryItemBasicDisplayProperties : FsmStateAction
{
	public FsmOwnerDefault Target;

	public FsmString DisplayNameSheet;

	public FsmString DisplayNameKey;

	public FsmString DescriptionSheet;

	public FsmString DescriptionKey;

	public override void Reset()
	{
		Target = null;
		DisplayNameKey = null;
		DisplayNameSheet = null;
		DescriptionSheet = null;
		DescriptionKey = null;
	}

	public override void OnEnter()
	{
		GameObject safe = Target.GetSafe(this);
		if ((bool)safe)
		{
			InventoryItemBasic component = safe.GetComponent<InventoryItemBasic>();
			if ((bool)component)
			{
				component.SetDisplayProperties(new LocalisedString
				{
					Sheet = DisplayNameSheet.Value,
					Key = DisplayNameKey.Value
				}, new LocalisedString
				{
					Sheet = DescriptionSheet.Value,
					Key = DescriptionKey.Value
				});
			}
		}
		Finish();
	}
}
