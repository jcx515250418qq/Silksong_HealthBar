using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.SharedUtils;
using TeamCherry.Splines;
using UnityEngine;
using UnityEngine.Events;

public class LiftControl : MonoBehaviour, HeroPlatformStick.IMoveHooks
{
	[Serializable]
	private class LiftStop
	{
		public float PosY;

		public bool OpenDoorLeft;

		public bool OpenDoorRight;

		[Space]
		public List<TempPressurePlate> CallPlates;

		public bool CallPlateUnlocks;

		[HideInInspector]
		[Obsolete]
		public TempPressurePlate CallPlate;

		public string[] EntryGatesStartAt;

		public OverrideFloat CallTeleportPoint;
	}

	private static readonly int _doorOpenAnim = Animator.StringToHash("Open");

	private static readonly int _doorCloseAnim = Animator.StringToHash("Close");

	[Header("Structure")]
	[SerializeField]
	private Animator doorLeft;

	[SerializeField]
	private Animator doorRight;

	[Space]
	[SerializeField]
	private PersistentBoolItem persistentUnlocked;

	[SerializeField]
	private PersistentIntItem persistentStopState;

	[SerializeField]
	private ItemReceptacle unlockReceptacle;

	[SerializeField]
	private bool unlockReceptacleIsInside;

	[Space]
	[SerializeField]
	private TriggerEnterEvent doorCloseTrigger;

	[SerializeField]
	private SimpleButton doorCloseButton;

	[SerializeField]
	private TriggerEnterEvent buttonRaiseExitTrigger;

	[Space]
	[SerializeField]
	private CameraShakeTarget startMoveCamShake;

	[SerializeField]
	private float moveDelay = 1f;

	[SerializeField]
	private float moveSpeed = 10f;

	[SerializeField]
	private AnimationCurve speedCurve = AnimationCurve.Constant(1f, 1f, 1f);

	[SerializeField]
	private float speedCurveDuration;

	[SerializeField]
	private bool controlCamera;

	[SerializeField]
	private MinMaxFloat clampCameraY;

	[SerializeField]
	private float endDelay = 1f;

	[SerializeField]
	private float plateResetDistance;

	[SerializeField]
	private LiftPlatform bobPlat;

	[SerializeField]
	private bool bobSilent;

	[SerializeField]
	private AudioEvent activateSound;

	[SerializeField]
	private AudioEvent arriveSound;

	[SerializeField]
	private AudioEvent doorsOpenSound;

	[SerializeField]
	private AudioSource movingLoop;

	[SerializeField]
	private VibrationPlayer movingVibrationPlayer;

	[Space]
	public UnityEvent OnUnlock;

	public UnityEvent OnUnlocked;

	[Header("Scene Config")]
	[SerializeField]
	private LiftStop[] stops;

	[SerializeField]
	private int defaultStop;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private GameObject supportMechanism;

	[SerializeField]
	private List<GameObject> supportMechanisms;

	[SerializeField]
	private SplineBase[] chainSplines;

	[SerializeField]
	private float chainSplineOffsetSpeed;

	[SerializeField]
	private GameObject activeWhileMoving;

	[Space]
	public UnityEvent OnStartedMoving;

	public UnityEvent OnStoppedMoving;

	private bool isUnlocked;

	private int currentStop;

	private bool overridingDefaultStop;

	private TempPressurePlate justPressedPlate;

	private LoopRotator[] mechanismRotators;

	private KeepWorldPosition[] keepWorldPositions;

	private float chainSplineOffset;

	private Coroutine moveRoutine;

	private VibrationEmission moveLoopEmission;

