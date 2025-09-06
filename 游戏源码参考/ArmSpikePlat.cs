using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ArmSpikePlat : MonoBehaviour
{
	[SerializeField]
	private Transform pivot;

	[SerializeField]
	private Transform platform;

	[SerializeField]
	private JitterSelf platformJitter;

	[SerializeField]
	private GameObject[] activePlatParts;

	[SerializeField]
	private GameObject[] inactivePlatParts;

	[SerializeField]
	private GameObject[] staticActive;

	[SerializeField]
	private GameObject strikePrefab;

	[Space]
	[SerializeField]
	protected float hitStartDelay;

	[SerializeField]
	protected float landStartDelay;

	[SerializeField]
	private float rotateDuration;

	[Space]
	[SerializeField]
	private UnityEvent onActivate;

	[SerializeField]
	private UnityEvent onEnd;

	private Coroutine rotateRoutine;

	protected float PreviousDirection { get; private set; }

	public bool IsRotating => rotateRoutine != null;

	private void Awake()
	{
		PreviousDirection = 1f;
	}

	private void OnEnable()
	{
		UpdatePlatActive();
		SetPlatMoving(isMoving: false);
	}

	private void OnDisable()
	{
		if (rotateRoutine != null)
		{
			StopCoroutine(rotateRoutine);
			rotateRoutine = null;
		}
	}

	public void DoLandRotate()
	{
		if (IsPlatformLeft())
		{
			DoRotate(landStartDelay, 1f);
		}
		else
		{
			DoRotate(landStartDelay, -1f);
		}
	}

	private bool IsPlatformLeft()
	{
		return platform.position.x < pivot.position.x;
	}

	protected void DoRotate(float startDelay, float direction, bool doLandEffect = true)
	{
		if (rotateRoutine == null)
		{
			if (doLandEffect && (bool)strikePrefab)
			{
				strikePrefab.Spawn(platform.position);
			}
			onActivate.Invoke();
			OnActivate();
			rotateRoutine = StartCoroutine(RotationSequence(startDelay, direction));
		}
	}

	private IEnumerator RotationSequence(float startDelay, float rotateDirection)
	{
		if ((bool)platformJitter)
		{
			platformJitter.StartJitter();
		}
		yield return new WaitForSeconds(startDelay);
		if ((bool)platformJitter)
		{
			platformJitter.StopJitter();
		}
		SetPlatActive(value: false);
		SetPlatMoving(isMoving: true);
		PreviousDirection = rotateDirection;
		float initialAngle = pivot.localEulerAngles.z;
		float targetAngle = initialAngle + rotateDirection * 180f;
		for (float elapsed = 0f; elapsed < rotateDuration; elapsed += Time.deltaTime)
		{
			pivot.SetRotation2D(Mathf.Lerp(initialAngle, targetAngle, elapsed / rotateDuration));
			yield return null;
		}
		pivot.SetRotation2D(targetAngle);
		UpdatePlatActive();
		SetPlatMoving(isMoving: false);
		rotateRoutine = null;
		onEnd.Invoke();
		OnEnd();
	}

	private void UpdatePlatActive()
	{
		SetPlatActive(IsPlatformUpright());
	}

	private bool IsPlatformUpright()
	{
		bool flag = platform.eulerAngles.z.IsWithinTolerance(5f, 0f);
		if (base.transform.lossyScale.y < 0f)
		{
			flag = !flag;
		}
		return flag;
	}

	private void SetPlatActive(bool value)
	{
		activePlatParts.SetAllActive(value);
		inactivePlatParts.SetAllActive(!value);
	}

	private void SetPlatMoving(bool isMoving)
	{
		staticActive.SetAllActive(!isMoving);
	}

	public void DoHitRotate(HitInstance.HitDirection hitDirection)
	{
		bool flag = IsPlatformLeft();
		IsPlatformUpright();
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Up:
			if (flag)
			{
				DoRotate(hitStartDelay, -1f);
			}
			else
			{
				DoRotate(hitStartDelay, 1f);
			}
			break;
		case HitInstance.HitDirection.Down:
			if (flag)
			{
				DoRotate(hitStartDelay, 1f);
			}
			else
			{
				DoRotate(hitStartDelay, -1f);
			}
			break;
		case HitInstance.HitDirection.Left:
		case HitInstance.HitDirection.Right:
			break;
		}
	}

	protected virtual void OnActivate()
	{
	}

	protected virtual void OnEnd()
	{
	}
}
