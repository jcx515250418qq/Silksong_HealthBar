using System.Collections.Generic;

public class NoClamberRegion : TrackTriggerObjects
{
	protected static HashSet<NoClamberRegion> InsideRegions = new HashSet<NoClamberRegion>();

	protected static List<NoClamberRegion> RefreshRegions = new List<NoClamberRegion>();

	public static bool IsClamberBlocked => InsideRegions.Count > 0;

	protected override bool RequireEnabled => true;

	protected override void OnDisable()
	{
		base.OnDisable();
		InsideRegions.Remove(this);
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		if (isInside)
		{
			InsideRegions.Add(this);
		}
		else
		{
			InsideRegions.Remove(this);
		}
	}

	public static void RefreshInside()
	{
		RefreshRegions.AddRange(InsideRegions);
		foreach (NoClamberRegion refreshRegion in RefreshRegions)
		{
			refreshRegion.Refresh();
		}
		RefreshRegions.Clear();
	}
}
