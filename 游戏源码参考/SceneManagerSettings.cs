using System;
using GlobalEnums;
using UnityEngine;

[Serializable]
public class SceneManagerSettings
{
	public enum Conditions
	{
		None = 0,
		BlackThread = 1
	}

	public MapZone mapZone;

	public Conditions condition;

	public Color defaultColor;

	public float defaultIntensity;

	public float saturation;

	public AnimationCurve redChannel;

	public AnimationCurve greenChannel;

	public AnimationCurve blueChannel;

	public Color heroLightColor;

	public float blurPlaneVibranceOffset = 1f;

	public float heroSaturationOffset;

	public SceneManagerSettings(MapZone mapZone, Conditions condition, Color defaultColor, float defaultIntensity, float saturation, AnimationCurve redChannel, AnimationCurve greenChannel, AnimationCurve blueChannel, Color heroLightColor, float blurPlaneVibranceOffset, float heroSaturationOffset)
	{
		this.mapZone = mapZone;
		this.condition = condition;
		this.defaultColor = defaultColor;
		this.defaultIntensity = defaultIntensity;
		this.saturation = saturation;
		this.redChannel = redChannel;
		this.greenChannel = greenChannel;
		this.blueChannel = blueChannel;
		this.heroLightColor = heroLightColor;
		this.blurPlaneVibranceOffset = blurPlaneVibranceOffset;
		this.heroSaturationOffset = heroSaturationOffset;
	}

	public SceneManagerSettings()
	{
	}
}
