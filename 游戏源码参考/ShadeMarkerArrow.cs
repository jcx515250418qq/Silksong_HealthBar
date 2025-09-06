using GlobalEnums;

public class ShadeMarkerArrow : MapMarkerArrow
{
	private bool isActive;

	protected override bool IsActive(bool isQuickMap, MapZone currentMapZone)
	{
		return isActive;
	}

	public void SetActive(bool value)
	{
		isActive = value;
	}
}
