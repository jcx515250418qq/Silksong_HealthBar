using System;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Stack)")]
public class CollectableItemStack : CollectableItem
{
	[Serializable]
	private struct StackVariation
	{
		public Sprite Icon;
	}

	[Space]
	[SerializeField]
	private LocalisedString singleDisplayName;

	[SerializeField]
	private LocalisedString singleDescription;

	[Space]
	[SerializeField]
	private LocalisedString pluralDisplayName;

	[SerializeField]
	private LocalisedString pluralDescription;

	[Space]
	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString allDisplayName;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString allDescription;

	[Space]
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private Sprite singleIcon;

	[SerializeField]
	private Sprite[] singleIcons;

	[SerializeField]
	private StackVariation[] stackVariations;

	[SerializeField]
	private bool isAlwaysUnlocked;

	[SerializeField]
	private bool displayAmount;

	public override int CollectedAmount => base.CollectedAmount + (isAlwaysUnlocked ? 1 : 0);

	public override bool DisplayAmount
	{
		get
		{
			if (displayAmount)
			{
				return CollectedAmount > 1;
			}
			return false;
		}
	}

	protected virtual void OnValidate()
	{
		if (stackVariations == null || stackVariations.Length == 0)
		{
			stackVariations = new StackVariation[1];
		}
		if ((bool)singleIcon)
		{
			singleIcons = new Sprite[1] { singleIcon };
			singleIcon = null;
		}
	}

	private (LocalisedString, LocalisedString) GetNameDescPair(ReadSource readSource)
	{
		bool flag = CollectedAmount >= singleIcons.Length;
		if (flag && readSource == ReadSource.TakePopup)
		{
			GetAllPair();
		}
		if (readSource != 0)
		{
			return (singleDisplayName, singleDescription);
		}
		if (flag)
		{
			return GetAllPair();
		}
		if (CollectedAmount <= 1)
		{
			return (singleDisplayName, singleDescription);
		}
		return (pluralDisplayName, pluralDescription);
		(LocalisedString, LocalisedString) GetAllPair()
		{
			return (allDisplayName.IsEmpty ? pluralDisplayName : allDisplayName, allDescription.IsEmpty ? pluralDescription : allDescription);
		}
	}

	public override string GetDisplayName(ReadSource readSource)
	{
		return GetNameDescPair(readSource).Item1;
	}

	public override string GetDescription(ReadSource readSource)
	{
		return GetNameDescPair(readSource).Item2;
	}

	public override Sprite GetIcon(ReadSource readSource)
	{
		if (readSource == ReadSource.Inventory || readSource == ReadSource.Shop || (readSource == ReadSource.TakePopup && CollectedAmount >= singleIcons.Length))
		{
			return GetCurrentStackVariation().Icon;
		}
		int num = CollectedAmount - 1;
		if (num >= singleIcons.Length)
		{
			num = singleIcons.Length - 1;
		}
		else if (num < 0)
		{
			num = 0;
		}
		return singleIcons[num];
	}

	private StackVariation GetCurrentStackVariation()
	{
		int num = CollectedAmount - 1;
		if (num >= stackVariations.Length)
		{
			num = stackVariations.Length - 1;
		}
		else if (num < 0)
		{
			num = 0;
		}
		return stackVariations[num];
	}
}
