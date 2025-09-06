using System.Collections.Generic;
using GlobalEnums;

public static class CaravanLocationScenes
{
	private static readonly Dictionary<CaravanTroupeLocations, string> _scenes = new Dictionary<CaravanTroupeLocations, string>
	{
		{
			CaravanTroupeLocations.Bone,
			"Bone_10"
		},
		{
			CaravanTroupeLocations.Greymoor,
			"Greymoor_08"
		},
		{
			CaravanTroupeLocations.CoralJudge,
			"Coral_Judge_Arena"
		},
		{
			CaravanTroupeLocations.Aqueduct,
			"Aqueduct_05"
		}
	};

	public static string GetSceneName(CaravanTroupeLocations location)
	{
		if (_scenes.ContainsKey(location))
		{
			return _scenes[location];
		}
		return null;
	}
}
