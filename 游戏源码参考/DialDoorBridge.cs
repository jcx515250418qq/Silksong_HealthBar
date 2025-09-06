using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialDoorBridge : MonoBehaviour
{
	private static readonly int _openAnim = Animator.StringToHash("Open");

	private static readonly int _closeAnim = Animator.StringToHash("Close");

	private static readonly int _movingAnim = Animator.StringToHash("Moving");

	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private Lever[] leversClockwise;

	[SerializeField]
	private Lever[] leversCounterclockwise;

	[SerializeField]
	private GameObject activeVertical;

	[SerializeField]
	private GameObject activeHorizontal;

	[SerializeField]
	private Animator[] doors;

	[SerializeField]
	private Transform front;

	[SerializeField]
	private Transform back;

	[SerializeField]
	private float moveDelay;

	[SerializeField]
	private float doorOpenDelay;

	[SerializeField]
	private AnimationCurve moveCurve;

	[SerializeField]
	private float moveDuration;

	[SerializeField]
	private CameraShakeTarget moveRumble;

	[SerializeField]
	[Range(0f, 1f)]
	private float endSlamPoint = 1f;

	[SerializeField]
	private CameraShakeTarget endSlamShake;

	[SerializeField]
	private CogRotationController cogController;

	[SerializeField]
	private bool startHorizontal;

	[Space]
	public UnityEvent OnDoorsClose;

	public UnityEvent OnDoorsOpen;

	[Space]
	public UnityEvent OnStartMoving;

	[Space]
	public UnityEvent OnEndSlam;

	private bool isRotated;

	private Coroutine rotateRoutine;

	private void Awake()
	{
		SetInitialRotation(startHorizontal);
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out bool value)
			{
				value = isRotated;
			};
			persistent.OnSetSaveState += SetInitialRotation;
		}
		Lever[] array = leversClockwise;
		foreach (Lever lever in array)
		{
			if ((bool)lever)
			{
				lever.OnHitDelayed.AddListener(delegate
				{
					OnLeverHitDelayed(1f);
				});
			}
		}
		array = leversCounterclockwise;
		foreach (Lever lever2 in array)
		{
			if ((bool)lever2)
			{
				lever2.OnHitDelayed.AddListener(delegate
				{
					OnLeverHitDelayed(-1f);
				});
			}
		}
	}

	private void Start()
	{
		Animator[] array = doors;
		foreach (Animator obj in array)
		{
			obj.Play(_openAnim, 0, 1f);
			obj.Update(0f);
			obj.enabled = false;
		}
	}

	private void SetInitialRotation(bool value)
	{
		isRotated = value;
		if ((bool)activeHorizontal)
		{
			activeHorizontal.SetActive(isRotated);
		}
		if ((bool)activeVertical)
		{
			activeVertical.SetActive(!isRotated);
		}
		if (isRotated)
		{
			front.SetLocalRotation2D(-90f);
			back.SetLocalRotation2D(90f);
		}
		else
		{
			front.SetLocalRotation2D(0f);
			back.SetLocalRotation2D(0f);
		}
	}

	private void OnLeverHitDelayed(float rotateDirection)
	{
		if (rotateRoutine == null)
		{
			isRotated = !isRotated;
			rotateRoutine = StartCoroutine(MoveRotate(rotateDirection));
		}
	}

	private IEnumerator MoveRotate(float direction)
	{
		Animator[] array = doors;
		foreach (Animator obj in array)
		{
			obj.enabled = true;
			obj.Play(_closeAnim);
		}
		OnDoorsClose.Invoke();
		if ((bool)activeHorizontal)
		{
			activeHorizontal.SetActive(value: true);
		}
		if ((bool)activeVertical)
		{
			activeVertical.SetActive(value: true);
		}
		yield return new WaitForSeconds(moveDelay);
		OnStartMoving.Invoke();
		array = doors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(_movingAnim);
		}
		Vector3 initialFrontRotation = front.localEulerAngles;
		Vector3 initialBackRotation = back.localEulerAngles;
		Vector3 targetFrontRotation = initialFrontRotation;
		targetFrontRotation.z += -90f * direction;
		Vector3 targetBackRotation = initialBackRotation;
		targetBackRotation.z += 90f * direction;
		moveRumble.DoShake(this);
		bool hasSlammed = false;
		float elapsed = 0f;
		while (elapsed < moveDuration)
		{
			float t = elapsed / moveDuration;
			float t2 = moveCurve.Evaluate(t);
			front.localEulerAngles = Vector3.LerpUnclamped(initialFrontRotation, targetFrontRotation, t2);
			back.localEulerAngles = Vector3.LerpUnclamped(initialBackRotation, targetBackRotation, t2);
			if (!hasSlammed && t >= endSlamPoint)
			{
				hasSlammed = true;
				EndSlam();
			}
			yield return null;
			elapsed += Time.deltaTime;
			if ((bool)cogController)
			{
				cogController.AnimateRotation += t * direction;
			}
		}
		front.localEulerAngles = targetFrontRotation;
		back.localEulerAngles = targetBackRotation;
		if (!hasSlammed)
		{
			EndSlam();
		}
		yield return new WaitForSeconds(doorOpenDelay);
		if ((bool)activeHorizontal)
		{
			activeHorizontal.SetActive(isRotated);
		}
		if ((bool)activeVertical)
		{
			activeVertical.SetActive(!isRotated);
		}
		array = doors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(_openAnim);
		}
		OnDoorsOpen.Invoke();
		rotateRoutine = null;
	}

	private void EndSlam()
	{
		moveRumble.CancelShake();
		endSlamShake.DoShake(this);
		OnEndSlam.Invoke();
	}
}
