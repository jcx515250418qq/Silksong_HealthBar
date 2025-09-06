using System.Collections.Generic;
using GlobalEnums;

public static class FastTravelScenes
{
	private static readonly Dictionary<FastTravelLocations, string> _scenes = new Dictionary<FastTravelLocations, string>
	{
		{
			FastTravelLocations.Bonetown,
			"Bellway_01"
		},
		{
			FastTravelLocations.Docks,
			"Bellway_02"
		},
		{
			FastTravelLocations.BoneforestEast,
			"Bellway_03"
		},
		{
			FastTravelLocations.Greymoor,
			"Bellway_04"
		},
		{
			FastTravelLocations.Belltown,
			"Belltown_basement"
		},
		{
			FastTravelLocations.CoralTower,
			"Bellway_08"
		},
		{
			FastTravelLocations.City,
			"Bellway_City"
		},
		{
			FastTravelLocations.Peak,
			"Slab_06"
		},
		{
			FastTravelLocations.Shellwood,
			"Shellwood_19"
		},
		{
			FastTravelLocations.Bone,
			"Bone_05"
		},
		{
			FastTravelLocations.Shadow,
			"Bellway_Shadow"
		},
		{
			FastTravelLocations.Aqueduct,
			"Bellway_Aqueduct"
		}
	};

	private static readonly Dictionary<TubeTravelLocations, string> _tubeScenes = new Dictionary<TubeTravelLocations, string>
	{
		{
			TubeTravelLocations.Hub,
			"Tube_Hub"
		},
		{
			TubeTravelLocations.Song,
			"Song_01b"
		},
		{
			TubeTravelLocations.Under,
			"Under_22"
		},
		{
			TubeTravelLocations.CityBellway,
			"Bellway_City"
		},
		{
			TubeTravelLocations.Hang,
			"Hang_06b"
		},
		{
			TubeTravelLocations.Enclave,
			"Song_Enclave_Tube"
		},
		{
			TubeTravelLocations.Arborium,
			"Arborium_Tube"
		}
	};

	public static string GetSceneName(FastTravelLocations location)
	{
		return GetSceneName(location, _scenes);
	}

	public static string GetSceneName(TubeTravelLocations location)
	{
		return GetSceneName(location, _tubeScenes);
	}

	private static string GetSceneName<T>(T location, Dictionary<T, string> dictionary)
	{
		if (!dictionary.TryGetValue(location, out var value))
		{
			return null;
		}
		return value;
	}
}
