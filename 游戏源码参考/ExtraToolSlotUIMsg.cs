using System;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;

public class ExtraToolSlotUIMsg : UIMsgBase<ToolItemType>
{
	[SerializeField]
	private SpriteRenderer defendSlot;

	[SerializeField]
	private LocalisedString defendSlotName;

	[SerializeField]
	private SpriteRenderer exploreSlot;

	[SerializeField]
	private LocalisedString exploreSlotName;

	[Space]
	[SerializeField]
	private TMP_Text nameText;

	[SerializeField]
	private Vector3 spawnPosition;

	public static void Spawn(ToolItemType slotType, GameObject prefab, Action afterMsg = null)
	{
		ExtraToolSlotUIMsg component = prefab.GetComponent<ExtraToolSlotUIMsg>();
		if ((bool)component)
		{
			UIMsgBase<ToolItemType>.Spawn(slotType, component, afterMsg);
		}
	}

	protected override void Setup(ToolItemType slotType)
	{
		base.transform.position = spawnPosition;
		defendSlot.gameObject.SetActive(value: false);
		exploreSlot.gameObject.SetActive(value: false);
		switch (slotType)
		{
		case ToolItemType.Blue:
			defendSlot.gameObject.SetActive(value: true);
			nameText.text = defendSlotName;
			defendSlot.color = UI.GetToolTypeColor(ToolItemType.Blue);
			break;
		case ToolItemType.Yellow:
			exploreSlot.gameObject.SetActive(value: true);
			nameText.text = exploreSlotName;
			exploreSlot.color = UI.GetToolTypeColor(ToolItemType.Yellow);
			break;
		default:
			throw new NotImplementedException();
		}
		nameText.text = defendSlotName;
	}
}
