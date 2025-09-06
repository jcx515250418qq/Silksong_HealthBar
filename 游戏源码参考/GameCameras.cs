using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class GameCameras : MonoBehaviour
{
	[Header("Cameras")]
	public Camera hudCamera;

	public Camera mainCamera;

	[Header("Controllers")]
	public CameraController cameraController;

	public CameraTarget cameraTarget;

	public ForceCameraAspect forceCameraAspect;

	public WorldRumbleManager worldRumbleManager;

	[Header("FSMs")]
	public PlayMakerFSM cameraFadeFSM;

	public PlayMakerFSM cameraShakeFSM;

	public PlayMakerFSM soulOrbFSM;

	public PlayMakerFSM soulVesselFSM;

	public PlayMakerFSM openStagFSM;

	public PlayMakerFSM hudCanvasSlideOut;

	[Header("Camera Effects")]
	public ColorCorrectionCurves colorCorrectionCurves;

	public SceneColorManager sceneColorManager;

	public BrightnessEffect brightnessEffect;

	public SceneParticlesController sceneParticlesPrefab;

	private SceneParticlesController sceneParticles;

	[Header("Other")]
	public tk2dCamera tk2dCam;

	public Transform cameraParent;

	public SilkSpool silkSpool;

	private GameManager gm;

	private GameSettings gs;

	private CanvasScaler canvasScaler;

	private bool init;

	private static GameCameras _instance;

	public static GameCameras instance
	{
		get
		{
			GameCameras silentInstance = SilentInstance;
			if (!silentInstance)
			{
				Debug.LogError("Couldn't find GameCameras, make sure one exists in the scene.");
			}
			return silentInstance;
		}
	}

	public static GameCameras SilentInstance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<GameCameras>();
				if ((bool)_instance && Application.isPlaying)
				{
					Object.DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}

	public bool IsHudVisible
	{
		get
		{
			if (!hudCanvasSlideOut)
			{
				return false;
			}
			return hudCanvasSlideOut.FsmVariables.FindFsmBool("Is Visible")?.Value ?? false;
		}
	}

	public bool IsInCinematic { get; private set; }

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			Object.DontDestroyOnLoad(this);
		}
		else if (this != _instance)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				Object.DestroyImmediate(base.gameObject);
			}
		}
	}

	private void Start()
	{
		gs.LoadOverscanSettings();
		SetOverscan(gs.overScanAdjustment);
	}

	public void SceneInit()
	{
		if (this == _instance)
		{
			StartScene();
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
		if (Application.isPlaying)
		{
			Object.Destroy(sceneParticles);
		}
		else
		{
			Object.DestroyImmediate(sceneParticles);
		}
	}

	private void SetupGameRefs()
	{
		gm = GameManager.instance;
		gs = gm.gameSettings;
		canvasScaler = UIManager.instance.canvasScaler;
		if (cameraController != null)
		{
			cameraController.GameInit();
		}
		else
		{
			Debug.LogError("CameraController not set in inspector.");
		}
		if (cameraTarget != null)
		{
			cameraTarget.GameInit();
		}
		else
		{
			Debug.LogError("CameraTarget not set in inspector.");
		}
		if (sceneParticlesPrefab != null)
		{
			sceneParticles = Object.Instantiate(sceneParticlesPrefab);
			sceneParticles.name = "SceneParticlesController";
			sceneParticles.transform.position = new Vector3(tk2dCam.transform.position.x, tk2dCam.transform.position.y, 0f);
			sceneParticles.transform.SetParent(tk2dCam.transform);
		}
		else
		{
			Debug.LogError("Scene Particles Prefab not set in inspector.");
		}
		if (sceneColorManager != null)
		{
			sceneColorManager.GameInit();
		}
		else
		{
			Debug.LogError("SceneColorManager not set in inspector.");
		}
		init = true;
	}

	private void StartScene()
	{
		if (!init)
		{
			SetupGameRefs();
		}
		if (gm.IsMenuScene())
		{
			MoveMenuToHUDCamera();
			hudCamera.GetComponent<HUDCamera>().SetIsGameplayMode(isGameplayMode: false);
			if (!hudCamera.gameObject.activeSelf)
			{
				hudCamera.gameObject.SetActive(value: true);
			}
		}
		else if (gm.IsGameplayScene() || gm.ShouldKeepHUDCameraActive())
		{
			MoveMenuToHUDCamera();
			HUDCamera component = hudCamera.GetComponent<HUDCamera>();
			component.EnsureGameMapSpawned();
			component.SetIsGameplayMode(isGameplayMode: true);
			if (!hudCamera.gameObject.activeSelf)
			{
				hudCamera.gameObject.SetActive(value: true);
			}
			if ((bool)gm.gameMap)
			{
				gm.gameMap.InitPinUpdate();
			}
		}
		else
		{
			DisableHUDCamIfAllowed();
			if (!hudCamera.gameObject.activeSelf && gm.IsCinematicScene())
			{
				MoveMenuToMainCamera();
			}
		}
		if (gm.IsMenuScene())
		{
			cameraController.transform.SetPosition2D(14.6f, 8.5f);
		}
		else if (gm.IsCinematicScene())
		{
			cameraController.transform.SetPosition2D(14.6f, 8.5f);
		}
		else if (gm.IsNonGameplayScene())
		{
			if (gm.IsBossDoorScene())
			{
				cameraController.transform.SetPosition2D(17.5f, 17.5f);
			}
			else if (InGameCutsceneInfo.IsInCutscene)
			{
				cameraController.transform.SetPosition2D(InGameCutsceneInfo.CameraPosition);
			}
			else
			{
				cameraController.transform.SetPosition2D(14.6f, 8.5f);
			}
		}
		cameraController.SceneInit();
		cameraTarget.SceneInit();
		sceneColorManager.SceneInit();
		sceneParticles.SceneInit();
	}

	public void MoveMenuToHUDCamera()
	{
		UIManager.instance.UICanvas.worldCamera = hudCamera;
		UIManager.instance.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
		UIManager.instance.GenericMessageCanvas.worldCamera = hudCamera;
		UIManager.instance.GenericMessageCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		hudCamera.cullingMask = 32;
		mainCamera.cullingMask &= ~hudCamera.cullingMask;
	}

	private void MoveMenuToMainCamera()
	{
		UIManager.instance.UICanvas.worldCamera = mainCamera;
		UIManager.instance.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
		UIManager.instance.GenericMessageCanvas.worldCamera = mainCamera;
		UIManager.instance.GenericMessageCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		hudCamera.cullingMask = 0;
		mainCamera.cullingMask |= 32;
	}

	public void SetMainCameraActive(bool value)
	{
		mainCamera.enabled = value;
		if (value)
		{
			hudCamera.clearFlags = CameraClearFlags.Depth;
			return;
		}
		hudCamera.clearFlags = CameraClearFlags.Color;
		hudCamera.backgroundColor = Color.black;
	}

	public void DisableHUDCamIfAllowed()
	{
		if (gm.IsNonGameplayScene() && !gm.IsStagTravelScene() && !gm.IsBossDoorScene() && !gm.ShouldKeepHUDCameraActive())
		{
			hudCamera.gameObject.SetActive(value: false);
		}
	}

	public void StopCameraShake()
	{
		cameraShakeFSM.Fsm.Event("CANCEL SHAKE");
	}

	public void ResumeCameraShake()
	{
		cameraShakeFSM.Fsm.Event("RESUME SHAKE");
	}

	public void OnCinematicBegin()
	{
		IsInCinematic = true;
		cameraController.ApplyEffectConfiguration();
		mainCamera.GetComponent<BloomOptimized>().enabled = false;
		mainCamera.GetComponent<ColorCorrectionCurves>().enabled = false;
	}

	public void OnCinematicEnd()
	{
		IsInCinematic = false;
		cameraController.ApplyEffectConfiguration();
	}

	public void SetOverscan(float value)
	{
		if (!init)
		{
			SetupGameRefs();
		}
		Invoke("TestParentForPosition", 0.33f * Time.timeScale);
		float num = (float)Screen.width / (float)Screen.height;
		if (canvasScaler == null)
		{
			canvasScaler = UIManager.instance.canvasScaler;
		}
		canvasScaler.referenceResolution = new Vector2(1920f * (1f - value) + -220f * value, 1080f * (1f - value) + 1f / num * (-220f * value));
		forceCameraAspect.SetOverscanViewport(value);
		gs.overScanAdjustment = value;
	}

	public void TestParentForPosition()
	{
		if (cameraParent.transform.localPosition.z != 0f)
		{
			cameraParent.transform.localPosition = new Vector3(cameraParent.transform.localPosition.x, cameraParent.transform.localPosition.y, 0f);
		}
	}

	public void HUDOut()
	{
		hudCanvasSlideOut.SendEventSafe("OUT");
	}

	public void HUDIn()
	{
		hudCanvasSlideOut.SendEventSafe("IN");
	}
}
