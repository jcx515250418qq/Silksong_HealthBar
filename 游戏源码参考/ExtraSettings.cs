using System;
using UnityEngine;

public sealed class ExtraSettings : MonoBehaviour
{
	[Flags]
	private enum ExtraSettingFlags
	{
		None = 0,
		RelinquishControl = 1,
		StopPlayingAudio = 2,
		StopForceWalkingSound = 4,
		SuppressNextLevelReadyRegainControl = 8,
		BlockFootstepAudio = 0x10,
		HeroEnterWithoutInput = 0x20,
		HeroSkipNormalEntry = 0x40
	}

	[SerializeField]
	private GameObject target;

	[SerializeField]
	private ExtraSettingFlags settings;

	private bool appliedSettings;

	private void Awake()
	{
		if (target == null)
		{
			target = base.gameObject;
		}
		IApplyExtraLoadSettings[] components = target.GetComponents<IApplyExtraLoadSettings>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ApplyExtraLoadSettings += ApplyExtraLoadSettings;
		}
	}

	private void OnValidate()
	{
		if (target == null)
		{
			target = base.gameObject;
		}
	}

	private void ApplyExtraLoadSettings()
	{
		if (!appliedSettings)
		{
			ApplySettings(settings);
			appliedSettings = true;
		}
	}

	private void ApplySettings(ExtraSettingFlags flags)
	{
		HeroController instance = HeroController.instance;
		if ((flags & ExtraSettingFlags.SuppressNextLevelReadyRegainControl) == ExtraSettingFlags.SuppressNextLevelReadyRegainControl)
		{
			GameManager.SuppressRegainControl = true;
		}
		if (instance != null)
		{
			if ((flags & ExtraSettingFlags.RelinquishControl) == ExtraSettingFlags.RelinquishControl)
			{
				instance.RelinquishControl();
			}
			if ((flags & ExtraSettingFlags.StopPlayingAudio) == ExtraSettingFlags.StopPlayingAudio)
			{
				instance.StopPlayingAudio();
			}
			if ((flags & ExtraSettingFlags.StopForceWalkingSound) == ExtraSettingFlags.StopForceWalkingSound)
			{
				instance.ForceRunningSound = false;
				instance.ForceWalkingSound = false;
			}
			if (flags.HasFlag(ExtraSettingFlags.BlockFootstepAudio))
			{
				instance.SetBlockFootstepAudio(blockFootStep: true);
			}
			if (flags.HasFlag(ExtraSettingFlags.HeroEnterWithoutInput))
			{
				instance.enterWithoutInput = true;
			}
			if (flags.HasFlag(ExtraSettingFlags.HeroSkipNormalEntry))
			{
				instance.skipNormalEntry = true;
			}
		}
	}
}
