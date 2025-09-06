public class WindRegion : TrackTriggerObjects
{
	private static int _insideRegions;

	private bool wasInside;

	protected override void OnInsideStateChanged(bool isInside)
	{
		if (wasInside != isInside)
		{
			wasInside = isInside;
			if (isInside)
			{
				_insideRegions++;
			}
			else
			{
				_insideRegions--;
			}
			UpdateWindy();
		}
	}

	public static void AddWind()
	{
		_insideRegions++;
		UpdateWindy();
	}

	public static void RemoveWind()
	{
		_insideRegions--;
		UpdateWindy();
	}

	private static void UpdateWindy()
	{
		if (_insideRegions == 1)
		{
			SetWindy(value: true);
		}
		else if (_insideRegions == 0)
		{
			SetWindy(value: false);
		}
	}

	private static void SetWindy(bool value)
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.cState.inWindRegion = value;
		}
	}
}
