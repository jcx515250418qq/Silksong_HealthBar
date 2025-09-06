using UnityEngine;

public class MakeSkippable : MonoBehaviour
{
	private enum SkipType
	{
		Inactive = 0,
		Cinematic = 1,
		Cutscene = 2
	}

	public float unlockAfterSec;

	private CinematicPlayer cinematicPlayer;

	private CutsceneHelper cutsceneHelper;

	private SkipType skipType;

	private void Awake()
	{
		cinematicPlayer = GetComponent<CinematicPlayer>();
		cutsceneHelper = GetComponent<CutsceneHelper>();
		if (cinematicPlayer != null)
		{
			skipType = SkipType.Cinematic;
			return;
		}
		if (cutsceneHelper != null)
		{
			skipType = SkipType.Cutscene;
			return;
		}
		skipType = SkipType.Inactive;
		Debug.LogError("MakeSkippable requires a Cinematic Player or Cutscene Helper component.");
	}

	private void Start()
	{
		if (skipType != 0)
		{
			if (unlockAfterSec <= 0f)
			{
				UnlockSkip();
			}
			else
			{
				Invoke("UnlockSkip", unlockAfterSec);
			}
		}
	}

	private void UnlockSkip()
	{
		if (skipType == SkipType.Cinematic)
		{
			if (cinematicPlayer != null)
			{
				cinematicPlayer.UnlockSkip();
			}
			else
			{
				Debug.LogError("MakeSkippable - Cinematic Player is null");
			}
		}
		else if (skipType == SkipType.Cutscene)
		{
			if (cutsceneHelper != null)
			{
				cutsceneHelper.UnlockSkip();
			}
			else
			{
				Debug.LogError("MakeSkippable - Cutscene Helper is null");
			}
		}
	}
}
