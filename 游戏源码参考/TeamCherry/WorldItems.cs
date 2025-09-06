using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamCherry
{
	[Serializable]
	public class WorldItems : ScriptableObject
	{
		public List<GeoRock> geoRocks;

		public void OnEnable()
		{
			if (geoRocks == null)
			{
				geoRocks = new List<GeoRock>();
			}
		}

		public void RegisterGeoRock()
		{
		}
	}
}