	private bool isSilent;

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		if (stops == null)
		{
			return;
		}
		LiftStop[] array = stops;
		foreach (LiftStop liftStop in array)
		{
			float? y = liftStop.PosY;
			Vector3 vector = position.Where(null, y, null);
			Gizmos.DrawWireSphere(vector, 0.2f);
			foreach (TempPressurePlate callPlate in liftStop.CallPlates)
			{
				if ((bool)callPlate)
				{
					Gizmos.DrawLine(vector, callPlate.transform.position);
				}
			}
		}
	}

	private void OnValidate()
	{
		if (stops != null)
		{
			LiftStop[] array = stops;
			foreach (LiftStop liftStop in array)
			{
				if (!(liftStop.CallPlate == null))
				{
					liftStop.CallPlates.Add(liftStop.CallPlate);
					liftStop.CallPlate = null;
				}
			}
		}
		if (supportMechanism != null)
		{
			supportMechanisms.Add(supportMechanism);
			supportMechanism = null;
		}
	}

	private void Awake()
	{
		OnValidate();
		keepWorldPositions = GetComponentsInChildren<KeepWorldPosition>();
		bool flag = false;
		LiftStop[] array = stops;
		foreach (LiftStop liftStop in array)
		{
			for (int num = liftStop.CallPlates.Count - 1; num >= 0; num--)
			{
				if (liftStop.CallPlates[num] == null)
				{
					liftStop.CallPlates.RemoveAt(num);
				}
			}
			if (liftStop.CallPlateUnlocks)
			{
				flag = true;
			}
		}
		currentStop = defaultStop;
		if ((bool)unlockReceptacle)
		{
			unlockReceptacle.Unlocked += Unlock;
			unlockReceptacle.StartedUnlocked += delegate
			{
				SetUnlocked(value: true);
			};
		}
		else if (!flag)
		{
			isUnlocked = true;
		}
		else if ((bool)persistentUnlocked)
		{
			persistentUnlocked.OnGetSaveState += delegate(out bool value)
			{
				value = isUnlocked;
			};
			persistentUnlocked.OnSetSaveState += SetUnlocked;
		}
		if ((bool)persistentStopState)
		{
			persistentStopState.OnGetSaveState += delegate(out int value)
			{
				value = currentStop;
			};
			persistentStopState.OnSetSaveState += delegate(int value)
			{
				int num2 = stops.Length;
				if (num2 != 0)
				{
					if (!overridingDefaultStop)
					{
						currentStop = Mathf.Clamp(value, 0, num2 - 1);
					}
					SetInitialPos();
				}
			};
		}
		if ((bool)doorCloseTrigger)
		{
			doorCloseTrigger.OnTriggerEntered += delegate
			{
				MoveToNextStop();
			};
		}
		if ((bool)doorCloseButton)
		{
			SimpleButton simpleButton = doorCloseButton;
			simpleButton.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton.DepressedChange, (Action<bool>)delegate(bool value)
			{
				if (value)
				{
					MoveToNextStop();
				}
			});
		}
		if ((bool)buttonRaiseExitTrigger && (bool)doorCloseButton)
		{
			buttonRaiseExitTrigger.OnTriggerExited += delegate
			{
				if (isUnlocked)
				{
					doorCloseButton.SetLocked(value: false);
				}
			};
		}
		bool flag2 = !unlockReceptacle;
		for (int j = 0; j < stops.Length; j++)
		{
			LiftStop stop = stops[j];
			if (stop.CallPlateUnlocks && flag2)
			{
				foreach (TempPressurePlate callPlate in stop.CallPlates)
				{
					callPlate.Activated += Unlock;
				}
			}
			int stopIndex = j;
			foreach (TempPressurePlate callPlate2 in stop.CallPlates)
			{
				TempPressurePlate capturedPlate = callPlate2;
				callPlate2.PreActivated += delegate
				{
					justPressedPlate = capturedPlate;
					if ((bool)justPressedPlate)
					{
						foreach (TempPressurePlate callPlate3 in stops[stopIndex].CallPlates)
						{
							if (callPlate3 != justPressedPlate)
							{
								callPlate3.ActivateSilent();
							}
						}
					}
				};
				callPlate2.Activated += delegate
				{
					if (stopIndex != currentStop && stop.CallTeleportPoint.IsEnabled)
					{
						base.transform.SetPositionY(stop.CallTeleportPoint.Value);
					}
					MoveToStop(stopIndex, isControllingCamera: false);
				};
			}
		}
		mechanismRotators = supportMechanisms.Where((GameObject m) => m).SelectMany((GameObject m) => m.GetComponentsInChildren<LoopRotator>()).ToArray();
	}

	private void Start()
	{
		GameManager instance = GameManager.instance;
		PlayerData instance2 = PlayerData.instance;
		string text = instance.GetEntryGateName();
		if (string.IsNullOrEmpty(text))
		{
			string sceneNameString = instance.GetSceneNameString();
			if (instance2.respawnScene == sceneNameString)
			{
				text = instance2.respawnMarkerName;
			}
		}
		overridingDefaultStop = false;
		for (int i = 0; i < stops.Length; i++)
		{
			string[] entryGatesStartAt = stops[i].EntryGatesStartAt;
			foreach (string text2 in entryGatesStartAt)
			{
				if (!string.IsNullOrEmpty(text2) && !(text2 != text))
				{
					currentStop = i;
					overridingDefaultStop = true;
					break;
				}
			}
			if (overridingDefaultStop)
			{
				break;
			}
		}
		SetInitialPos();
		if ((bool)activeWhileMoving)
		{
			activeWhileMoving.SetActive(value: false);
		}
	}

	private void SetInitialPos()
	{
		LiftStop liftStop = stops[currentStop];
		base.transform.SetPositionY(liftStop.PosY);
		isSilent = true;
		if (isUnlocked)
		{
			SetOpenDoors(liftStop.OpenDoorLeft, liftStop.OpenDoorRight, isInstant: true);
			ResetPlates();
		}
		else if ((bool)unlockReceptacle)
		{
			if (unlockReceptacleIsInside)
			{
				SetOpenDoors(liftStop.OpenDoorLeft, liftStop.OpenDoorRight, isInstant: true);
			}
			else
			{
				SetOpenDoors(left: false, right: false, isInstant: true);
			}
			LiftStop[] array = stops;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (TempPressurePlate callPlate in array[i].CallPlates)
				{
					callPlate.ActivateSilent();
				}
			}
		}
		else
		{
			SetOpenDoors(left: false, right: false, isInstant: true);
			LiftStop[] array = stops;
			foreach (LiftStop liftStop2 in array)
			{
				if (liftStop2.CallPlateUnlocks)
				{
					continue;
				}
				foreach (TempPressurePlate callPlate2 in liftStop2.CallPlates)
				{
					callPlate2.ActivateSilent();
				}
			}
		}
		isSilent = false;
	}

	private void SetOpenDoors(bool left, bool right, bool isInstant, bool vibrate = true)
	{
		if (isInstant)
		{
			if ((bool)doorLeft)
			{
				doorLeft.Play(left ? _doorOpenAnim : _doorCloseAnim, 0, 1f);
			}
			if ((bool)doorRight)
			{
				doorRight.Play(right ? _doorOpenAnim : _doorCloseAnim, 0, 1f);
			}
			return;
		}
		if ((bool)doorLeft)
		{
			doorLeft.Play(left ? _doorOpenAnim : _doorCloseAnim);
		}
		if ((bool)doorRight)
		{
			doorRight.Play(right ? _doorOpenAnim : _doorCloseAnim);
		}
		doorsOpenSound.SpawnAndPlayOneShot(base.transform.position, vibrate);
	}

	public void MoveToNextStop()
	{
		if (moveRoutine == null)
		{
			int num = currentStop + 1;
			if (num >= stops.Length)
			{
				num = 0;
			}
			MoveToStop(num, isControllingCamera: true);
		}
	}

	public void StopMoving()
	{
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
			LoopRotator[] array = mechanismRotators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StopRotation();
			}
			if ((bool)movingLoop)
			{
				movingLoop.Stop();
			}
			if ((bool)movingVibrationPlayer)
			{
				movingVibrationPlayer.Stop();
			}
			if ((bool)activeWhileMoving)
			{
				activeWhileMoving.SetActive(value: false);
			}
		}
	}

	public void MoveToStop(int stopIndex, bool isControllingCamera)
	{
		if (!isUnlocked)
		{
			return;
		}
		if (moveRoutine != null)
		{
			if (stopIndex == currentStop)
			{
				return;
			}
			StopCoroutine(moveRoutine);
		}
		if (stopIndex == currentStop)
		{
			ResetPlates();
			LiftStop liftStop = stops[currentStop];
			SetOpenDoors(liftStop.OpenDoorLeft, liftStop.OpenDoorRight, isInstant: false);
		}
		else
		{
			currentStop = stopIndex;
			moveRoutine = StartCoroutine(MoveRoutine(isControllingCamera));
		}
	}

	private IEnumerator MoveRoutine(bool isControllingCamera)
	{
		LiftStop stop = stops[currentStop];
		bool vibrate = isControllingCamera;
		SetOpenDoors(left: false, right: false, isInstant: false, vibrate);
		activateSound.SpawnAndPlayOneShot(base.transform.position);
		startMoveCamShake.DoShake(this);
		if ((bool)activeWhileMoving)
		{
			activeWhileMoving.SetActive(value: true);
		}
		OnStartedMoving.Invoke();
		if ((bool)doorCloseButton)
		{
			doorCloseButton.SetLocked(value: true);
		}
		yield return new WaitForSeconds(moveDelay);
		if ((bool)bobPlat)
		{
			bobPlat.enabled = false;
		}
		if ((bool)movingLoop)
		{
			movingLoop.Play();
		}
		if (vibrate && (bool)movingVibrationPlayer)
		{
			movingVibrationPlayer.Play();
		}
		Vector2 startPos = base.transform.position;
		Vector2 original = startPos;
		float? y = stop.PosY;
		Vector2 targetPos = original.Where(null, y);
		float num = Vector2.Distance(targetPos, startPos);
		float duration = num / moveSpeed;
		float currentChainSpeed;
		LoopRotator[] array;
		if (targetPos.y > startPos.y)
		{
			array = mechanismRotators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StartRotationReversed();
			}
			currentChainSpeed = 0f - chainSplineOffsetSpeed;
		}
		else
		{
			array = mechanismRotators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StartRotation();
			}
			currentChainSpeed = chainSplineOffsetSpeed;
		}
		CameraController camCtrl = GameCameras.instance.cameraController;
		float camOffset = 0f;
		if (isControllingCamera)
		{
			if (controlCamera)
			{
				camCtrl.SetMode(CameraController.CameraMode.PANNING);
				camOffset = camCtrl.camTarget.transform.position.y - startPos.y;
			}
			else
			{
				isControllingCamera = false;
			}
		}
		bool hasResetPlates = plateResetDistance <= Mathf.Epsilon;
		float elapsed = 0f;
		float unscaledElapsed = 0f;
		float num2;
		for (; elapsed < duration; elapsed += num2)
		{
			Vector2 vector = Vector2.Lerp(startPos, targetPos, elapsed / duration);
			base.transform.SetPosition2D(vector);
			chainSplineOffset += currentChainSpeed * Time.deltaTime;
			PositionUpdated();
			if (!hasResetPlates && Vector2.Distance(startPos, vector) >= plateResetDistance)
			{
				hasResetPlates = true;
				ResetPlates();
			}
			if (isControllingCamera)
			{
				camCtrl.SnapTargetToY(clampCameraY.GetClampedBetween(vector.y + camOffset));
			}
			yield return null;
			num2 = Time.deltaTime;
			if (speedCurveDuration > 0f)
			{
				float num3 = unscaledElapsed / speedCurveDuration;
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				unscaledElapsed += Time.deltaTime;
				num2 = Time.deltaTime * speedCurve.Evaluate(num3);
			}
		}
		if (isControllingCamera)
		{
			camCtrl.camTarget.PositionToStart();
			camCtrl.camTarget.destination.y = clampCameraY.GetClampedBetween(targetPos.y + camOffset);
			camCtrl.camTarget.transform.position = camCtrl.KeepWithinSceneBounds(camCtrl.camTarget.destination);
			camCtrl.SetMode(CameraController.CameraMode.PREVIOUS);
		}
		base.transform.SetPosition2D(targetPos);
		PositionUpdated();
		array = mechanismRotators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopRotation();
		}
		if ((bool)bobPlat)
		{
			bobPlat.enabled = true;
			if (bobSilent)
			{
				bobPlat.DoBobSilent();
			}
			else
			{
				bobPlat.DoBob(vibrate);
			}
		}
		if ((bool)movingLoop)
		{
			movingLoop.Stop();
		}
		if ((bool)movingVibrationPlayer)
		{
			movingVibrationPlayer.Stop();
		}
		arriveSound.SpawnAndPlayOneShot(base.transform.position);
		if ((bool)activeWhileMoving)
		{
			activeWhileMoving.SetActive(value: false);
		}
		OnStoppedMoving.Invoke();
		yield return new WaitForSeconds(endDelay);
		SetOpenDoors(stop.OpenDoorLeft, stop.OpenDoorRight, isInstant: false, vibrate);
		if ((bool)doorCloseButton && (!buttonRaiseExitTrigger || (bool)justPressedPlate))
		{
			doorCloseButton.SetLocked(value: false);
		}
		moveRoutine = null;
		justPressedPlate = null;
		ResetPlates();
	}

	private void PositionUpdated()
	{
		KeepWorldPosition[] array = keepWorldPositions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ForceUpdate();
		}
		SplineBase[] array2 = chainSplines;
		foreach (SplineBase splineBase in array2)
		{
			if ((bool)splineBase)
			{
				splineBase.TextureOffset = chainSplineOffset;
				splineBase.UpdateSpline();
			}
		}
	}

	public void SetUnlocked(bool value)
	{
		isUnlocked = value;
		if (isUnlocked)
		{
			OnUnlocked.Invoke();
		}
		SetInitialPos();
	}

	public void Unlock()
	{
		isUnlocked = true;
		OnUnlock.Invoke();
		bool vibrate = true;
		if (!bobSilent)
		{
			HeroController instance = HeroController.instance;
			if ((bool)instance)
			{
				Vector3 vector = base.transform.position - instance.transform.position;
				if (Mathf.Abs(vector.x) > 5f || Mathf.Abs(vector.y) > 5f)
				{
					vibrate = false;
				}
			}
		}
		if ((bool)bobPlat)
		{
			if (bobSilent)
			{
				bobPlat.DoBobSilent();
			}
			else
			{
				bobPlat.DoBob(vibrate);
			}
		}
		if ((bool)doorCloseButton && doorCloseButton.IsDepressed)
		{
			MoveToNextStop();
		}
		else if ((bool)unlockReceptacle && !unlockReceptacleIsInside)
		{
			MoveToStop(currentStop, isControllingCamera: false);
		}
	}

	private void ResetPlates()
	{
		bool flag = moveRoutine != null;
		for (int i = 0; i < stops.Length; i++)
		{
			LiftStop liftStop = stops[i];
			if (currentStop == i && (!flag || (bool)justPressedPlate))
			{
				foreach (TempPressurePlate callPlate in liftStop.CallPlates)
				{
					callPlate.ActivateSilent();
				}
				continue;
			}
			foreach (TempPressurePlate callPlate2 in liftStop.CallPlates)
			{
				if (isSilent)
				{
					callPlate2.DeactivateSilent();
				}
				else
				{
					callPlate2.Deactivate();
				}
			}
		}
	}

	public void AddMoveHooks(Action onStartMove, Action onStopMove)
	{
		OnStartedMoving.AddListener(delegate
		{
			onStartMove();
		});
		OnStoppedMoving.AddListener(delegate
		{
			onStopMove();
		});
	}
}
