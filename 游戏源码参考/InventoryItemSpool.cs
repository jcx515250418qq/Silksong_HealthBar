using TeamCherry.Localization;
using UnityEngine;

public class InventoryItemSpool : InventoryItemBasic
{
	[SerializeField]
	private LocalisedString heartVerDesc;

	[Space]
	[SerializeField]
	private GameObject wideVer;

	[SerializeField]
	private GameObject slimVer;

	[SerializeField]
	private GameObject heartVer;

	[SerializeField]
	private GameObject[] hearts;

	[SerializeField]
	private Transform conditionalParent;

	public override string Description
	{
		get
		{
			if (PlayerData.instance.silkRegenMax <= 0)
			{
				return base.Description;
			}
			return heartVerDesc;
		}
	}

	protected override bool IsSeen
	{
		get
		{
			if (PlayerData.instance.silkRegenMax <= 0)
			{
				return true;
			}
			return base.IsSeen;
		}
		set
		{
			if (PlayerData.instance.silkRegenMax > 0)
			{
				base.IsSeen = value;
			}
		}
	}

	protected override void UpdateDisplay()
	{
		base.UpdateDisplay();
		PlayerData instance = PlayerData.instance;
		wideVer.SetActive(value: false);
		slimVer.SetActive(value: false);
		heartVer.SetActive(value: false);
		if (instance.silkRegenMax > 0)
		{
			heartVer.SetActive(value: true);
			for (int i = 0; i < hearts.Length; i++)
			{
				hearts[i].SetActive(instance.silkRegenMax > i);
			}
			return;
		}
		bool flag = false;
		foreach (Transform item in conditionalParent)
		{
			InventoryItemConditional component = item.GetComponent<InventoryItemConditional>();
			if ((bool)component && component.Test.IsFulfilled)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			slimVer.SetActive(value: true);
		}
		else
		{
			wideVer.SetActive(value: true);
		}
	}
}
