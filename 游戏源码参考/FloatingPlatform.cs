using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FloatingPlatform : MonoBehaviour
{
	[SerializeField]
	private Transform[] moveObjects;

	[Space]
	[SerializeField]
	private float moveAmount = 0.1f;

	[SerializeField]
	private float moveSpeed = 0.05f;

	[SerializeField]
	private float maxSinOffset = 0.1f;

	[Space]
	[SerializeField]
	private float landSinStopTime;

	[SerializeField]
	private float landSinStartTime;

	[Space]
	[SerializeField]
	private float landMoveAmount = 0.1f;

	[SerializeField]
	private float landMoveLength = 0.5f;

	[SerializeField]
	private AnimationCurve landCurve;

	[SerializeField]
	private ParticleSystem[] landParticles;

	[Space]
	[SerializeField]
	private UnityEvent OnLanded;

	private float overallSinOffset;

	private float sinBlend;

	private float landOffset;

	private Vector3[] initialObjectPositions;

	private bool isHeroOnTop;

	private Coroutine landReactRoutine;

	private Coroutine landSinRoutine;

	private void Start()
	{
		overallSinOffset = Random.Range(0f, 1f);
		sinBlend = 1f;
		initialObjectPositions = new Vector3[moveObjects.Length];
		for (int i = 0; i < moveObjects.Length; i++)
		{
			if (!(moveObjects[i] == null))
			{
				initialObjectPositions[i] = moveObjects[i].localPosition;
			}
		}
	}

	private void Update()
	{
		if (Mathf.Abs(moveAmount) <= 0f)
		{
			return;
		}
		for (int i = 0; i < moveObjects.Length; i++)
		{
			Transform transform = moveObjects[i];
			if (!(transform == null))
			{
				float num = maxSinOffset * ((float)(i + 1) / (float)moveObjects.Length) + overallSinOffset;
				float num2 = moveAmount * sinBlend;
				transform.localPosition = initialObjectPositions[i] + (Vector3.up * (num2 * Mathf.Sin((Time.time + num) * moveSpeed)) + Vector3.down * landOffset) + Vector3.up * num2;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!collision.collider.CompareTag("Player"))
		{
			return;
		}
		Collision2DUtils.Collision2DSafeContact safeContact = collision.GetSafeContact();
		if (!(safeContact.Point.y < collision.otherCollider.bounds.max.y))
		{
			if (!safeContact.IsLegitimate)
			{
				Debug.LogWarning("Platform contact point was not legitimate! (dang it, Unity D:)", this);
			}
			isHeroOnTop = true;
			OnLanded.Invoke();
			if (landReactRoutine != null)
			{
				StopCoroutine(landReactRoutine);
			}
			landReactRoutine = StartCoroutine(LandPush());
			if (landSinRoutine != null)
			{
				StopCoroutine(landSinRoutine);
			}
			landSinRoutine = StartCoroutine(LandSinState(enableSinFloat: false));
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (isHeroOnTop && collision.collider.CompareTag("Player"))
		{
			if (landSinRoutine != null)
			{
				StopCoroutine(landSinRoutine);
			}
			landSinRoutine = StartCoroutine(LandSinState(enableSinFloat: true));
		}
	}

	private IEnumerator LandPush()
	{
		float elapsed = 0f;
		ParticleSystem[] array = landParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		for (; elapsed < landMoveLength; elapsed += Time.deltaTime)
		{
			landOffset = landCurve.Evaluate(elapsed / landMoveLength) * landMoveAmount;
			yield return null;
		}
		landOffset = 0f;
	}

	private IEnumerator LandSinState(bool enableSinFloat)
	{
		float elapsed = 0f;
		float startSinBlend = sinBlend;
		float targetSinBlend;
		float duration;
		if (enableSinFloat)
		{
			targetSinBlend = 1f;
			duration = landSinStartTime;
		}
		else
		{
			targetSinBlend = 0f;
			duration = landSinStopTime;
		}
		for (; elapsed < duration; elapsed += Time.deltaTime)
		{
			sinBlend = Mathf.Lerp(startSinBlend, targetSinBlend, elapsed / duration);
			yield return null;
		}
		sinBlend = targetSinBlend;
	}
}
