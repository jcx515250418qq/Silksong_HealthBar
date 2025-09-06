using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CogEnergyTimeline : UnlockablePropBase, IMutable
{
	[SerializeField]
	private PlayableDirector director;

	[SerializeField]
	private Mutable directorMuter;

	[SerializeField]
	private float maxTime;

	[SerializeField]
	private float maxEnergy = 360f;

	[SerializeField]
	private float animateTime;

	[SerializeField]
	private AnimationCurve animateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AudioEventRandom hitSounds;

	[SerializeField]
	private AudioEvent finishSound;

	[SerializeField]
	private AudioEvent backAtStartSound;

	[SerializeField]
	private AudioSource retractAudioLoop;

	[Space]
	[SerializeField]
	private InteractableBase realBenchInteract;

	[SerializeField]
	private NestedFadeGroupBase realBenchFadeGroup;

	[SerializeField]
	private float realBenchFadeTime;

	[SerializeField]
	private float benchActivateDelay;

	[SerializeField]
	private GameObject activateObj;

	[Space]
	[SerializeField]
	private UnityEvent OnAnimateStart;

	[SerializeField]
	private UnityEvent OnAnimateComplete;

	private bool wasAbove;

	private float energy;

	private Coroutine animateRoutine;

	private bool isOpen;

	private bool muted;

	public bool Muted => muted;

	private void Start()
	{
		UpdateEnergy(0f, playSound: false);
		SetComplete(value: false);
	}

	private void OnValidate()
	{
		if (director != null && directorMuter == null)
		{
			directorMuter = director.GetComponent<Mutable>();
		}
	}

	public void SetEnergy(float newEnergy, bool animate)
	{
		if (animateRoutine != null)
		{
			StopCoroutine(animateRoutine);
			animateRoutine = null;
		}
		if (Math.Abs(newEnergy - energy) > Mathf.Epsilon)
		{
			if (newEnergy > energy)
			{
				if (retractAudioLoop.isPlaying)
				{
					retractAudioLoop.Stop();
				}
			}
			else if (!retractAudioLoop.isPlaying)
			{
				retractAudioLoop.Play();
			}
		}
		if (animate)
		{
			animateRoutine = StartCoroutine(Animate(newEnergy));
			return;
		}
		UpdateEnergy(newEnergy, playSound: false);
		SetComplete(isOpen && newEnergy / maxEnergy > 0.99f);
	}

	private IEnumerator Animate(float newEnergy)
	{
		float startEnergy = energy;
		SetComplete(value: false);
		OnAnimateStart.Invoke();
		hitSounds.SpawnAndPlayOneShot(base.transform.position);
		for (float elapsed = 0f; elapsed < animateTime; elapsed += Time.deltaTime)
		{
			float t = animateCurve.Evaluate(elapsed / animateTime);
			UpdateEnergy(Mathf.Lerp(startEnergy, newEnergy, t));
			yield return null;
		}
		UpdateEnergy(newEnergy);
		if (newEnergy / maxEnergy > 0.99f)
		{
			OnAnimateComplete.Invoke();
			finishSound.SpawnAndPlayOneShot(base.transform.position);
			if (benchActivateDelay > 0f)
			{
				yield return new WaitForSeconds(benchActivateDelay);
			}
			if (isOpen)
			{
				SetComplete(value: true);
			}
		}
		else
		{
			SetComplete(value: false);
		}
	}

	private void UpdateEnergy(float newEnergy, bool playSound = true)
	{
		energy = newEnergy;
		float num = Mathf.Clamp01(energy / maxEnergy);
		bool flag = num > 0.01f;
		if (!flag && wasAbove)
		{
			backAtStartSound.SpawnAndPlayOneShot(base.transform.position);
		}
		wasAbove = flag;
		if ((bool)director)
		{
			bool mute = muted;
			if (!playSound)
			{
				SetMute(value: true);
			}
			director.time = maxTime * num;
			director.Evaluate();
			SetMute(mute);
		}
	}

	private void SetComplete(bool value)
	{
		if ((bool)realBenchInteract)
		{
			if (value)
			{
				realBenchInteract.Activate();
			}
			else
			{
				realBenchInteract.Deactivate(allowQueueing: false);
			}
		}
		if ((bool)realBenchFadeGroup)
		{
			realBenchFadeGroup.FadeTo(value ? 1f : 0f, realBenchFadeTime);
		}
		if ((bool)activateObj)
		{
			activateObj.SetActive(value);
		}
		if (value && retractAudioLoop.isPlaying)
		{
			retractAudioLoop.Stop();
		}
	}

	public override void Open()
	{
		isOpen = true;
		SetEnergy(maxEnergy, animate: true);
	}

	public override void Opened()
	{
		isOpen = true;
		SetEnergy(maxEnergy, animate: false);
	}

	public void SetMute(bool value)
	{
		muted = value;
		if ((bool)directorMuter)
		{
			directorMuter.SetMute(value);
		}
	}
}
