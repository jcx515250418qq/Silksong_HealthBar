using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour
{
	public enum CameraMode
	{
		FROZEN = 0,
		FOLLOWING = 1,
		LOCKED = 2,
		PANNING = 3,
		FADEOUT = 4,
		FADEIN = 5,
		PREVIOUS = 6
	}

	private bool verboseMode;

	public CameraMode mode;

	private CameraMode prevMode;

	public bool atSceneBounds;

	public bool atHorizontalSceneBounds;

	private bool isGameplayScene;

	public Vector3 lastFramePosition;

	public Vector2 lastLockPosition;

	private Coroutine fadeInFailSafeCo;

	[Header("Inspector Variables")]
	public float dampTime;

	public float dampTimeX;

	public float dampTimeY;

	public float dampTimeYSlow = 0.2f;

	public float dampTimeYSlowTimer;

	public float lookSlowTime = 0.35f;

	public bool isFalling;

	public bool isRising;

	public float dampTimeFalling;

	public float dampTimeRising;

	public float heroBotYLimit;

	private float panTime;

	private float currentPanTime;

	private Vector3 velocity;

	private Vector3 velocityX;

	private Vector3 velocityY;

	public float fallOffset;

	public float fallOffset_multiplier;

	public Vector3 destination;

	public float maxVelocity;

	public float maxVelocityFalling;

	private float maxVelocityCurrent;

	private float horizontalOffset;

	public float lookOffset;

	private float startLockedTimer;

	private float targetDeltaX;

	private float targetDeltaY;

	[HideInInspector]
	public Vector2 panToTarget;

	public float sceneWidth;

	public float sceneHeight;

	public float xLimit;

	public float yLimit;

	private CameraLockArea currentLockArea;

	private Vector3 panStartPos;

	private Vector3 panEndPos;

	[SerializeField]
	private PlayMakerFSM fadeFSM;

	public Camera cam;

	private HeroController hero_ctrl;

	private GameManager gm;

	public tk2dTileMap tilemap;

	public CameraTarget camTarget;

	private Transform cameraParent;

	public List<CameraLockArea> lockZoneList;

	public float xLockMin;

	public float xLockMax;

	public float yLockMin;

	public float yLockMax;

	public SimpleFadeOut screenFlash;

	public Color flash_lifeblood;

	public Color flash_poison;

	public Color flash_bomb;

	public Color flash_trobbio;

	public Color flash_frostStart;

	public Color flash_frostDamage;

	public Color flash_perfectDash;

	private bool isBloomForced;

	private HashSet<CameraLockArea> instantLockedArea = new HashSet<CameraLockArea>();

	private Coroutine positionToHeroCoroutine;

	private string lastInPositionScene;

	private static CameraController lastPositioner;

	private static readonly List<string> _forceBloomScenes = new List<string> { "Weave_10", "Slab_10b", "Shellwood_11b", "Coral_Tower_01", "Ant_Queen", "Clover_01", "Clover_20" };

	public float StartLockedTimer => startLockedTimer;

	public bool AllowExitingSceneBounds { get; private set; }

	public bool HasBeenPositionedAtHero
	{
		get
		{
			if (gm != null)
			{
				return lastInPositionScene == gm.sceneName;
			}
			return false;
		}
	}

	public static bool IsPositioningCamera { get; private set; }

	public CameraLockArea CurrentLockArea => currentLockArea;

	public bool IsBloomForced
	{
		get
		{
			return isBloomForced;
		}
		set
		{
			isBloomForced = value;
			ApplyEffectConfiguration();
		}
	}

	public event Action PositionedAtHero;

	public void GameInit()
	{
		gm = GameManager.instance;
		cam = GetComponent<Camera>();
		cameraParent = base.transform.parent.transform;
		ApplyEffectConfiguration();
		gm.UnloadingLevel += OnLevelUnload;
		gm.OnFinishedEnteringScene += OnFinishedEnteringScene;
	}

	private void OnFinishedEnteringScene()
	{
		startLockedTimer = 0f;
	}

	public void SceneInit()
	{
		ResetStartTimer();
		velocity = Vector3.zero;
		if (gm.IsGameplayScene())
		{
			isGameplayScene = true;
			if (hero_ctrl == null)
			{
				hero_ctrl = HeroController.instance;
				hero_ctrl.heroInPosition += PositionToHero;
			}
			lockZoneList = new List<CameraLockArea>();
			GetTilemapInfo();
			xLockMin = 0f;
			xLockMax = xLimit;
			yLockMin = 0f;
			yLockMax = yLimit;
			AllowExitingSceneBounds = false;
			dampTimeX = dampTime;
			dampTimeY = dampTime;
			maxVelocityCurrent = maxVelocity;
		}
		else
		{
			isGameplayScene = false;
		}
		ApplyEffectConfiguration();
	}

	public void ApplyEffectConfiguration()
	{
		bool flag = gm.IsGameplayScene();
		MapZone currentMapZoneEnum = gm.GetCurrentMapZoneEnum();
		string sceneNameString = gm.GetSceneNameString();
		bool flag2 = Platform.Current.GraphicsTier > Platform.GraphicsTiers.Low;
		FastNoise component = GetComponent<FastNoise>();
		component.Init();
		component.enabled = false;
		GetComponent<NewCameraNoise>().enabled = flag && flag2 && ConfigManager.IsNoiseEffectEnabled;
		BloomOptimized component2 = GetComponent<BloomOptimized>();
		component2.enabled = flag2 || !flag || isBloomForced || currentMapZoneEnum == MapZone.MEMORY || (currentMapZoneEnum == MapZone.CRADLE && sceneNameString != "Tube_Hub") || currentMapZoneEnum == MapZone.CLOVER || _forceBloomScenes.Contains(sceneNameString);
		component2.BlurIterations = ((!flag2) ? 1 : component2.InitialIterations);
		GetComponent<BrightnessEffect>().enabled = flag && flag2;
		ColorCorrectionCurves component3 = GetComponent<ColorCorrectionCurves>();
		component3.enabled = true;
		component3.IsBloomActive = component2.enabled;
	}

	private void LateUpdate()
	{
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		Vector3 position = base.transform.position;
		float x = position.x;
		float y = position.y;
		float z = position.z;
		Vector3 position2 = cameraParent.position;
		float x2 = position2.x;
		float y2 = position2.y;
		Vector3 position3 = camTarget.transform.position;
		if (isGameplayScene && mode != 0)
		{
			if (hero_ctrl.cState.lookingUp || hero_ctrl.cState.lookingUpRing)
			{
				if (camTarget.mode != CameraTarget.TargetMode.FREE)
				{
					dampTimeYSlowTimer = lookSlowTime;
					lookOffset = hero_ctrl.transform.position.y - position3.y + 6f;
				}
			}
			else if (hero_ctrl.cState.lookingDown || hero_ctrl.cState.lookingDownRing)
			{
				if (camTarget.mode != CameraTarget.TargetMode.FREE)
				{
					dampTimeYSlowTimer = lookSlowTime;
					lookOffset = hero_ctrl.transform.position.y - position3.y - 6f;
				}
			}
			else
			{
				lookOffset = 0f;
			}
			UpdateTargetDestinationDelta();
			Vector3 vector = cam.WorldToViewportPoint(position3);
			Vector3 vector2 = new Vector3(targetDeltaX, targetDeltaY, 0f) - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, vector.z));
			destination = new Vector3(x + vector2.x, y + vector2.y, z);
			if (mode == CameraMode.LOCKED && currentLockArea != null)
			{
				if (lookOffset > Mathf.Epsilon)
				{
					if (currentLockArea.preventLookUp && destination.y > currentLockArea.lookYMax)
					{
						if (position.y > currentLockArea.lookYMax)
						{
							destination = new Vector3(destination.x, destination.y - lookOffset, destination.z);
						}
						else
						{
							destination = new Vector3(destination.x, currentLockArea.lookYMax, destination.z);
						}
					}
				}
				else if (lookOffset < 0f - Mathf.Epsilon && currentLockArea.preventLookDown && destination.y < currentLockArea.lookYMin)
				{
					if (position.y < currentLockArea.lookYMin)
					{
						destination = new Vector3(destination.x, destination.y - lookOffset, destination.z);
					}
					else
					{
						destination = new Vector3(destination.x, currentLockArea.lookYMin, destination.z);
					}
				}
			}
			if (mode == CameraMode.FOLLOWING || mode == CameraMode.LOCKED)
			{
				destination = KeepWithinSceneBounds(destination);
			}
			position.x = Vector3.SmoothDamp(position, new Vector3(destination.x, y, z), ref velocityX, dampTimeX).x;
			if (dampTimeYSlowTimer > 0f)
			{
				position.y = Vector3.SmoothDamp(position, new Vector3(x, destination.y, z), ref velocityY, 0.15f).y;
				dampTimeYSlowTimer -= Time.deltaTime;
			}
			else if (isRising)
			{
				dampTimeRising -= Time.deltaTime / 10f;
				if (dampTimeRising < 0.03f)
				{
					dampTimeRising = 0.03f;
				}
				position.y = Vector3.SmoothDamp(position, new Vector3(x, destination.y, z), ref velocityY, dampTimeRising).y;
			}
			else if (isFalling)
			{
				dampTimeFalling -= Time.deltaTime / 10f;
				if (dampTimeFalling < 0.03f)
				{
					dampTimeFalling = 0.03f;
				}
				position.y = Vector3.SmoothDamp(position, new Vector3(x, destination.y, z), ref velocityY, dampTimeFalling).y;
			}
			else
			{
				position.y = Vector3.SmoothDamp(position, new Vector3(x, destination.y, z), ref velocityY, dampTimeY).y;
				if (dampTimeFalling != dampTimeY)
				{
					dampTimeFalling = dampTimeY;
				}
			}
			base.transform.SetPosition2D(position);
			x = position.x;
			y = position.y;
			if (velocity.magnitude > maxVelocityCurrent)
			{
				velocity = velocity.normalized * maxVelocityCurrent;
			}
		}
		if (!isGameplayScene)
		{
			return;
		}
		if (!AllowExitingSceneBounds)
		{
			if (x + x2 < 14.6f)
			{
				position.x = 14.6f;
			}
			if (position.x + x2 > xLimit)
			{
				position.x = xLimit;
			}
			if (y + y2 < 8.3f)
			{
				position.y = 8.3f;
			}
			if (position.y + y2 > yLimit)
			{
				position.y = yLimit;
			}
		}
		base.transform.SetPosition2D(position);
		if (startLockedTimer > 0f)
		{
			startLockedTimer -= Time.deltaTime;
			if (startLockedTimer <= 0f)
			{
				instantLockedArea.Clear();
			}
		}
	}

	private void OnDisable()
	{
		if (hero_ctrl != null)
		{
			hero_ctrl.heroInPosition -= PositionToHero;
		}
		if (lastPositioner == this)
		{
			IsPositioningCamera = false;
		}
	}

	public void FreezeInPlace(bool freezeTarget = false)
	{
		SetMode(CameraMode.FROZEN);
		if (freezeTarget)
		{
			camTarget.FreezeInPlace();
		}
	}

	public void StopFreeze(bool stopFreezeTarget = false)
	{
		SetMode(CameraMode.FOLLOWING);
		if (stopFreezeTarget)
		{
			camTarget.EndFreeMode();
		}
	}

	public void FadeOut(CameraFadeType type)
	{
		SetMode(CameraMode.FROZEN);
		switch (type)
		{
		case CameraFadeType.LEVEL_TRANSITION:
			fadeFSM.SendEventSafe("SCENE FADE OUT");
			break;
		case CameraFadeType.HERO_DEATH:
			fadeFSM.SendEventSafe("DEATH RESPAWN");
			break;
		case CameraFadeType.HERO_HAZARD_DEATH:
			fadeFSM.SendEventSafe("HAZARD FADE");
			break;
		case CameraFadeType.JUST_FADE:
			fadeFSM.SendEventSafe("SCENE FADE OUT");
			break;
		case CameraFadeType.START_FADE:
			fadeFSM.SendEventSafe("START FADE");
			break;
		case CameraFadeType.TO_MENU:
			fadeFSM.SendEventSafe("SCENE FADE TO MENU");
			break;
		}
	}

	public void FadeSceneIn()
	{
	}

	public void LockToArea(CameraLockArea lockArea)
	{
		if (lockZoneList.Contains(lockArea) && !(lockArea == currentLockArea))
		{
			return;
		}
		if (verboseMode)
		{
			Debug.LogFormat("LockZone Activated: {0} at startLockedTimer {1} ({2}s)", lockArea.name, startLockedTimer, Time.timeSinceLevelLoad);
		}
		if (lockArea != currentLockArea)
		{
			lockZoneList.Add(lockArea);
		}
		if (!(currentLockArea != null) || currentLockArea.priority <= lockArea.priority)
		{
			currentLockArea = lockArea;
			if (mode != 0)
			{
				SetMode(CameraMode.LOCKED);
			}
			xLockMin = ((lockArea.cameraXMin < 14.6f) ? 14.6f : lockArea.cameraXMin);
			if (lockArea.cameraXMax < 0f || lockArea.cameraXMax > xLimit)
			{
				xLockMax = xLimit;
			}
			else
			{
				xLockMax = lockArea.cameraXMax;
			}
			yLockMin = ((lockArea.cameraYMin < 8.3f) ? 8.3f : lockArea.cameraYMin);
			if (lockArea.cameraYMax < 0f || lockArea.cameraYMax > yLimit)
			{
				yLockMax = yLimit;
			}
			else
			{
				yLockMax = lockArea.cameraYMax;
			}
			if (startLockedTimer > 0f && (hero_ctrl.transitionState != 0 || instantLockedArea.Contains(lockArea)))
			{
				Vector3 position = hero_ctrl.transform.position;
				position.x += camTarget.xOffset;
				camTarget.transform.SetPosition2D(KeepWithinLockBounds(position));
				camTarget.destination = camTarget.transform.position;
				camTarget.EnterLockZoneInstant(xLockMin, xLockMax, yLockMin, yLockMax);
				base.transform.SetPosition2D(KeepWithinLockBounds(position));
				destination = base.transform.position;
				instantLockedArea.Add(lockArea);
				lockArea.OnDestroyEvent += OnLockAreaDestroyed;
			}
			else
			{
				camTarget.EnterLockZone(xLockMin, xLockMax, yLockMin, yLockMax);
			}
		}
	}

	public void ReleaseLock(CameraLockArea lockarea)
	{
		if (this == null)
		{
			return;
		}
		lockZoneList.Remove(lockarea);
		if (verboseMode)
		{
			Debug.Log("LockZone Released " + lockarea.name);
		}
		if (lockarea == currentLockArea)
		{
			if (lockZoneList.Count > 0)
			{
				int num = int.MinValue;
				for (int num2 = lockZoneList.Count - 1; num2 >= 0; num2--)
				{
					CameraLockArea cameraLockArea = lockZoneList[num2];
					if (cameraLockArea.priority > num)
					{
						num = cameraLockArea.priority;
						currentLockArea = cameraLockArea;
					}
				}
				xLockMin = currentLockArea.cameraXMin;
				xLockMax = currentLockArea.cameraXMax;
				yLockMin = currentLockArea.cameraYMin;
				yLockMax = currentLockArea.cameraYMax;
				xLockMin = ((currentLockArea.cameraXMin < 14.6f) ? 14.6f : currentLockArea.cameraXMin);
				if (currentLockArea.cameraXMax < 0f || currentLockArea.cameraXMax > xLimit)
				{
					xLockMax = xLimit;
				}
				else
				{
					xLockMax = currentLockArea.cameraXMax;
				}
				yLockMin = ((currentLockArea.cameraYMin < 8.3f) ? 8.3f : currentLockArea.cameraYMin);
				if (currentLockArea.cameraYMax < 0f || currentLockArea.cameraYMax > yLimit)
				{
					yLockMax = yLimit;
				}
				else
				{
					yLockMax = currentLockArea.cameraYMax;
				}
				camTarget.enteredFromLockZone = true;
				camTarget.EnterLockZone(xLockMin, xLockMax, yLockMin, yLockMax);
				if (startLockedTimer > 0f && hero_ctrl.transitionState != 0)
				{
					Vector3 position = hero_ctrl.transform.position;
					position.x += camTarget.xOffset;
					camTarget.transform.SetPosition2D(KeepWithinLockBounds(position));
					camTarget.destination = camTarget.transform.position;
					camTarget.EnterLockZoneInstant(xLockMin, xLockMax, yLockMin, yLockMax);
					base.transform.SetPosition2D(KeepWithinLockBounds(position));
					destination = base.transform.position;
				}
			}
			else
			{
				lastLockPosition = base.transform.position;
				if (camTarget != null)
				{
					camTarget.enteredFromLockZone = false;
					camTarget.ExitLockZone();
				}
				currentLockArea = null;
				if (!hero_ctrl.cState.hazardDeath && !hero_ctrl.cState.dead && gm.GameState != GameState.EXITING_LEVEL && mode != 0)
				{
					SetMode(CameraMode.FOLLOWING);
				}
				if (startLockedTimer > 0f)
				{
					Vector3 position2 = hero_ctrl.transform.position;
					position2.x += camTarget.xOffset;
					camTarget.transform.SetPosition2D(KeepWithinLockBounds(position2));
					camTarget.destination = camTarget.transform.position;
					camTarget.EnterLockZoneInstant(xLockMin, xLockMax, yLockMin, yLockMax);
					base.transform.SetPosition2D(KeepWithinLockBounds(position2));
					destination = base.transform.position;
				}
			}
		}
		else if (verboseMode)
		{
			Debug.Log("LockZone was not the current lock when removed.");
		}
	}

	public void ResetStartTimer()
	{
		startLockedTimer = 0.65f;
	}

	public void SnapTo(float x, float y)
	{
		Transform transform = camTarget.transform;
		transform.position = new Vector3(x, y, transform.position.z);
		Transform transform2 = base.transform;
		transform2.position = new Vector3(x, y, transform2.position.z);
	}

	public void SnapToY(float y)
	{
		Transform obj = camTarget.transform;
		Vector3 position = obj.position;
		position = new Vector3(position.x, y, position.z);
		obj.position = position;
		Transform obj2 = base.transform;
		Vector3 position2 = obj2.position;
		position2 = new Vector3(position2.x, y, position2.z);
		obj2.position = position2;
	}

	public void SnapTargetToY(float y)
	{
		Transform obj = camTarget.transform;
		Vector3 position = obj.position;
		position = new Vector3(position.x, y, position.z);
		obj.position = position;
	}

	private void OnLockAreaDestroyed(CameraLockArea lockArea)
	{
		instantLockedArea.Remove(lockArea);
		lockArea.OnDestroyEvent -= OnLockAreaDestroyed;
	}

	private void UpdateTargetDestinationDelta()
	{
		targetDeltaX = camTarget.transform.position.x;
		targetDeltaY = camTarget.transform.position.y + lookOffset;
	}

	public void SetMode(CameraMode newMode)
	{
		if (newMode != mode)
		{
			if (newMode == CameraMode.PREVIOUS)
			{
				mode = prevMode;
				return;
			}
			prevMode = mode;
			mode = newMode;
		}
	}

	public Vector3 KeepWithinSceneBounds(Vector3 targetDest)
	{
		Vector3 result = targetDest;
		bool flag = false;
		bool flag2 = false;
		if (!AllowExitingSceneBounds)
		{
			if (result.x < 14.6f)
			{
				result = new Vector3(14.6f, result.y, result.z);
				flag = true;
				flag2 = true;
			}
			if (result.x > xLimit)
			{
				result = new Vector3(xLimit, result.y, result.z);
				flag = true;
				flag2 = true;
			}
			if (result.y < 8.3f)
			{
				result = new Vector3(result.x, 8.3f, result.z);
				flag = true;
			}
			if (result.y > yLimit)
			{
				result = new Vector3(result.x, yLimit, result.z);
				flag = true;
			}
		}
		atSceneBounds = flag;
		atHorizontalSceneBounds = flag2;
		return result;
	}

	private Vector2 KeepWithinSceneBounds(Vector2 targetDest)
	{
		bool flag = false;
		if (targetDest.x < 14.6f)
		{
			targetDest = new Vector2(14.6f, targetDest.y);
			flag = true;
		}
		if (targetDest.x > xLimit)
		{
			targetDest = new Vector2(xLimit, targetDest.y);
			flag = true;
		}
		if (targetDest.y < 8.3f)
		{
			targetDest = new Vector2(targetDest.x, 8.3f);
			flag = true;
		}
		if (targetDest.y > yLimit)
		{
			targetDest = new Vector2(targetDest.x, yLimit);
			flag = true;
		}
		atSceneBounds = flag;
		return targetDest;
	}

	private bool IsAtSceneBounds(Vector2 targetDest)
	{
		bool result = false;
		if (targetDest.x <= 14.6f)
		{
			result = true;
		}
		if (targetDest.x >= xLimit)
		{
			result = true;
		}
		if (targetDest.y <= 8.3f)
		{
			result = true;
		}
		if (targetDest.y >= yLimit)
		{
			result = true;
		}
		return result;
	}

	private bool IsAtHorizontalSceneBounds(Vector2 targetDest, out bool leftSide)
	{
		bool result = false;
		leftSide = false;
		if (targetDest.x <= 14.6f)
		{
			result = true;
			leftSide = true;
		}
		if (targetDest.x >= xLimit)
		{
			result = true;
			leftSide = false;
		}
		return result;
	}

	private bool IsTouchingSides(float x)
	{
		bool result = false;
		if (x <= 14.6f)
		{
			result = true;
		}
		if (x >= xLimit)
		{
			result = true;
		}
		return result;
	}

	public Vector2 KeepWithinLockBounds(Vector2 targetDest)
	{
		float x = targetDest.x;
		float y = targetDest.y;
		if (x < xLockMin)
		{
			x = xLockMin;
		}
		if (x > xLockMax)
		{
			x = xLockMax;
		}
		if (y < yLockMin)
		{
			y = yLockMin;
		}
		if (y > yLockMax)
		{
			y = yLockMax;
		}
		return new Vector2(x, y);
	}

	private void GetTilemapInfo()
	{
		tilemap = gm.tilemap;
		if ((bool)tilemap)
		{
			sceneWidth = tilemap.width;
			sceneHeight = tilemap.height;
			xLimit = sceneWidth - 14.6f;
			yLimit = sceneHeight - 8.3f;
		}
	}

	public void SetAllowExitingSceneBounds(bool value)
	{
		AllowExitingSceneBounds = value;
	}

	public void ResetPositionedAtHero()
	{
		lastInPositionScene = string.Empty;
	}

	public void PositionToHero(bool forceDirect)
	{
		if (positionToHeroCoroutine != null)
		{
			StopCoroutine(positionToHeroCoroutine);
		}
		lastPositioner = this;
		IsPositioningCamera = true;
		positionToHeroCoroutine = StartCoroutine(DoPositionToHero(forceDirect));
	}

	public void PositionToHeroInstant(bool forceDirect)
	{
		lastPositioner = this;
		DoPositionToHeroInstant(forceDirect);
	}

	private IEnumerator DoPositionToHero(bool forceDirect)
	{
		yield return new WaitForFixedUpdate();
		GetTilemapInfo();
		camTarget.PositionToStart();
		UpdateTargetDestinationDelta();
		CameraMode previousMode = mode;
		SetMode(CameraMode.FROZEN);
		Vector3 newPosition = KeepWithinSceneBounds(camTarget.transform.position);
		if (verboseMode)
		{
			Debug.LogFormat("CC - STR: NewPosition: {0} TargetDelta: ({1}, {2}) CT-XOffset: {3} HeroPos: {4} CT-Pos: {5}", newPosition, targetDeltaX, targetDeltaY, camTarget.xOffset, hero_ctrl.transform.position, camTarget.transform.position);
		}
		if (forceDirect)
		{
			if (verboseMode)
			{
				Debug.Log("====> TEST 1a - ForceDirect Positioning Mode");
			}
			base.transform.SetPosition2D(newPosition);
		}
		else
		{
			if (verboseMode)
			{
				Debug.Log("====> TEST 1b - Normal Positioning Mode");
			}
			bool leftSide;
			bool num = IsAtHorizontalSceneBounds(newPosition, out leftSide);
			if (currentLockArea != null)
			{
				if (verboseMode)
				{
					Debug.Log("====> TEST 3 - Lock Zone Active");
				}
				base.transform.SetPosition2D(KeepWithinLockBounds(newPosition));
			}
			else
			{
				if (verboseMode)
				{
					Debug.Log("====> TEST 4 - No Lock Zone");
				}
				base.transform.SetPosition2D(newPosition);
			}
			if (num)
			{
				if (verboseMode)
				{
					Debug.Log("====> TEST 2 - At Horizontal Scene Bounds");
				}
				if ((leftSide && !hero_ctrl.cState.facingRight) || (!leftSide && hero_ctrl.cState.facingRight))
				{
					if (verboseMode)
					{
						Debug.Log("====> TEST 2a - Hero Facing Bounds");
					}
					base.transform.SetPosition2D(newPosition);
				}
				else
				{
					if (verboseMode)
					{
						Debug.Log("====> TEST 2b - Hero Facing Inwards");
					}
					if (IsTouchingSides(targetDeltaX))
					{
						if (verboseMode)
						{
							Debug.Log("Xoffset still touching sides");
						}
						base.transform.SetPosition2D(newPosition);
					}
					else
					{
						if (verboseMode)
						{
							Debug.LogFormat("Not Touching Sides with Xoffset CT: {0} Hero: {1}", camTarget.transform.position, hero_ctrl.transform.position);
						}
						if (hero_ctrl.cState.facingRight)
						{
							base.transform.SetPosition2D(hero_ctrl.transform.position.x + 1f, newPosition.y);
						}
						else
						{
							base.transform.SetPosition2D(hero_ctrl.transform.position.x - 1f, newPosition.y);
						}
					}
				}
			}
		}
		destination = base.transform.position;
		velocity = Vector3.zero;
		velocityX = Vector3.zero;
		velocityY = Vector3.zero;
		yield return new WaitForSeconds(0.1f);
		switch (previousMode)
		{
		case CameraMode.FROZEN:
			SetMode(CameraMode.FOLLOWING);
			if (currentLockArea != null)
			{
				LockToArea(currentLockArea);
			}
			break;
		case CameraMode.LOCKED:
			if (currentLockArea != null)
			{
				SetMode(previousMode);
			}
			else
			{
				SetMode(CameraMode.FOLLOWING);
			}
			break;
		default:
			SetMode(previousMode);
			break;
		}
		if (verboseMode)
		{
			Debug.LogFormat("CC - PositionToHero FIN: - TargetDelta: ({0}, {1}) Destination: {2} CT-XOffset: {3} NewPosition: {4} CamTargetPos: {5} HeroPos: {6}", targetDeltaX, targetDeltaY, destination, camTarget.xOffset, newPosition, camTarget.transform.position, hero_ctrl.transform.position);
		}
		if (gm != null)
		{
			lastInPositionScene = gm.sceneName;
		}
		IsPositioningCamera = false;
		if (this.PositionedAtHero != null)
		{
			this.PositionedAtHero();
		}
		positionToHeroCoroutine = null;
	}

	private void DoPositionToHeroInstant(bool forceDirect)
	{
		GetTilemapInfo();
		camTarget.PositionToStart();
		UpdateTargetDestinationDelta();
		CameraMode cameraMode = mode;
		SetMode(CameraMode.FROZEN);
		Vector3 position = camTarget.transform.position;
		camTarget.Update();
		Vector3 vector = KeepWithinSceneBounds(position);
		if (verboseMode)
		{
			Debug.LogFormat("CC - STR: NewPosition: {0} TargetDelta: ({1}, {2}) CT-XOffset: {3} HeroPos: {4} CT-Pos: {5}", vector, targetDeltaX, targetDeltaY, camTarget.xOffset, hero_ctrl.transform.position, camTarget.transform.position);
		}
		if (forceDirect)
		{
			if (verboseMode)
			{
				Debug.Log("====> TEST 1a - ForceDirect Positioning Mode");
			}
			base.transform.SetPosition2D(vector);
		}
		else
		{
			if (verboseMode)
			{
				Debug.Log("====> TEST 1b - Normal Positioning Mode");
			}
			bool leftSide;
			bool num = IsAtHorizontalSceneBounds(vector, out leftSide);
			if (currentLockArea != null)
			{
				if (verboseMode)
				{
					Debug.Log("====> TEST 3 - Lock Zone Active");
				}
				base.transform.SetPosition2D(KeepWithinLockBounds(vector));
			}
			else
			{
				if (verboseMode)
				{
					Debug.Log("====> TEST 4 - No Lock Zone");
				}
				base.transform.SetPosition2D(vector);
			}
			if (num)
			{
				if (verboseMode)
				{
					Debug.Log("====> TEST 2 - At Horizontal Scene Bounds");
				}
				if ((leftSide && !hero_ctrl.cState.facingRight) || (!leftSide && hero_ctrl.cState.facingRight))
				{
					if (verboseMode)
					{
						Debug.Log("====> TEST 2a - Hero Facing Bounds");
					}
					base.transform.SetPosition2D(vector);
				}
				else
				{
					if (verboseMode)
					{
						Debug.Log("====> TEST 2b - Hero Facing Inwards");
					}
					if (IsTouchingSides(targetDeltaX))
					{
						if (verboseMode)
						{
							Debug.Log("Xoffset still touching sides");
						}
						base.transform.SetPosition2D(vector);
					}
					else
					{
						if (verboseMode)
						{
							Debug.LogFormat("Not Touching Sides with Xoffset CT: {0} Hero: {1}", camTarget.transform.position, hero_ctrl.transform.position);
						}
						if (hero_ctrl.cState.facingRight)
						{
							base.transform.SetPosition2D(hero_ctrl.transform.position.x + 1f, vector.y);
						}
						else
						{
							base.transform.SetPosition2D(hero_ctrl.transform.position.x - 1f, vector.y);
						}
					}
				}
			}
		}
		destination = base.transform.position;
		velocity = Vector3.zero;
		velocityX = Vector3.zero;
		velocityY = Vector3.zero;
		switch (cameraMode)
		{
		case CameraMode.FROZEN:
			SetMode(CameraMode.FOLLOWING);
			if (currentLockArea != null)
			{
				LockToArea(currentLockArea);
			}
			break;
		case CameraMode.LOCKED:
			if (currentLockArea != null)
			{
				SetMode(cameraMode);
			}
			else
			{
				SetMode(CameraMode.FOLLOWING);
			}
			break;
		default:
			SetMode(cameraMode);
			break;
		}
		if (verboseMode)
		{
			Debug.LogFormat("CC - PositionToHero FIN: - TargetDelta: ({0}, {1}) Destination: {2} CT-XOffset: {3} NewPosition: {4} CamTargetPos: {5} HeroPos: {6}", targetDeltaX, targetDeltaY, destination, camTarget.xOffset, vector, camTarget.transform.position, hero_ctrl.transform.position);
		}
		if (gm != null)
		{
			lastInPositionScene = gm.sceneName;
		}
		if (!IsPositioningCamera && this.PositionedAtHero != null)
		{
			this.PositionedAtHero();
		}
	}

	private IEnumerator FadeInFailSafe()
	{
		yield return new WaitForSeconds(5f);
		if (fadeFSM.ActiveStateName != "Normal" && fadeFSM.ActiveStateName != "FadingOut")
		{
			Debug.LogFormat("Failsafe fade in activated. State: {0} Scene: {1}", fadeFSM.ActiveStateName, gm.sceneName);
			fadeFSM.Fsm.Event("FADE SCENE IN");
		}
	}

	private void StopFailSafe()
	{
		if (fadeInFailSafeCo != null)
		{
			StopCoroutine(fadeInFailSafeCo);
		}
	}

	private void OnLevelUnload()
	{
		AudioGroupManager.ClearAudioGroups();
		if (!(this == null))
		{
			if (verboseMode)
			{
				Debug.Log("Removing cam locks. (" + lockZoneList.Count + " total)");
			}
			while (lockZoneList.Count > 0)
			{
				ReleaseLock(lockZoneList[0]);
			}
		}
	}

	private void OnDestroy()
	{
		if (gm != null)
		{
			gm.UnloadingLevel -= OnLevelUnload;
			gm.OnFinishedEnteringScene -= OnFinishedEnteringScene;
		}
		if (lastPositioner == this)
		{
			lastPositioner = null;
		}
	}

	public void ScreenFlash(Color colour)
	{
		screenFlash.gameObject.SetActive(value: true);
		screenFlash.SetColor(colour);
	}

	public void ScreenFlashLifeblood()
	{
		ScreenFlash(flash_lifeblood);
	}

	public void ScreenFlashPoison()
	{
		ScreenFlash(flash_poison);
	}

	public void ScreenFlashBomb()
	{
		ScreenFlash(flash_bomb);
	}

	public void ScreenFlashTrobbio()
	{
		ScreenFlash(flash_trobbio);
	}

	public void ScreenFlashFrostStart()
	{
		ScreenFlash(flash_frostStart);
	}

	public void ScreenFlashFrostDamage()
	{
		ScreenFlash(flash_frostDamage);
	}

	public void ScreenFlashPerfectDash()
	{
		ScreenFlash(flash_perfectDash);
	}
}
