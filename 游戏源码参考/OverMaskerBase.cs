using System;
using UnityEngine;

public abstract class OverMaskerBase : MonoBehaviour
{
	[Serializable]
	protected enum OverMaskType
	{
		Blackout = 0,
		UnderBlackout = 1
	}

	protected sealed class OverMaskValue
	{
		public readonly int layerID;

		public readonly short order;

		public OverMaskValue(int layerID, short order)
		{
			this.layerID = layerID;
			this.order = order;
		}
	}

	[SerializeField]
	private OverMaskType overMaskType;

	private static bool cached;

	private static OverMaskValue blackoutMaskSetting;

	private static OverMaskValue underBlackoutMaskSetting;

	private void Awake()
	{
		if (!cached)
		{
			blackoutMaskSetting = new OverMaskValue(SortingLayer.NameToID("Over"), 1);
			underBlackoutMaskSetting = new OverMaskValue(SortingLayer.NameToID("Over"), -1);
			cached = true;
		}
		switch (overMaskType)
		{
		case OverMaskType.Blackout:
			ApplySettings(blackoutMaskSetting);
			break;
		case OverMaskType.UnderBlackout:
			ApplySettings(underBlackoutMaskSetting);
			break;
		default:
			ApplySettings(blackoutMaskSetting);
			break;
		}
	}

	protected abstract void ApplySettings(int sortingLayer, short order);

	protected void ApplySettings(OverMaskValue overMaskValue)
	{
		ApplySettings(overMaskValue.layerID, overMaskValue.order);
	}
}
