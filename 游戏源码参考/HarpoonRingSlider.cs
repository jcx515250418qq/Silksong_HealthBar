using System;
using System.Collections;
using UnityEngine;

public class HarpoonRingSlider : MonoBehaviour
{
	[SerializeField]
	private GameObject ringPrefab;

	[SerializeField]
	private Transform ring;

	[SerializeField]
	private Transform ringTarget;

	[SerializeField]
	private GameObject camLockOnRing;

	[Space]
	[SerializeField]
	private GameObject activeMovingUp;

	[SerializeField]
	private AudioEvent impactUpSound;

	[SerializeField]
	private GameObject activeMovingDown;

	[SerializeField]
	private AudioEvent impactDownSound;

	[Space]
	[SerializeField]
	private float moveDelay;

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private float returnDelay;

	[SerializeField]
	private float returnSpeed;

	[SerializeField]
	private float accelTime;

	private Transform spawnedRing;

	private Collider2D spawnedRingCollider;

	private Coroutine moveRoutine;

	private bool isRingActivated;

	private bool isOnRing;

	private bool isInDelay;

	private void Awake()
	{
		spawnedRing = UnityEngine.Object.Instantiate(ringPrefab, ring.parent).transform;
		spawnedRing.localPosition = ring.localPosition;
		spawnedRing.SetPositionZ(ringPrefab.transform.position.z);
		Transform transform = spawnedRing.Find("Backing");
		if ((bool)transform)
		{
			transform.gameObject.SetActive(value: false);
		}
		spawnedRingCollider = spawnedRing.GetComponent<Collider2D>();
		while (ring.childCount > 0)
		{
			ring.GetChild(0).SetParent(spawnedRing, worldPositionStays: true);
		}
		ring.gameObject.SetActive(value: false);
		ringTarget.gameObject.SetActive(value: false);
		if ((bool)camLockOnRing)
		{
			camLockOnRing.SetActive(value: false);
		}
		if ((bool)activeMovingDown)
		{
			activeMovingDown.SetActive(value: false);
		}
		if ((bool)activeMovingUp)
		{
			activeMovingUp.SetActive(value: false);
		}
	}

	private void Start()
	{
		if (!isRingActivated)
		{
			spawnedRingCollider.enabled = false;
			spawnedRing.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmBool("Collider Start Enabled").Value = false;
		}
	}

	public void HeroOnRing()
	{
		if (isRingActivated)
		{
			isOnRing = true;
			if ((bool)camLockOnRing)
			{
				camLockOnRing.SetActive(value: true);
			}
			if (isInDelay)
			{
				StopCoroutine(moveRoutine);
				moveRoutine = null;
			}
			if (moveRoutine == null)
			{
				StartMoveUp();
			}
		}
	}

	private void StartMoveUp()
	{
		if ((bool)activeMovingDown)
		{
			activeMovingDown.SetActive(value: false);
		}
		if ((bool)activeMovingUp)
		{
			activeMovingUp.SetActive(value: true);
		}
		moveRoutine = StartCoroutine(MoveRoutine(ringTarget.position, moveSpeed, moveDelay, delegate
		{
			if (isOnRing)
			{
				FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "RING DROP IMPACT");
			}
			else
			{
				FSMUtility.SendEventToGameObject(spawnedRing.gameObject, "IMPACT");
			}
			Stopped();
			impactUpSound.SpawnAndPlayOneShot(spawnedRing.transform.position);
		}));
	}

	public void HeroOffRing()
	{
		if (isRingActivated)
		{
			isOnRing = false;
			if ((bool)camLockOnRing)
			{
				camLockOnRing.SetActive(value: false);
			}
			if (isInDelay)
			{
				StopCoroutine(moveRoutine);
				moveRoutine = null;
			}
			if (moveRoutine == null)
			{
				StartMoveDown();
			}
		}
	}

	private void Stopped()
	{
		if ((bool)activeMovingDown)
		{
			activeMovingDown.SetActive(value: false);
		}
		if ((bool)activeMovingUp)
		{
			activeMovingUp.SetActive(value: false);
		}
	}

	private void StartMoveDown()
	{
		if ((bool)activeMovingUp)
		{
			activeMovingUp.SetActive(value: false);
		}
		if ((bool)activeMovingDown)
		{
			activeMovingDown.SetActive(value: true);
		}
		moveRoutine = StartCoroutine(MoveRoutine(ring.position, returnSpeed, returnDelay, delegate
		{
			FSMUtility.SendEventToGameObject(spawnedRing.gameObject, "IMPACT");
			Stopped();
			impactDownSound.SpawnAndPlayOneShot(spawnedRing.transform.position);
		}));
	}

	private IEnumerator MoveRoutine(Vector2 toPos, float speed, float delay, Action onEnd)
	{
		bool wasOnRing = isOnRing;
		isInDelay = true;
		yield return new WaitForSeconds(delay);
		isInDelay = false;
		Vector2 fromPos = spawnedRing.position;
		float num = Vector2.Distance(fromPos, toPos);
		float time = num / speed;
		float speedMultiplier = 0f;
		float elapsed = 0f;
		float unscaledElapsed = 0f;
		while (elapsed < time)
		{
			float t = elapsed / time;
			Vector2 position = Vector2.Lerp(fromPos, toPos, t);
			spawnedRing.SetPosition2D(position);
			yield return null;
			elapsed += Time.deltaTime * speedMultiplier;
			unscaledElapsed += Time.deltaTime;
			speedMultiplier = Mathf.Clamp01(unscaledElapsed / accelTime);
		}
		spawnedRing.SetPosition2D(toPos);
		onEnd?.Invoke();
		moveRoutine = null;
		if (wasOnRing)
		{
			if (!isOnRing)
			{
				StartMoveDown();
			}
		}
		else if (isOnRing)
		{
			StartMoveUp();
		}
	}

	public void ActivateRing()
	{
		isRingActivated = true;
		spawnedRingCollider.enabled = true;
	}
}
