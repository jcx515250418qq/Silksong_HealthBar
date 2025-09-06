using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class AutoRecycleSelf : MonoBehaviour
{
	public interface IRecycleResponder
	{
		void OnRecycled();
	}

	[Header("Trigger Event Type")]
	public AfterEvent afterEvent;

	[Header("Time Event Settings")]
	public float timeToWait;

	public bool unscaledTime;

	[Space]
	public UnityEvent OnBeforeRecycle;

	private AudioSource audioSource;

	private bool validAudioSource;

	private bool ApplicationIsQuitting;

	private bool subscribeLevelUnload;

	private Coroutine recycleTimer;

	private bool recycleTimerRunning;

	private static HashSet<AutoRecycleSelf> activeRecyclers = new HashSet<AutoRecycleSelf>(200);

	private static List<AutoRecycleSelf> recycleList = new List<AutoRecycleSelf>();

	private bool subbed;

	private float timer;

	private bool hasTk2dAnimator;

	private bool hasAnimator;

	private tk2dSpriteAnimator tk2dAnimator;

	private Animator animator;

	private int stateTracker;

	private bool childrenValid;

	private List<IRecycleResponder> recycleChildren = new List<IRecycleResponder>();

	private void Awake()
	{
		FindChildren();
	}

	private void OnEnable()
	{
		ComponentSingleton<AutoRecycleSelfCallbackHooks>.Instance.OnUpdate += OnUpdate;
		if (ObjectPool.IsCreatingPool)
		{
			return;
		}
		timer = 0f;
		stateTracker = 0;
		switch (afterEvent)
		{
		case AfterEvent.TIME:
			if (timeToWait > 0f)
			{
				recycleTimerRunning = true;
				if (unscaledTime)
				{
					timer = Time.realtimeSinceStartup + timeToWait;
				}
				else
				{
					timer = timeToWait;
				}
			}
			subscribeLevelUnload = true;
			break;
		case AfterEvent.LEVEL_UNLOAD:
			subscribeLevelUnload = true;
			break;
		case AfterEvent.AUDIO_CLIP_END:
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
				Debug.LogError(base.name + " requires an AudioSource to auto-recycle itself.");
				validAudioSource = false;
			}
			else
			{
				validAudioSource = true;
			}
			break;
		case AfterEvent.TK2D_ANIM_END:
			tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
			hasAnimator = tk2dAnimator != null;
			subscribeLevelUnload = true;
			break;
		case AfterEvent.MECANIM_ANIM_END:
			animator = GetComponent<Animator>();
			hasAnimator = animator != null;
			subscribeLevelUnload = true;
			break;
		}
		if (subscribeLevelUnload)
		{
			subbed = true;
			activeRecyclers.Add(this);
		}
	}

	private void OnDisable()
	{
		ComponentSingleton<AutoRecycleSelfCallbackHooks>.Instance.OnUpdate -= OnUpdate;
		animator = null;
		hasAnimator = false;
		hasTk2dAnimator = false;
		tk2dAnimator = null;
		recycleTimer = null;
		if (recycleTimerRunning)
		{
			recycleTimerRunning = false;
			RecycleSelf();
		}
		if (subbed)
		{
			subbed = false;
			activeRecyclers.Remove(this);
		}
		if (subscribeLevelUnload)
		{
			_ = ApplicationIsQuitting;
		}
	}

	private void OnDestroy()
	{
		if (subbed)
		{
			subbed = false;
			activeRecyclers.Remove(this);
		}
	}

	private void OnUpdate()
	{
		RecycleUpdate();
	}

	private void OnTransformChildrenChanged()
	{
		childrenValid = false;
	}

	private void RecycleUpdate()
	{
		switch (afterEvent)
		{
		case AfterEvent.TIME:
			if (!recycleTimerRunning)
			{
				break;
			}
			if (unscaledTime)
			{
				if (Time.realtimeSinceStartup >= timer)
				{
					RecycleSelf();
				}
				break;
			}
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				RecycleSelf();
			}
			break;
		case AfterEvent.TK2D_ANIM_END:
			if (!hasTk2dAnimator)
			{
				break;
			}
			if (stateTracker == 0)
			{
				if (tk2dAnimator.Playing)
				{
					stateTracker++;
				}
			}
			else if (!tk2dAnimator.Playing)
			{
				RecycleSelf();
				hasTk2dAnimator = false;
			}
			break;
		case AfterEvent.AUDIO_CLIP_END:
			if (Time.frameCount % 20 == 0)
			{
				Update20();
			}
			break;
		case AfterEvent.MECANIM_ANIM_END:
			if (!hasAnimator)
			{
				break;
			}
			if (stateTracker == 0)
			{
				stateTracker++;
				break;
			}
			if (stateTracker == 1)
			{
				stateTracker++;
				timer = animator.GetCurrentAnimatorStateInfo(0).length;
				break;
			}
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				RecycleSelf();
				hasAnimator = false;
			}
			break;
		case AfterEvent.LEVEL_UNLOAD:
			break;
		}
	}

	private void Update20()
	{
		if (validAudioSource && !audioSource.isPlaying)
		{
			RecycleSelf();
		}
	}

	private void OnApplicationQuit()
	{
		ApplicationIsQuitting = true;
	}

	public static void RecycleActiveRecyclers()
	{
		recycleList.AddRange(activeRecyclers);
		foreach (AutoRecycleSelf recycle in recycleList)
		{
			recycle.RecycleSelf();
		}
		recycleList.Clear();
	}

	private IEnumerator RecycleAfterTime(float wait)
	{
		recycleTimerRunning = true;
		if (unscaledTime)
		{
			timer = Time.realtimeSinceStartup + wait;
			yield return new WaitForSecondsRealtime(wait);
		}
		else
		{
			timer = wait;
			yield return new WaitForSeconds(wait);
		}
		if (recycleTimerRunning)
		{
			RecycleSelf();
		}
		recycleTimerRunning = false;
		recycleTimer = null;
	}

	private void RecycleSelf()
	{
		recycleTimerRunning = false;
		OnBeforeRecycle.Invoke();
		base.gameObject.Recycle();
		OnRecycled();
	}

	public void ForceRecycle()
	{
		RecycleSelf();
	}

	public void ActivateTimer()
	{
		if (!recycleTimerRunning && timeToWait > 0f)
		{
			recycleTimer = StartCoroutine(RecycleAfterTime(timeToWait));
		}
	}

	private void FindChildren()
	{
		if (!childrenValid)
		{
			childrenValid = true;
			recycleChildren.Clear();
			recycleChildren.AddRange(GetComponentsInChildren<IRecycleResponder>(includeInactive: true));
		}
	}

	private void OnRecycled()
	{
		FindChildren();
		int num = 0;
		for (int i = 0; i < recycleChildren.Count; i++)
		{
			IRecycleResponder recycleResponder = recycleChildren[i];
			if (recycleResponder != null)
			{
				recycleResponder.OnRecycled();
				recycleChildren[num++] = recycleResponder;
			}
		}
		if (num < recycleChildren.Count)
		{
			recycleChildren.RemoveRange(num, recycleChildren.Count - num);
		}
	}
}
