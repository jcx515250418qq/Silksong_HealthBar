using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraLockArea : TrackTriggerObjects
{
	public float cameraXMin = -1f;

	public float cameraYMin = -1f;

	public float cameraXMax = -1f;

	public float cameraYMax = -1f;

	public Space positionSpace;

	[Space]
	public bool preventLookUp;

	[ModifiableProperty]
	[Conditional("preventLookUp", true, false, false)]
	public float lookYMax = -1f;

	public bool preventLookDown;

	[ModifiableProperty]
	[Conditional("preventLookDown", true, false, false)]
	public float lookYMin = -1f;

	[Space]
	public int priority;

	[Obsolete]
	[SerializeField]
	[HideInInspector]
	private bool maxPriority;

	private float leftSideX;

	private float rightSideX;

	private float topSideY;

	private float botSideY;

	private Vector3 heroPos;

	private bool enteredLeft;

	private bool enteredRight;

	private bool enteredTop;

	private bool enteredBot;

	private bool exitedLeft;

	private bool exitedRight;

	private bool exitedTop;

	private bool exitedBot;

	private bool hasStarted;

	private GameCameras gcams;

	private CameraController cameraCtrl;

	private CameraTarget camTarget;

	private Collider2D box2d;

	public event Action<CameraLockArea> OnDestroyEvent;

	private void OnValidate()
	{
		if (maxPriority)
		{
			maxPriority = false;
			priority = 1;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		box2d = GetComponent<Collider2D>();
		gcams = GameCameras.instance;
		cameraCtrl = gcams.cameraController;
		camTarget = gcams.cameraTarget;
	}

	protected override void OnEnable()
	{
		if (positionSpace == Space.Self)
		{
			ValidateBounds();
		}
		base.OnEnable();
		if (!hasStarted)
		{
			hasStarted = true;
			StartCoroutine(StartRoutine());
		}
	}

	private void OnDestroy()
	{
		this.OnDestroyEvent?.Invoke(this);
		this.OnDestroyEvent = null;
	}

	private IEnumerator StartRoutine()
	{
		Scene scene = base.gameObject.scene;
		while (cameraCtrl.tilemap == null || cameraCtrl.tilemap.gameObject.scene != scene)
		{
			yield return null;
		}
		ValidateBounds();
		if (box2d != null)
		{
			Bounds bounds = box2d.bounds;
			leftSideX = bounds.min.x;
			rightSideX = bounds.max.x;
			botSideY = bounds.min.y;
			topSideY = bounds.max.y;
		}
	}

	public static bool IsInApplicableGameState()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance == null)
		{
			return false;
		}
		if (unsafeInstance.GameState != GameState.PLAYING && unsafeInstance.GameState != GameState.ENTERING_LEVEL)
		{
			return unsafeInstance.GameState == GameState.CUTSCENE;
		}
		return true;
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		heroPos = silentInstance.transform.position;
		if (isInside)
		{
			if (!IsInApplicableGameState())
			{
				return;
			}
			if (box2d != null)
			{
				if (heroPos.x > leftSideX - 1f && heroPos.x < leftSideX + 1f)
				{
					camTarget.enteredLeft = true;
				}
				else
				{
					camTarget.enteredLeft = false;
				}
				if (heroPos.x > rightSideX - 1f && heroPos.x < rightSideX + 1f)
				{
					camTarget.enteredRight = true;
				}
				else
				{
					camTarget.enteredRight = false;
				}
				if (heroPos.y > topSideY - 2f && heroPos.y < topSideY + 2f)
				{
					camTarget.enteredTop = true;
				}
				else
				{
					camTarget.enteredTop = false;
				}
				if (heroPos.y > botSideY - 1f && heroPos.y < botSideY + 1f)
				{
					camTarget.enteredBot = true;
				}
				else
				{
					camTarget.enteredBot = false;
				}
			}
			cameraCtrl.LockToArea(this);
			return;
		}
		if (box2d != null)
		{
			if (heroPos.x > leftSideX - 1f && heroPos.x < leftSideX + 1f)
			{
				camTarget.exitedLeft = true;
			}
			else
			{
				camTarget.exitedLeft = false;
			}
			if (heroPos.x > rightSideX - 1f && heroPos.x < rightSideX + 1f)
			{
				camTarget.exitedRight = true;
			}
			else
			{
				camTarget.exitedRight = false;
			}
			if (heroPos.y > topSideY - 2f && heroPos.y < topSideY + 2f)
			{
				camTarget.exitedTop = true;
			}
			else
			{
				camTarget.exitedTop = false;
			}
			if (heroPos.y > botSideY - 1f && heroPos.y < botSideY + 1f)
			{
				camTarget.exitedBot = true;
			}
			else
			{
				camTarget.exitedBot = false;
			}
		}
		cameraCtrl.ReleaseLock(this);
	}

	private bool ValidateBounds()
	{
		GetWorldBounds(out cameraXMin, out cameraYMin, out cameraXMax, out cameraYMax, out lookYMin, out lookYMax);
		positionSpace = Space.World;
		if (cameraXMin < 0f)
		{
			cameraXMin = 14.6f;
		}
		if (cameraXMax < 0f)
		{
			cameraXMax = cameraCtrl.xLimit;
		}
		if (cameraYMin < 0f)
		{
			cameraYMin = 8.3f;
		}
		if (cameraYMax < 0f)
		{
			cameraYMax = cameraCtrl.yLimit;
		}
		if (lookYMin < 0f)
		{
			lookYMin = cameraYMin;
		}
		if (lookYMax < 0f)
		{
			lookYMax = cameraYMax;
		}
		if (Math.Abs(cameraXMin) <= Mathf.Epsilon && Math.Abs(cameraXMax) <= Mathf.Epsilon && Math.Abs(cameraYMin) <= Mathf.Epsilon && Math.Abs(cameraYMax) <= Mathf.Epsilon)
		{
			return false;
		}
		return true;
	}

	public void SetXMin(float xMin)
	{
		cameraXMin = xMin;
		DidResetBounds();
	}

	public void SetXMax(float xMax)
	{
		cameraXMax = xMax;
		DidResetBounds();
	}

	public void SetYMin(float yMin)
	{
		cameraYMin = yMin;
		DidResetBounds();
	}

	public void SetYMax(float yMax)
	{
		cameraYMax = yMax;
		DidResetBounds();
	}

	private void DidResetBounds()
	{
	}

	public void GetWorldBounds(out float outCameraXMin, out float outCameraYMin, out float outCameraXMax, out float outCameraYMax, out float outLookYMin, out float outLookYMax)
	{
		if (positionSpace == Space.World)
		{
			outCameraXMin = cameraXMin;
			outCameraYMin = cameraYMin;
			outCameraXMax = cameraXMax;
			outCameraYMax = cameraYMax;
			outLookYMin = lookYMin;
			outLookYMax = lookYMax;
		}
		else
		{
			Vector2 vector = new Vector2(cameraXMin, cameraYMin);
			Vector2 vector2 = new Vector2(cameraXMax, cameraYMax);
			Vector2 vector3 = base.transform.position;
			vector += vector3;
			vector2 += vector3;
			outCameraXMin = vector.x;
			outCameraYMin = vector.y;
			outCameraXMax = vector2.x;
			outCameraYMax = vector2.y;
			outLookYMin = lookYMin + vector3.y;
			outLookYMax = lookYMax + vector3.y;
		}
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.CameraLock);
	}
}
