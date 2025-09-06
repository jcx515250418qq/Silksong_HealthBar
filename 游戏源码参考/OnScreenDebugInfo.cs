using System;
using System.Collections;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnScreenDebugInfo : MonoBehaviour, IOnGUI
{
	private GameManager gm;

	private InputHandler ih;

	private float unloadTime;

	private float loadTime;

	private float frameRate;

	private string fps;

	private string infoString;

	private string versionNumber;

	private const float textWidth = 100f;

	private Rect loadProfilerRect;

	private Rect fpsRect;

	private Rect infoRect;

	private Rect inputRect;

	private Rect tfrRect;

	private bool showFPS;

	private bool showInfo;

	private bool showInput;

	private bool showLoadingTime;

	private bool showTFR;

	public int GUIDepth => 0;

	private void Awake()
	{
		fpsRect = new Rect(7f, 5f, 100f, 25f);
		infoRect = new Rect(Screen.width - 105, 5f, 100f, 70f);
		inputRect = new Rect(7f, 65f, 300f, 120f);
		loadProfilerRect = new Rect((float)(Screen.width / 2) - 50f, 5f, 100f, 25f);
		tfrRect = new Rect(7f, 20f, 100f, 25f);
	}

	private IEnumerator Start()
	{
		gm = GameManager.instance;
		gm.UnloadingLevel += OnLevelUnload;
		ih = gm.inputHandler;
		RetrieveInfo();
		GUI.depth = 2;
		while (showFPS)
		{
			if (Time.timeScale == 1f)
			{
				yield return new WaitForSeconds(0.1f);
				frameRate = 1f / Time.deltaTime;
				fps = "FPS :" + Mathf.Round(frameRate);
			}
			else
			{
				fps = "Pause";
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void LevelActivated(Scene sceneFrom, Scene sceneTo)
	{
		RetrieveInfo();
		if (showLoadingTime)
		{
			loadTime = (float)Math.Round(Time.realtimeSinceStartup - unloadTime, 2);
		}
	}

	private void OnEnable()
	{
		SceneManager.activeSceneChanged += LevelActivated;
	}

	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= LevelActivated;
		if (gm != null)
		{
			gm.UnloadingLevel -= OnLevelUnload;
		}
		GUIDrawer.RemoveDrawer(this);
	}

	public void DrawGUI()
	{
		if (showInfo)
		{
			if (showFPS)
			{
				GUI.Label(fpsRect, fps);
			}
			if (showInfo)
			{
				GUI.Label(infoRect, infoString);
			}
			if (showInput)
			{
				GUI.Label(inputRect, ReadInput());
			}
			if (showLoadingTime)
			{
				GUI.Label(loadProfilerRect, loadTime + "s");
			}
			if (showTFR)
			{
				GUI.Label(tfrRect, "TFR: " + Application.targetFrameRate);
			}
		}
	}

	public void ShowFPS()
	{
		showFPS = !showFPS;
	}

	public void ShowGameInfo()
	{
		showInfo = !showInfo;
		if (showInfo)
		{
			GUIDrawer.AddDrawer(this);
		}
		else
		{
			GUIDrawer.RemoveDrawer(this);
		}
	}

	public void ShowInput()
	{
		showInput = !showInput;
	}

	public void ShowLoadingTime()
	{
		showLoadingTime = !showLoadingTime;
	}

	public void ShowTargetFrameRate()
	{
		showTFR = !showTFR;
	}

	private void OnLevelUnload()
	{
		unloadTime = Time.realtimeSinceStartup;
	}

	private void RetrieveInfo()
	{
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		versionNumber = "1.0.28324";
		infoString = Language.Get("GAME_TITLE") + "\r\n" + versionNumber + " " + Language.CurrentLanguage().ToString() + "\r\n" + gm.GetSceneNameString();
	}

	private string ReadInput()
	{
		return string.Concat(string.Concat(string.Concat("" + $"Move Vector: {ih.inputActions.MoveVector.Vector.x.ToString()}, {ih.inputActions.MoveVector.Vector.y.ToString()}", $"\nMove Pressed: {ih.inputActions.Left.IsPressed || ih.inputActions.Right.IsPressed}"), $"\nMove Raw L: {ih.inputActions.Left.RawValue} R: {ih.inputActions.Right.RawValue}"), $"\nAny Key Down: {InputManager.AnyKeyIsPressed}");
	}
}
