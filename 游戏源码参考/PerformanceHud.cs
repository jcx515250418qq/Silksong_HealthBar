using System.Collections.Generic;
using System.Text;
using TeamCherry.BuildBot;
using TeamCherry.SharedUtils;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class PerformanceHud : MonoBehaviour, IOnGUI
{
	private class LoadReport
	{
		public Color Color;

		public GUIContent Content;
	}

	public enum DisplayStates
	{
		Hidden = 0,
		Minimal = 1,
		Full = 2
	}

	private class VibrationHudDrawer
	{
		private class VibrationTracker
		{
			private VibrationEmission emission;

			private float timer;

			public VibrationEmission Emission => emission;

			public VibrationTracker(VibrationEmission emission)
			{
				this.emission = emission;
				timer = 5f;
			}

			public bool Update()
			{
				if (Emission == null)
				{
					return false;
				}
				if (!Emission.IsPlaying)
				{
					timer -= Time.deltaTime;
				}
				return timer > 0f;
			}

			public override string ToString()
			{
				if (emission != null)
				{
					if (emission.IsPlaying)
					{
						return $"{emission}";
					}
					return $"{emission} finished";
				}
				return "Empty";
			}

			public override bool Equals(object obj)
			{
				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}
				VibrationTracker vibrationTracker = (VibrationTracker)obj;
				return object.Equals(Emission, vibrationTracker.Emission);
			}

			public override int GetHashCode()
			{
				return Emission?.GetHashCode() ?? 0;
			}
		}

		private PerformanceHud performanceHud;

		private const float DISPLAY_TIME = 5f;

		private HashSet<VibrationEmission> activeEmissions = new HashSet<VibrationEmission>();

		private List<VibrationTracker> trackers = new List<VibrationTracker>();

		private GUIContent vibrationsContent = new GUIContent("");

		public GUIContent VibrationsContent => vibrationsContent;

		public VibrationHudDrawer(PerformanceHud performanceHud)
		{
			this.performanceHud = performanceHud;
		}

		public void Update()
		{
			VibrationMixer mixer = VibrationManager.GetMixer();
			if (mixer != null)
			{
				for (int i = 0; i < mixer.PlayingEmissionCount; i++)
				{
					VibrationEmission playingEmission = mixer.GetPlayingEmission(i);
					if (activeEmissions.Add(playingEmission))
					{
						trackers.Add(new VibrationTracker(playingEmission));
					}
				}
			}
			for (int num = trackers.Count - 1; num >= 0; num--)
			{
				VibrationTracker vibrationTracker = trackers[num];
				if (!vibrationTracker.Update())
				{
					activeEmissions.Remove(vibrationTracker.Emission);
					trackers.RemoveAt(num);
				}
			}
		}

		public void OnGUI()
		{
			GUI.color = Color.white;
			for (int num = trackers.Count - 1; num >= 0; num--)
			{
				VibrationTracker vibrationTracker = trackers[num];
				vibrationsContent.text = vibrationTracker.ToString();
				LabelWithShadowRight(vibrationsContent, ref performanceHud.rightLineIndex);
			}
			if (trackers.Count > 0)
			{
				performanceHud.rightLineIndex++;
			}
		}
	}

	private int frameCounter;

	private int lastSecond;

	private int framesLastSecond;

	private Color framesColor;

	private UIDocument uiDoc;

	private Label sceneLabel;

	private Label cpuMemLabel;

	private Label resolutionLabel;

	private Label fpsLabel;

	private Label profileLabel;

	private VisualElement monoGroup;

	private Label gcLabel;

	private Label heapLabel;

	private int previousFpsValue;

	private Dictionary<int, string> fpsStringValues;

	private int lastScreenWidth;

	private int lastScreenHeight;

	private int lastScreenWidthScaled;

	private int lastScreenHeightScaled;

	private StringBuilder sceneInfoBuilder;

	private GUIContent memoryContent;

	private GarbageCollector.Mode? lastGcMode;

	private List<LoadReport> loadReports;

	private static bool _showVibrations;

	private bool isProfileRecordingEnabled;

	private ProfilerRecorder setPassCallRecorder;

	private ProfilerRecorder trianglesRecorder;

	private GUIStyle rightAlignedStyle;

	private bool isDrawing;

	private const int LINE_HEIGHT = 24;

	private VibrationHudDrawer vibrationHudDrawer;

	private int rightLineIndex;

	private DisplayStates displayState;

	private bool isMonoGroupEnabled = true;

	private const float SCREEN_EDGE_PADDING = 5f;

	public static PerformanceHud Shared { get; private set; }

	public static bool ShowVibration
	{
		get
		{
			return _showVibrations;
		}
		set
		{
			if (_showVibrations != value)
			{
				_showVibrations = value;
				if (Shared != null)
				{
					Shared.UpdateDrawState();
				}
			}
		}
	}

	private GUIStyle RightAlignedStyle
	{
		get
		{
			if (rightAlignedStyle == null)
			{
				rightAlignedStyle = new GUIStyle(GUI.skin.label);
				rightAlignedStyle.alignment = TextAnchor.MiddleRight;
			}
			return rightAlignedStyle;
		}
	}

	private static int LineHeight => Mathf.RoundToInt(24f * CheatManager.Multiplier);

	public bool IsMonoGroupEnabled
	{
		get
		{
			return isMonoGroupEnabled;
		}
		set
		{
			isMonoGroupEnabled = value;
			if (monoGroup != null)
			{
				monoGroup.style.display = ((!value) ? DisplayStyle.None : DisplayStyle.Flex);
			}
		}
	}

	public DisplayStates DisplayState
	{
		get
		{
			return displayState;
		}
		set
		{
			if (displayState == value)
			{
				return;
			}
			DisplayStates displayStates = displayState;
			displayState = value;
			UpdateDrawState();
			if (displayState != 0)
			{
				if (displayStates == DisplayStates.Hidden)
				{
					if (!uiDoc)
					{
						uiDoc = base.gameObject.AddComponent<UIDocument>();
						uiDoc.panelSettings = Object.Instantiate(Resources.Load<PanelSettings>("DebugPanelSettings"));
					}
					VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("PerformanceHud");
					StyleSheet styleSheet = Resources.Load<StyleSheet>("PerformanceHud");
					uiDoc.visualTreeAsset = visualTreeAsset;
					uiDoc.rootVisualElement.styleSheets.Add(styleSheet);
					VisualElement rootVisualElement = uiDoc.rootVisualElement;
					Label label = rootVisualElement.Q<Label>("revisionLabel");
					BuildMetadata embedded = BuildMetadata.Embedded;
					label.text = ((embedded != null) ? ("r" + embedded.Revision + " - " + embedded.MachineName) : "No Build Metadata");
					sceneLabel = rootVisualElement.Q<Label>("sceneLabel");
					cpuMemLabel = rootVisualElement.Q<Label>("cpuMemLabel");
					resolutionLabel = rootVisualElement.Q<Label>("resolutionLabel");
					fpsLabel = rootVisualElement.Q<Label>("fpsLabel");
					profileLabel = rootVisualElement.Q<Label>("profileLabel");
					monoGroup = rootVisualElement.Q<VisualElement>("monoGroup");
					IsMonoGroupEnabled = IsMonoGroupEnabled;
					gcLabel = rootVisualElement.Q<Label>("gcLabel");
					UpdateGCMode(GarbageCollector.GCMode);
					heapLabel = rootVisualElement.Q<Label>("monoHeapLabel");
					UpdateAll();
					UpdateScene();
				}
			}
			else if ((bool)uiDoc)
			{
				uiDoc.visualTreeAsset = null;
				sceneLabel = null;
				cpuMemLabel = null;
				resolutionLabel = null;
				fpsLabel = null;
				profileLabel = null;
				monoGroup = null;
				gcLabel = null;
				heapLabel = null;
				fpsStringValues = null;
				lastScreenHeight = 0;
				lastScreenWidth = 0;
				lastScreenHeightScaled = 0;
				lastScreenWidthScaled = 0;
				previousFpsValue = 0;
				lastGcMode = null;
			}
		}
	}

	public bool EnableProfileRecording
	{
		get
		{
			return isProfileRecordingEnabled;
		}
		set
		{
			if (value != isProfileRecordingEnabled)
			{
				isProfileRecordingEnabled = value;
				if (value)
				{
					setPassCallRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
					trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
				}
				else
				{
					setPassCallRecorder.Dispose();
					trianglesRecorder.Dispose();
				}
				UpdateResolution();
			}
		}
	}

	public int GUIDepth => 1;

	public static float ScreenEdgePadding => 5f * CheatManager.Multiplier;

	public static void Init()
	{
		if (!(Shared != null))
		{
			GameObject obj = new GameObject("PerformanceHud");
			Shared = obj.AddComponent<PerformanceHud>();
			Shared.DisplayState = DisplayStates.Hidden;
			Object.DontDestroyOnLoad(obj);
		}
	}

	public static void ReInit()
	{
		if ((bool)Shared)
		{
			Object.Destroy(Shared.gameObject);
			Shared = null;
		}
		Init();
	}

	protected void Awake()
	{
		frameCounter = 0;
		lastSecond = (int)Time.realtimeSinceStartup;
		framesColor = Color.gray;
		memoryContent = new GUIContent("N/A");
		loadReports = new List<LoadReport>();
		vibrationHudDrawer = new VibrationHudDrawer(this);
	}

	protected void OnEnable()
	{
		GameManager.SceneTransitionBegan += GameManager_SceneTransitionBegan;
		SceneManager.activeSceneChanged += OnActiveSceneChanged;
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
		GarbageCollector.GCModeChanged += UpdateGCMode;
	}

	protected void OnDisable()
	{
		GameManager.SceneTransitionBegan -= GameManager_SceneTransitionBegan;
		SceneManager.activeSceneChanged -= OnActiveSceneChanged;
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
		GarbageCollector.GCModeChanged -= UpdateGCMode;
		EnableProfileRecording = false;
	}

	private void OnDestroy()
	{
		ToggleDraw(draw: false);
	}

	protected void Update()
	{
		if (DisplayState != 0)
		{
			frameCounter++;
			int num = (int)Time.realtimeSinceStartup;
			if (num != lastSecond)
			{
				framesLastSecond = frameCounter;
				int num2 = framesLastSecond;
				Color color = ((num2 >= 58) ? Color.green : ((num2 < 50) ? Color.red : Color.yellow));
				framesColor = color;
				lastSecond = num;
				frameCounter = 0;
				UpdateAll();
			}
			if (ShowVibration)
			{
				vibrationHudDrawer.Update();
			}
		}
	}

	private void ToggleDraw(bool draw)
	{
		if (isDrawing != draw)
		{
			isDrawing = draw;
			if (draw)
			{
				GUIDrawer.AddDrawer(this);
			}
			else
			{
				GUIDrawer.RemoveDrawer(this);
			}
			if ((bool)uiDoc)
			{
				uiDoc.enabled = draw;
			}
		}
	}

	private void UpdateDrawState()
	{
		if (displayState == DisplayStates.Hidden)
		{
			ToggleDraw(ShowVibration);
		}
		else
		{
			ToggleDraw(draw: true);
		}
	}

	private void GameManager_SceneTransitionBegan(SceneLoad sceneLoad)
	{
		LoadReport loadReport = new LoadReport
		{
			Color = Color.white,
			Content = new GUIContent()
		};
		loadReports.Add(loadReport);
		while (loadReports.Count > 2)
		{
			loadReports.RemoveAt(0);
		}
		sceneLoad.FetchComplete += delegate
		{
			UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.ActivationComplete += delegate
		{
			UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.Complete += delegate
		{
			UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.StartCalled += delegate
		{
			UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.BossLoaded += delegate
		{
			UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.Finish += delegate
		{
			UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		UpdateSceneLoadRecordContent(sceneLoad, loadReport);
	}

	private static void UpdateSceneLoadRecordContent(SceneLoad sceneLoad, LoadReport report)
	{
		StringBuilder tempStringBuilder = Helper.GetTempStringBuilder(sceneLoad.TargetSceneName);
		tempStringBuilder.Append(":    ");
		float num = 0f;
		for (int i = 0; i < 9; i++)
		{
			SceneLoad.Phases phase = (SceneLoad.Phases)i;
			float? duration = sceneLoad.GetDuration(phase);
			if (duration.HasValue && !(duration.Value <= Mathf.Epsilon))
			{
				tempStringBuilder.Append(phase.ToString());
				tempStringBuilder.Append(": ");
				tempStringBuilder.Append(duration.Value.ToString("0.00s"));
				tempStringBuilder.Append("    ");
				num += duration.Value;
			}
		}
		if (num > Mathf.Epsilon)
		{
			tempStringBuilder.Append("Total: ");
			tempStringBuilder.Append(num.ToString("0.00s"));
		}
		Color color = ((num > 3.5f) ? Color.red : ((!(num > 3f)) ? Color.white : Color.yellow));
		report.Color = color;
		report.Content.text = tempStringBuilder.ToString();
	}

	private void UpdateAll()
	{
		UpdateMemory();
		UpdateResolution();
	}

	private void UpdateMemory()
	{
		double num = (double)GCManager.GetMemoryUsage() / 1024.0 / 1024.0;
		double num2 = (double)GCManager.GetMemoryTotal() / 1024.0 / 1024.0;
		double num3 = SystemInfo.systemMemorySize;
		if (cpuMemLabel != null)
		{
			cpuMemLabel.text = $"CPU Mem.: {num:n} / {num2:n} / {num3:n}";
		}
		Label label = heapLabel;
		if (label != null && label.visible)
		{
			double num4 = (double)GCManager.GetMonoHeapUsage() / 1024.0 / 1024.0;
			double num5 = (double)GCManager.GetMonoHeapTotal() / 1024.0 / 1024.0;
			heapLabel.text = $"Heap: {num4:n} / {num5:n} / {GCManager.HeapUsageThreshold:n}";
		}
	}

	private void UpdateGCMode(GarbageCollector.Mode mode)
	{
		if (gcLabel != null && mode != lastGcMode)
		{
			gcLabel.text = "GC: " + mode;
			lastGcMode = mode;
		}
	}

	private void OnActiveSceneChanged(Scene fromScene, Scene toScene)
	{
		UpdateScene();
	}

	private void OnSceneUnloaded(Scene arg0)
	{
		UpdateScene();
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		UpdateScene();
	}

	private void UpdateScene()
	{
		if (sceneInfoBuilder == null)
		{
			sceneInfoBuilder = new StringBuilder();
		}
		else
		{
			sceneInfoBuilder.Clear();
		}
		Scene activeScene = SceneManager.GetActiveScene();
		bool flag = false;
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (sceneAt.isLoaded)
			{
				if (flag)
				{
					sceneInfoBuilder.Append(" + ");
				}
				bool flag2 = false;
				if (sceneAt != activeScene)
				{
					sceneInfoBuilder.Append("<color=#808080>");
					flag2 = true;
				}
				sceneInfoBuilder.Append(sceneAt.name);
				flag = true;
				if (flag2)
				{
					sceneInfoBuilder.Append("</color>");
				}
			}
		}
		if (sceneLabel != null)
		{
			sceneLabel.text = sceneInfoBuilder.ToString();
		}
	}

	private void UpdateResolution()
	{
		if (profileLabel != null)
		{
			if (isProfileRecordingEnabled)
			{
				profileLabel.text = $"- SetPass Calls: {setPassCallRecorder.LastValue}, Triangles: {trianglesRecorder.LastValue}";
				profileLabel.style.display = DisplayStyle.Flex;
			}
			else
			{
				profileLabel.style.display = DisplayStyle.None;
			}
		}
		int width = Screen.width;
		int height = Screen.height;
		ScreenRes resolution = CameraRenderScaled.Resolution;
		if (resolutionLabel != null && (width != lastScreenWidth || height != lastScreenHeight || resolution.Width != lastScreenWidthScaled || resolution.Height != lastScreenHeightScaled))
		{
			resolutionLabel.text = $"{width}x{height} [{resolution.Width}x{resolution.Height}]";
			lastScreenWidth = width;
			lastScreenHeight = height;
			lastScreenWidthScaled = resolution.Width;
			lastScreenHeightScaled = resolution.Height;
			uiDoc.panelSettings.scale = CheatManager.Multiplier;
		}
		if (fpsLabel != null && framesLastSecond != previousFpsValue)
		{
			if (fpsStringValues == null)
			{
				fpsStringValues = new Dictionary<int, string>(60);
			}
			if (!fpsStringValues.TryGetValue(framesLastSecond, out var value))
			{
				value = (fpsStringValues[framesLastSecond] = framesLastSecond.ToString());
			}
			fpsLabel.text = value;
			fpsLabel.style.color = framesColor;
			previousFpsValue = framesLastSecond;
		}
	}

	public void DrawGUI()
	{
		if (ShowVibration)
		{
			vibrationHudDrawer.OnGUI();
		}
		rightLineIndex = 1;
		if (DisplayState != DisplayStates.Full)
		{
			return;
		}
		int lineIndex = (IsMonoGroupEnabled ? 4 : 3);
		OnGUIFull(ref lineIndex);
		GUI.color = Color.white;
		LabelWithShadow(new GUIContent("Boost Mode: " + (CheatManager.BoostModeActive ? "Enabled" : "Disabled")), ref lineIndex);
		MazeController newestInstance = MazeController.NewestInstance;
		if (!newestInstance)
		{
			return;
		}
		LabelWithShadowRight(new GUIContent($"Incorrect Doors Left: {newestInstance.IncorrectDoorsLeft}"), ref rightLineIndex);
		LabelWithShadowRight(new GUIContent($"Correct Doors Left: {newestInstance.CorrectDoorsLeft}"), ref rightLineIndex);
		foreach (TransitionPoint item in newestInstance.EnumerateCorrectDoors())
		{
			LabelWithShadowRight(new GUIContent("Correct Door: " + (item ? item.gameObject.name : "none")), ref rightLineIndex);
		}
	}

	private void OnGUIFull(ref int lineIndex)
	{
		GUI.color = Color.white;
		LabelWithShadow(memoryContent, ref lineIndex);
		foreach (LoadReport loadReport in loadReports)
		{
			GUI.color = loadReport.Color;
			LabelWithShadow(loadReport.Content, ref lineIndex);
		}
		if ((bool)GameManager.instance && (bool)GameManager.instance.sm)
		{
			CustomSceneManager sm = GameManager.instance.sm;
			string text = $"Saturation: {sm.saturation}, Adjusted: {sm.AdjustSaturation(sm.saturation)}";
			GUI.color = Color.white;
			LabelWithShadow(new GUIContent(text), ref lineIndex);
		}
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			LabelWithShadow(new GUIContent("MapZone: " + unsafeInstance.GetCurrentMapZone()), ref lineIndex);
		}
		GUI.color = Color.white;
		LabelWithShadow(new GUIContent("Interaction: " + (InteractManager.CanInteract ? "Enabled" : "Disabled") + ", Blocked by: " + (InteractManager.BlockingInteractable ? InteractManager.BlockingInteractable.gameObject.name : "None")), ref lineIndex);
	}

	private static void LabelWithShadow(GUIContent content, ref int lineIndex)
	{
		lineIndex++;
		LabelWithShadow(new Rect(ScreenEdgePadding, (float)(Screen.height - LineHeight * lineIndex) - ScreenEdgePadding, (float)Screen.width - ScreenEdgePadding, LineHeight), content);
	}

	private static void LabelWithShadowRight(GUIContent content, ref int lineIndex)
	{
		lineIndex++;
		Vector2 vector = CheatManager.LabelStyle.CalcSize(content);
		LabelWithShadow(new Rect((float)Screen.width - vector.x - ScreenEdgePadding, (float)(Screen.height - LineHeight * lineIndex) - ScreenEdgePadding, vector.x, LineHeight), content);
	}

	private static void LabelWithShadow(Rect rect, GUIContent content)
	{
		GUIStyle labelStyle = CheatManager.LabelStyle;
		Vector2 vector = labelStyle.CalcSize(content);
		Color color = GUI.color;
		try
		{
			GUI.color = new Color(0f, 0f, 0f, 0.5f);
			GUI.DrawTexture(new Rect(rect.x, rect.y, vector.x, rect.height), Texture2D.whiteTexture);
			GUI.color = Color.black;
			GUI.Label(new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height), content, labelStyle);
			GUI.color = color;
			GUI.Label(new Rect(rect.x + 0f, rect.y + 0f, rect.width, rect.height), content, labelStyle);
		}
		finally
		{
			GUI.color = color;
		}
	}
}
