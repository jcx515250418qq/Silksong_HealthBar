using System.Collections;
using GlobalEnums;
using GlobalSettings;
using UnityEngine;

public class CutsceneHelper : MonoBehaviour, GameManager.ISkippable
{
	public enum NextScene
	{
		SpecifyScene = 0,
		MainMenu = 1,
		PermaDeathUnlock = 2,
		GameCompletionScreen = 3,
		EndCredits = 4,
		MrMushroomUnlock = 5,
		GGReturn = 6,
		MainMenuNoSave = 7
	}

	public float waitBeforeFadeIn;

	public CameraFadeInType fadeInSpeed;

	public SkipPromptMode skipMode;

	[SerializeField]
	private AudioSource skipAudioSource;

	[Tooltip("Prevents the skip action from taking place until the lock is released. Useful for animators delaying skip feature.")]
	public bool startSkipLocked;

	public NextScene nextSceneType;

	public string nextScene;

	[SerializeField]
	private bool fadeTransitionAudioOnSkip;

	private GameManager gm;

	private bool isLoadingScene;

	private bool skipped;

	private IEnumerator Start()
	{
		gm = GameManager.instance;
		gm.RegisterSkippable(this);
		skipped = false;
		if (startSkipLocked)
		{
			gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		}
		else
		{
			gm.inputHandler.SetSkipMode(skipMode);
		}
		GameCameras.instance.DisableHUDCamIfAllowed();
		yield return new WaitForSeconds(waitBeforeFadeIn);
		if (fadeInSpeed == CameraFadeInType.SLOW)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN SLOWLY");
		}
		else if (fadeInSpeed == CameraFadeInType.NORMAL)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
		}
		else if (fadeInSpeed == CameraFadeInType.INSTANT)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN INSTANT");
		}
	}

	private void OnDestroy()
	{
		if ((bool)gm)
		{
			gm.DeregisterSkippable(this);
		}
	}

	public void LoadNextScene()
	{
		GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE INSTANT");
		DoSceneLoad();
	}

	public IEnumerator Skip()
	{
		if (!skipped)
		{
			skipped = true;
			Audio.StopConfirmSound.PlayOnSource(skipAudioSource);
			if (fadeTransitionAudioOnSkip)
			{
				TransitionAudioFader.FadeOutAllFaders();
			}
			PlayMakerFSM.BroadcastEvent("JUST FADE");
			yield return new WaitForSeconds(0.5f);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			DoSceneLoad();
		}
	}

	public void UnlockSkip()
	{
		gm.inputHandler.SetSkipMode(skipMode);
	}

	private void DoSceneLoad()
	{
		if (!isLoadingScene)
		{
			InputHandler instance = ManagerSingleton<InputHandler>.Instance;
			if ((bool)instance)
			{
				instance.StopAcceptingInput();
			}
			isLoadingScene = true;
			switch (nextSceneType)
			{
			case NextScene.SpecifyScene:
				GameManager.instance.LoadScene(nextScene);
				break;
			case NextScene.MainMenu:
				GameManager.instance.StartCoroutine(GameManager.instance.ReturnToMainMenu(willSave: true, null, isEndGame: true));
				break;
			case NextScene.PermaDeathUnlock:
				GameManager.instance.LoadPermadeathUnlockScene();
				break;
			case NextScene.GameCompletionScreen:
				GameManager.instance.LoadScene("End_Game_Completion");
				break;
			case NextScene.EndCredits:
				GameManager.instance.LoadScene("End_Credits");
				break;
			case NextScene.GGReturn:
				GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
				{
					SceneName = nextScene,
					EntryGateName = GameManager.instance.playerData.bossReturnEntryGate,
					EntryDelay = 0f,
					PreventCameraFadeOut = true,
					WaitForSceneTransitionCameraFade = false
				});
				break;
			case NextScene.MainMenuNoSave:
				GameManager.instance.StartCoroutine(GameManager.instance.ReturnToMainMenu(willSave: false));
				break;
			case NextScene.MrMushroomUnlock:
				break;
			}
		}
	}
}
