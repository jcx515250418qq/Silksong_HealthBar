using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Grower)")]
public class CollectableItemGrower : CollectableItem
{
	[Serializable]
	private struct ItemState
	{
		public LocalisedString DisplayName;

		public LocalisedString Description;

		public Sprite Icon;

		[SerializeField]
		public UseResponse[] UseResponses;
	}

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(int), true)]
	private string growStatePdInt;

	[SerializeField]
	private ItemState[] states;

	public override bool DisplayAmount => false;

	public override bool TakeItemOnConsume => false;

	private void OnValidate()
	{
		if (states == null || states.Length == 0)
		{
			states = new ItemState[1];
		}
	}

	private void Awake()
	{
		OnValidate();
	}

	public override string GetDisplayName(ReadSource readSource)
	{
		return GetState(readSource).DisplayName;
	}

	public override string GetDescription(ReadSource readSource)
	{
		return GetState(readSource).Description;
	}

	public override Sprite GetIcon(ReadSource readSource)
	{
		return GetState(readSource).Icon;
	}

	private ItemState GetState(ReadSource readSource)
	{
		if (readSource == ReadSource.GetPopup || readSource == ReadSource.Shop)
		{
			if (states.Length == 0)
			{
				return default(ItemState);
			}
			return states[^1];
		}
		return GetCurrentState();
	}

	private ItemState GetCurrentState()
	{
		if (!Application.isPlaying)
		{
			return states[0];
		}
		int num = PlayerData.instance.GetInt(growStatePdInt);
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= states.Length)
		{
			num = states.Length - 1;
		}
		return states[num];
	}

	protected override IEnumerable<UseResponse> GetUseResponses()
	{
		return base.GetUseResponses().Concat(GetCurrentState().UseResponses);
	}

	public override void ConsumeItemResponse()
	{
		base.ConsumeItemResponse();
		PlayerData.instance.SetInt(growStatePdInt, 0);
	}

	public override bool IsConsumable()
	{
		return true;
	}

	protected override void OnCollected()
	{
		PlayerData.instance.SetInt(growStatePdInt, states.Length - 1);
	}
}
