using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class MetronomePlat : MonoBehaviour
{
	public interface INotify
	{
		void PlatRetracted(MetronomePlat plat);
	}

	[SerializeField]
	private TimedTicker ticker;

	[SerializeField]
	private bool inverted;

	[Space]
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioEvent inAudio;

	[SerializeField]
	private AudioEvent outAudio;

	[Space]
	[SerializeField]
	private Transform metronome;

	[SerializeField]
	private MinMaxFloat metronomeRotateRange;

	[SerializeField]
	[Range(0f, 1f)]
	private float metronomeTickPoint = 0.5f;

	[SerializeField]
	private AnimationCurve metronomeSwingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[SerializeField]
	private float metronomeFpsLimit;

	[Space]
	[SerializeField]
	private HeroVibrationRegion heroVibrationRegion;

	private bool hasVibrationRegion;

	private bool receivedEvent;

	private HashSet<INotify> registeredNotifiers;

	private List<INotify> iteratingNotifiers;

	private static readonly int _inAnimatorState = Animator.StringToHash("In");

	private static readonly int _outAnimatorState = Animator.StringToHash("Out");

	private void Awake()
	{
		ticker.ReceivedEvent += delegate
		{
			receivedEvent = true;
		};
		hasVibrationRegion = heroVibrationRegion;
	}

	private void Start()
	{
		StartCoroutine(SwingRoutine());
	}

	private IEnumerator SwingRoutine()
	{
		bool flipped = inverted;
		animator.Play(flipped ? _inAnimatorState : _outAnimatorState, 0, 1f);
		while (true)
		{
			bool changedState = false;
			float elapsed = 0f;
			float duration = ticker.TickDelay;
			float nextUpdateElapsed = 0f;
			for (; elapsed <= duration; elapsed += Time.deltaTime)
			{
				float time = elapsed / duration;
				time = metronomeSwingCurve.Evaluate(time);
				if (!changedState && time >= metronomeTickPoint)
				{
					if (flipped)
					{
						animator.Play(_inAnimatorState);
						inAudio.PlayOnSource(audioSource);
					}
					else
					{
						animator.Play(_outAnimatorState);
						outAudio.PlayOnSource(audioSource);
					}
					if (hasVibrationRegion)
					{
						heroVibrationRegion.StartVibration();
					}
					changedState = true;
				}
				if (flipped)
				{
					time = 1f - time;
				}
				bool flag = true;
				if (metronomeFpsLimit > 0f)
				{
					if (elapsed >= nextUpdateElapsed)
					{
						nextUpdateElapsed = elapsed + 1f / metronomeFpsLimit;
					}
					else
					{
						flag = false;
					}
				}
				if (flag)
				{
					float lerpedValue = metronomeRotateRange.GetLerpedValue(time);
					metronome.SetLocalRotation2D(lerpedValue);
				}
				yield return null;
			}
			while (!receivedEvent)
			{
				yield return null;
			}
			receivedEvent = false;
			flipped = !flipped;
		}
	}

	public void NotifyRetracted()
	{
		if (registeredNotifiers == null)
		{
			return;
		}
		if (iteratingNotifiers == null)
		{
			iteratingNotifiers = new List<INotify>(registeredNotifiers.Count);
		}
		iteratingNotifiers.AddRange(registeredNotifiers);
		foreach (INotify iteratingNotifier in iteratingNotifiers)
		{
			iteratingNotifier.PlatRetracted(this);
		}
		iteratingNotifiers.Clear();
	}

	public bool RegisterNotifier(INotify notify)
	{
		if (registeredNotifiers == null)
		{
			registeredNotifiers = new HashSet<INotify>();
		}
		return registeredNotifiers.Add(notify);
	}

	public bool UnregisterNotifier(INotify notify)
	{
		return registeredNotifiers.Remove(notify);
	}
}
