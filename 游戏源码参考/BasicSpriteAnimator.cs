using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class BasicSpriteAnimator : MonoBehaviour
{
	[SerializeField]
	private float fps = 30f;

	[Space]
	[SerializeField]
	private MinMaxFloat startDelay;

	[SerializeField]
	private Sprite[] preFrames;

	[SerializeField]
	private Sprite[] frames;

	[SerializeField]
	private bool startRandom = true;

	[SerializeField]
	private bool looping = true;

	[SerializeField]
	private bool playOnEnable = true;

	[SerializeField]
	private bool visibilityCulling;

	[Space]
	public UnityEvent OnAnimStart;

	public UnityEvent OnAnimEnd;

	private SpriteRenderer rend;

	private Coroutine animRoutine;

	private float maxTime;

	private bool queuedStop;

	private bool isVisible;

	private int FrameCount
	{
		get
		{
			Sprite[] array = preFrames;
			int num = ((array != null) ? array.Length : 0);
			Sprite[] array2 = frames;
			return num + ((array2 != null) ? array2.Length : 0);
		}
	}

	public float Length => (float)FrameCount / fps;

	private void OnValidate()
	{
		maxTime = (float)FrameCount * (1f / fps);
	}

	private void Awake()
	{
		OnValidate();
		rend = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		isVisible = rend.isVisible;
		if (playOnEnable)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		StopAnimRoutine();
	}

	private void OnBecameVisible()
	{
		isVisible = true;
	}

	private void OnBecameInvisible()
	{
		isVisible = false;
	}

	public void Play()
	{
		PlayInternal(forceStartRandom: false);
	}

	public void PlayRandom()
	{
		PlayInternal(forceStartRandom: true);
	}

	private void PlayInternal(bool forceStartRandom)
	{
		if (base.isActiveAndEnabled)
		{
			StopAnimRoutine();
			if (FrameCount > 1)
			{
				animRoutine = StartCoroutine(Animate(forceStartRandom));
			}
			OnAnimStart.Invoke();
		}
	}

	public void QueueStop()
	{
		queuedStop = true;
	}

	public void StopImmediately()
	{
		StopAnimRoutine();
	}

	private void StopAnimRoutine()
	{
		if (animRoutine != null)
		{
			StopCoroutine(animRoutine);
			animRoutine = null;
			AnimEnded();
		}
	}

	private IEnumerator Animate(bool forceStartRandom)
	{
		queuedStop = false;
		float elapsedTime = 0f;
		bool hasStartedLoop = false;
		float randomValue = startDelay.GetRandomValue();
		if (randomValue > 0f)
		{
			rend.sprite = ((preFrames.Length != 0) ? preFrames[0] : frames[0]);
			yield return new WaitForSeconds(randomValue);
		}
		WaitUntil wait = (visibilityCulling ? new WaitUntil(() => isVisible) : null);
		while (true)
		{
			int num = Mathf.FloorToInt((float)FrameCount * (elapsedTime / maxTime));
			Sprite sprite;
			if (num < preFrames.Length)
			{
				sprite = preFrames[num];
			}
			else
			{
				if (frames.Length == 0)
				{
					break;
				}
				if (!hasStartedLoop && (startRandom || forceStartRandom))
				{
					elapsedTime = Random.Range((float)preFrames.Length / fps, maxTime);
				}
				hasStartedLoop = true;
				num -= preFrames.Length;
				num %= frames.Length;
				sprite = frames[num];
			}
			if (rend.enabled)
			{
				rend.sprite = sprite;
			}
			double waitStartTime = Time.timeAsDouble;
			yield return wait;
			float num2 = (float)(Time.timeAsDouble - waitStartTime);
			elapsedTime += num2;
			if (!(elapsedTime < maxTime))
			{
				if (queuedStop)
				{
					queuedStop = false;
					break;
				}
				if (!looping)
				{
					break;
				}
				elapsedTime %= maxTime;
				float num3 = (float)preFrames.Length / fps;
				elapsedTime += num3;
			}
		}
		AnimEnded();
	}

	private void AnimEnded()
	{
		animRoutine = null;
		OnAnimEnd.Invoke();
	}
}
