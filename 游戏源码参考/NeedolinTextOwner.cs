using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Events;

public class NeedolinTextOwner : MonoBehaviour
{
	public enum NeedolinRangeCheckTypes
	{
		Inner = 0,
		Outer = 1,
		Custom = 2,
		Manual = 3,
		Trigger = 4
	}

	[Serializable]
	private struct MapZoneOverride
	{
		public MapZone MapZone;

		public LocalisedTextCollectionField TextCollection;
	}

	[SerializeField]
	private NeedolinRangeCheckTypes rangeCheck;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingCustomRange", true, true, false)]
	private float customRange;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingTrigger", true, true, false)]
	private TrackTriggerObjects insideTrigger;

	[Space]
	[SerializeField]
	private LocalisedTextCollectionField text;

	[Space]
	[SerializeField]
	private List<MapZoneOverride> mapZoneOverrides;

	[Space]
	public UnityEvent OnAddText;

	public UnityEvent OnRemoveText;

	private LocalisedTextCollectionField addedText;

	private bool isPlaying;

	private bool wasInRange;

	public NeedolinRangeCheckTypes RangeCheck => rangeCheck;

	private bool IsUsingCustomRange()
	{
		return rangeCheck == NeedolinRangeCheckTypes.Custom;
	}

	private bool IsUsingTrigger()
	{
		return rangeCheck == NeedolinRangeCheckTypes.Trigger;
	}

	private void OnDrawGizmosSelected()
	{
		if (IsUsingCustomRange())
		{
			Vector3 position = base.transform.position;
			float? z = 0f;
			Gizmos.DrawWireSphere(position.Where(null, null, z), customRange);
		}
	}

	private void OnEnable()
	{
		HeroPerformanceRegion.StartedPerforming += OnStartedNeedolin;
		HeroPerformanceRegion.StoppedPerforming += OnStoppedNeedolin;
		if (HeroPerformanceRegion.IsPerforming)
		{
			OnStartedNeedolin();
		}
	}

	private void OnDisable()
	{
		HeroPerformanceRegion.StartedPerforming -= OnStartedNeedolin;
		HeroPerformanceRegion.StoppedPerforming -= OnStoppedNeedolin;
		OnStoppedNeedolin();
		if (wasInRange)
		{
			RemoveNeedolinText();
			wasInRange = false;
		}
	}

	private void Update()
	{
		bool flag = isPlaying && IsInRange();
		if (flag && !wasInRange)
		{
			AddNeedolinText();
		}
		else if (!flag && wasInRange)
		{
			RemoveNeedolinText();
		}
		wasInRange = flag;
	}

	private void OnStartedNeedolin()
	{
		isPlaying = true;
	}

	private void OnStoppedNeedolin()
	{
		isPlaying = false;
	}

	private bool IsInRange()
	{
		if (InteractManager.BlockingInteractable != null)
		{
			return false;
		}
		switch (rangeCheck)
		{
		case NeedolinRangeCheckTypes.Inner:
			return HeroPerformanceRegion.GetAffectedState(base.transform, ignoreRange: false) == HeroPerformanceRegion.AffectedState.ActiveInner;
		case NeedolinRangeCheckTypes.Outer:
			return HeroPerformanceRegion.GetAffectedState(base.transform, ignoreRange: false) != HeroPerformanceRegion.AffectedState.None;
		case NeedolinRangeCheckTypes.Custom:
			return HeroPerformanceRegion.IsPlayingInRange(base.transform.position, customRange);
		case NeedolinRangeCheckTypes.Manual:
			return false;
		case NeedolinRangeCheckTypes.Trigger:
			if ((bool)insideTrigger)
			{
				return insideTrigger.IsInside;
			}
			return false;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void AddNeedolinText()
	{
		if (addedText != null)
		{
			return;
		}
		MapZone currentMapZoneEnum = GameManager.instance.GetCurrentMapZoneEnum();
		LocalisedTextCollectionField textCollection = text;
		foreach (MapZoneOverride mapZoneOverride in mapZoneOverrides)
		{
			if (mapZoneOverride.MapZone == currentMapZoneEnum)
			{
				textCollection = mapZoneOverride.TextCollection;
			}
		}
		NeedolinMsgBox.AddText(textCollection.GetCollection());
		addedText = textCollection;
		OnAddText.Invoke();
	}

	public void RemoveNeedolinText()
	{
		if (addedText != null)
		{
			NeedolinMsgBox.RemoveText(addedText.GetCollection());
			addedText = null;
			OnRemoveText.Invoke();
		}
	}

	public void SetTextCollection(LocalisedTextCollection textCollection)
	{
		text.SetCollection(textCollection);
	}

	public void SetRangeCheckInner()
	{
		rangeCheck = NeedolinRangeCheckTypes.Inner;
	}
}
