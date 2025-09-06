using System;
using TMProOld;
using UnityEngine;

public class ToolCrestUIMsg : UIMsgBase<ToolCrest>
{
	[SerializeField]
	private SpriteRenderer crestDisplay;

	[SerializeField]
	private TMP_Text nameText;

	[SerializeField]
	private TMP_Text descText;

	[SerializeField]
	private TMP_Text itemPrefixText;

	[SerializeField]
	private TMP_Text equipText;

	public static void Spawn(ToolCrest crest, GameObject prefab, Action afterMsg = null)
	{
		ToolCrestUIMsg component = prefab.GetComponent<ToolCrestUIMsg>();
		if ((bool)component)
		{
			UIMsgBase<ToolCrest>.Spawn(crest, component, afterMsg);
		}
	}

	protected override void Setup(ToolCrest crest)
	{
		if ((bool)crestDisplay)
		{
			crestDisplay.sprite = crest.CrestSprite;
		}
		if ((bool)nameText)
		{
			nameText.text = crest.DisplayName;
		}
		if ((bool)descText)
		{
			descText.text = crest.GetPromptDesc;
		}
		if ((bool)itemPrefixText)
		{
			itemPrefixText.text = crest.ItemNamePrefix;
		}
		if ((bool)equipText)
		{
			equipText.text = crest.EquipText;
		}
	}
}
