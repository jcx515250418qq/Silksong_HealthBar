using System.Collections.Generic;
using UnityEngine;

public sealed class AntRegionFlingChildrenNotifier : MonoBehaviour, AntRegion.IPickUpNotify
{
	[SerializeField]
	private FlingChildrenOnStart flingChildrenOnStart;

	private ResetDynamicHierarchy resetDynamicHierarchy;

	private AntRegionFlingChildrenNotifier parentNotifier;

	private List<AntRegionFlingChildrenNotifier> childNotifiers = new List<AntRegionFlingChildrenNotifier>();

	private HashSet<AntRegion> activeRegions = new HashSet<AntRegion>();

	private bool disconnected;

	private void Awake()
	{
		if (!flingChildrenOnStart)
		{
			flingChildrenOnStart = GetComponent<FlingChildrenOnStart>();
			if (!flingChildrenOnStart)
			{
				return;
			}
		}
		foreach (Transform item in base.transform)
		{
			AntRegionFlingChildrenNotifier antRegionFlingChildrenNotifier = item.gameObject.AddComponentIfNotPresent<AntRegionFlingChildrenNotifier>();
			childNotifiers.Add(antRegionFlingChildrenNotifier);
			antRegionFlingChildrenNotifier.parentNotifier = this;
		}
	}

	private void OnValidate()
	{
		if (!flingChildrenOnStart)
		{
			flingChildrenOnStart = GetComponent<FlingChildrenOnStart>();
		}
	}

	private void OnDisable()
	{
		if (activeRegions.Count > 0)
		{
			activeRegions.Clear();
			if ((bool)parentNotifier)
			{
				parentNotifier.ChildExitedAntRegion();
			}
			else if ((bool)resetDynamicHierarchy)
			{
				resetDynamicHierarchy.Reconnect(base.transform, applyRoot: true, recursive: true);
			}
		}
	}

	private void EnteredAntRegion()
	{
		if (disconnected)
		{
			return;
		}
		if (!resetDynamicHierarchy)
		{
			if (!flingChildrenOnStart)
			{
				return;
			}
			Transform parentParent = flingChildrenOnStart.GetParentParent();
			if (!parentParent)
			{
				return;
			}
			resetDynamicHierarchy = parentParent.GetComponent<ResetDynamicHierarchy>();
			if (!resetDynamicHierarchy)
			{
				return;
			}
		}
		if (childNotifiers.Count == 0)
		{
			return;
		}
		bool flag = true;
		foreach (AntRegionFlingChildrenNotifier childNotifier in childNotifiers)
		{
			if (childNotifier.activeRegions.Count > 0)
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			resetDynamicHierarchy.Disconnect(base.transform, recursive: true);
			disconnected = true;
		}
	}

	private void ChildExitedAntRegion()
	{
		if (!disconnected)
		{
			return;
		}
		foreach (AntRegionFlingChildrenNotifier childNotifier in childNotifiers)
		{
			if (childNotifier.activeRegions.Count > 0)
			{
				return;
			}
		}
		if ((bool)resetDynamicHierarchy)
		{
			resetDynamicHierarchy.Reconnect(base.transform, applyRoot: true, recursive: true);
		}
		disconnected = false;
	}

	public void PickUpStarted(AntRegion antRegion)
	{
		if (!(antRegion == null) && activeRegions.Add(antRegion))
		{
			if ((bool)parentNotifier)
			{
				parentNotifier.EnteredAntRegion();
			}
			else
			{
				EnteredAntRegion();
			}
		}
	}

	public void PickUpEnd(AntRegion antRegion)
	{
		if (activeRegions.Remove(antRegion))
		{
			if ((bool)parentNotifier)
			{
				parentNotifier.ChildExitedAntRegion();
			}
			else
			{
				ChildExitedAntRegion();
			}
		}
	}
}
