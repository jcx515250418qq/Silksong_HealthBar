using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

namespace TeamCherry
{
	[Serializable]
	public class SceneDefaultSettings : ScriptableObject
	{
		[SerializeField]
		public List<SceneManagerSettings> settingsList;

		public void OnEnable()
		{
			if (settingsList == null)
			{
				settingsList = new List<SceneManagerSettings>();
			}
		}

		public SceneManagerSettings GetMapZoneSettingsRuntime(MapZone mapZone, SceneManagerSettings.Conditions condition)
		{
			SceneManagerSettings result = null;
			foreach (SceneManagerSettings settings in settingsList)
			{
				if (settings.mapZone == mapZone)
				{
					result = settings;
					if (settings.condition >= condition)
					{
						return settings;
					}
				}
			}
			return result;
		}

		public SceneManagerSettings GetMapZoneSettingsEdit(MapZone mapZone, SceneManagerSettings.Conditions condition)
		{
			foreach (SceneManagerSettings settings in settingsList)
			{
				if (settings.mapZone == mapZone && settings.condition == condition)
				{
					return settings;
				}
			}
			return null;
		}

		public void SaveSettings(SceneManagerSettings sms)
		{
			SceneManagerSettings sceneManagerSettings = null;
			foreach (SceneManagerSettings settings in settingsList)
			{
				if (settings.mapZone == sms.mapZone && settings.condition == sms.condition)
				{
					sceneManagerSettings = settings;
				}
			}
			if (sceneManagerSettings != null)
			{
				sceneManagerSettings.defaultColor = new Color(sms.defaultColor.r, sms.defaultColor.g, sms.defaultColor.b, sms.defaultColor.a);
				sceneManagerSettings.defaultIntensity = sms.defaultIntensity;
				sceneManagerSettings.saturation = sms.saturation;
				sceneManagerSettings.redChannel = new AnimationCurve(sms.redChannel.keys.Clone() as Keyframe[]);
				sceneManagerSettings.greenChannel = new AnimationCurve(sms.greenChannel.keys.Clone() as Keyframe[]);
				sceneManagerSettings.blueChannel = new AnimationCurve(sms.blueChannel.keys.Clone() as Keyframe[]);
				sceneManagerSettings.heroLightColor = new Color(sms.heroLightColor.r, sms.heroLightColor.g, sms.heroLightColor.b, sms.heroLightColor.a);
				sceneManagerSettings.blurPlaneVibranceOffset = sms.blurPlaneVibranceOffset;
				sceneManagerSettings.heroSaturationOffset = sms.heroSaturationOffset;
			}
			else
			{
				settingsList.Add(new SceneManagerSettings(sms.mapZone, sms.condition, new Color(sms.defaultColor.r, sms.defaultColor.g, sms.defaultColor.b), sms.defaultIntensity, sms.saturation, new AnimationCurve(sms.redChannel.keys.Clone() as Keyframe[]), new AnimationCurve(sms.greenChannel.keys.Clone() as Keyframe[]), new AnimationCurve(sms.blueChannel.keys.Clone() as Keyframe[]), new Color(sms.heroLightColor.r, sms.heroLightColor.g, sms.heroLightColor.b, sms.heroLightColor.a), sms.blurPlaneVibranceOffset, sms.heroSaturationOffset));
			}
		}
	}
}
