using TeamCherry.Cinematics;
using UnityEngine;

public class StagTravel : FastTravelCutscene
{
	[Space]
	[SerializeField]
	private CinematicVideoReference normalVideo;

	[SerializeField]
	private CinematicVideoReference childrenVideo;

	[Space]
	[SerializeField]
	private CinematicVideoReference normalVideo30FPS;

	[SerializeField]
	private CinematicVideoReference childrenVideo30FPS;

	[Space]
	[SerializeField]
	private VibrationDataAsset normalVideoVibration;

	[SerializeField]
	private VibrationDataAsset childVideoVibration;

	private bool isReadyToActivate;

	protected override bool IsReadyToActivate => isReadyToActivate;

	protected override bool ShouldFlipX
	{
		get
		{
			GameManager instance = GameManager.instance;
			Vector2 directionBetweenScenes = instance.gameMap.GetDirectionBetweenScenes(instance.lastSceneName, instance.playerData.nextScene);
			if (directionBetweenScenes.x != 0f)
			{
				return directionBetweenScenes.x < 0f;
			}
			return false;
		}
	}

	private bool UseChildrenVersion()
	{
		return PlayerData.instance.UnlockedFastTravelTeleport;
	}

	protected override CinematicVideoReference GetVideoReference()
	{
		if (!UseChildrenVersion())
		{
			return GetNormalVideo();
		}
		return GetChildrenVideo();
	}

	protected override void OnSkipped()
	{
		isReadyToActivate = true;
	}

	protected override void OnFadedOut()
	{
		isReadyToActivate = true;
	}

	protected override VibrationDataAsset GetVibrationData()
	{
		if (!UseChildrenVersion())
		{
			return normalVideoVibration;
		}
		return childVideoVibration;
	}

	private CinematicVideoReference GetChildrenVideo()
	{
		if (Platform.Current.MaxVideoFrameRate <= 30 && (bool)childrenVideo30FPS)
		{
			return childrenVideo30FPS;
		}
		return childrenVideo;
	}

	private CinematicVideoReference GetNormalVideo()
	{
		if (Platform.Current.MaxVideoFrameRate <= 30 && (bool)normalVideo30FPS)
		{
			return normalVideo30FPS;
		}
		return normalVideo;
	}
}
