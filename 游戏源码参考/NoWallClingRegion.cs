using System.Collections.Generic;

public sealed class NoWallClingRegion : TrackTriggerObjects
{
	protected static HashSet<NoWallClingRegion> InsideRegions = new HashSet<NoWallClingRegion>();

	protected static List<NoWallClingRegion> RefreshRegions = new List<NoWallClingRegion>();

	public static bool IsWallClingBlocked => InsideRegions.Count > 0;

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
		foreach (NoWallClingRegion refreshRegion in RefreshRegions)
		{
			refreshRegion.Refresh();
		}
		RefreshRegions.Clear();
	}
}
