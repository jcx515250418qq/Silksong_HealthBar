using System;
using System.Text;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (States)")]
public class CollectableItemStates : CollectableItem
{
	[Serializable]
	private struct ItemState
	{
		public LocalisedString DisplayName;

		public LocalisedString Description;

		[LocalisedString.NotRequired]
		public LocalisedString DescriptionExtra;

		public Sprite Icon;

		public PlayerDataTest Test;

		public bool HideAppends;

		public bool DisplayButtonPrompt;

		public InventoryItemButtonPromptData[] ButtonPromptData;
	}

	[Serializable]
	private class AppendDesc
	{
		public LocalisedString Text;

		public string Format;

		public PlayerDataTest Condition;
	}

	[Space]
	[SerializeField]
	private bool overridesCollected;

	[Space]
	[SerializeField]
	private ItemState[] states;

	[Space]
	[SerializeField]
	private AppendDesc[] appends;

	public override bool DisplayAmount
	{
		get
		{
			if (!overridesCollected)
			{
				return CollectedAmount > 1;
			}
			return false;
		}
	}

	public override int CollectedAmount
	{
		get
		{
			if (!overridesCollected)
			{
				return base.CollectedAmount;
			}
			ItemState[] array = states;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Test.IsFulfilled)
				{
					return 1;
				}
			}
			return 0;
		}
	}

	protected override int IsSeenIndex
	{
		get
		{
			int currentStateIndex = GetCurrentStateIndex();
			if (currentStateIndex == 0 && !states[0].Test.IsDefined)
			{
				return -1;
			}
			return currentStateIndex;
		}
	}

	public override string GetDisplayName(ReadSource readSource)
	{
		return GetCurrentState().DisplayName;
	}

	public override string GetDescription(ReadSource readSource)
	{
		ItemState currentState = GetCurrentState();
		StringBuilder tempStringBuilder = Helper.GetTempStringBuilder(currentState.Description);
		if (!currentState.DescriptionExtra.IsEmpty)
		{
			tempStringBuilder.AppendLine();
			tempStringBuilder.AppendLine();
			tempStringBuilder.AppendLine(currentState.DescriptionExtra);
		}
		if (currentState.HideAppends)
		{
			return tempStringBuilder.ToString();
		}
		if (appends.Length == 0)
		{
			return tempStringBuilder.ToString();
		}
		tempStringBuilder.AppendLine();
		AppendDesc[] array = appends;
		foreach (AppendDesc appendDesc in array)
		{
			if (appendDesc.Condition.IsFulfilled)
			{
				string value = (string.IsNullOrEmpty(appendDesc.Format) ? ((string)appendDesc.Text) : string.Format(appendDesc.Format, appendDesc.Text));
				tempStringBuilder.AppendLine();
				tempStringBuilder.Append(value);
			}
		}
		return tempStringBuilder.ToString();
	}

	public override Sprite GetIcon(ReadSource readSource)
	{
		return GetCurrentState().Icon;
	}

	public override InventoryItemButtonPromptData[] GetButtonPromptData()
	{
		ItemState currentState = GetCurrentState();
		if (currentState.DisplayButtonPrompt)
		{
			return currentState.ButtonPromptData;
		}
		return null;
	}

	private ItemState GetCurrentState()
	{
		int currentStateIndex = GetCurrentStateIndex();
		if (currentStateIndex >= 0)
		{
			return states[currentStateIndex];
		}
		return default(ItemState);
	}

	private int GetCurrentStateIndex()
	{
		if (!Application.isPlaying)
		{
			return 0;
		}
		int num = -1;
		for (int i = 0; i < states.Length; i++)
		{
			if (states[i].Test.IsFulfilled)
			{
				num = i;
			}
		}
		if (num < 0)
		{
			Debug.LogError("Item state was less than 0");
		}
		return num;
	}
}
