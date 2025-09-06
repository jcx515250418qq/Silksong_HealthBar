using System;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveSlotBackgrounds : MonoBehaviour
{
	[Serializable]
	public class AreaBackground
	{
		[FormerlySerializedAs("backgroundImage")]
		public Sprite BackgroundImage;

		public Sprite Act3BackgroundImage;

		[LocalisedString.NotRequired]
		public LocalisedString NameOverride;

		public bool Act3OverlayOptOut;
	}

	[SerializeField]
	[ArrayForEnum(typeof(MapZone))]
	private AreaBackground[] areaBackgrounds;

	[SerializeField]
	[ArrayForEnum(typeof(ExtraRestZones))]
	private AreaBackground[] extraAreaBackgrounds;

	[SerializeField]
	[ArrayForEnum(typeof(BellhomePaintColours))]
	private Sprite[] bellhomeBackgrounds;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref areaBackgrounds, typeof(MapZone));
		ArrayForEnumAttribute.EnsureArraySize(ref extraAreaBackgrounds, typeof(ExtraRestZones));
		ArrayForEnumAttribute.EnsureArraySize(ref bellhomeBackgrounds, typeof(BellhomePaintColours));
	}

	private void Awake()
	{
		OnValidate();
	}

	public AreaBackground GetBackground(SaveStats currentSaveStats)
	{
		ExtraRestZones extraRestZone = currentSaveStats.ExtraRestZone;
		AreaBackground[] array;
		if (extraRestZone > ExtraRestZones.None)
		{
			array = extraAreaBackgrounds;
			if (array != null && array.Length > 0)
			{
				AreaBackground extraBackground = GetExtraBackground((int)extraRestZone);
				if (extraRestZone == ExtraRestZones.Bellhome)
				{
					extraBackground.BackgroundImage = bellhomeBackgrounds[(int)currentSaveStats.BellhomePaintColour];
				}
				if (extraBackground != null && (bool)extraBackground.BackgroundImage)
				{
					return extraBackground;
				}
			}
		}
		MapZone mapZone = currentSaveStats.MapZone;
		array = areaBackgrounds;
		if (array != null && array.Length > 0)
		{
			AreaBackground background = GetBackground((int)mapZone);
			if (background != null && (bool)background.BackgroundImage)
			{
				return background;
			}
			return GetBackground(13);
		}
		Debug.LogError("No background images have been created in this prefab.");
		return null;
	}

	public AreaBackground GetBackground(MapZone mapZone)
	{
		return GetBackground((int)mapZone);
	}

	private AreaBackground GetBackground(int i)
	{
		if (i < 0 || i >= areaBackgrounds.Length)
		{
			return null;
		}
		return areaBackgrounds[i];
	}

	private AreaBackground GetExtraBackground(int i)
	{
		if (i < 0 || i >= extraAreaBackgrounds.Length)
		{
			return null;
		}
		return extraAreaBackgrounds[i];
	}
}
