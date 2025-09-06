using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class WeaverHeatLamp : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects appearTrigger;

	[Space]
	[SerializeField]
	private Animator core;

	[SerializeField]
	private Animator warmthRegion;

	[Space]
	[SerializeField]
	private NestedFadeGroupBase snowFader;

	[SerializeField]
	private float snowFadeOffDuration;

	[SerializeField]
	private AnimationCurve snowFadeOffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	[Space]
	[SerializeField]
	private AudioSource insideAudioLoop;

	[SerializeField]
	private AudioEvent enterSound;

	[SerializeField]
	private AudioEvent exitSound;

	private Coroutine snowFadeOffRoutine;

	private bool isInside;

	private static readonly int _activateId = Animator.StringToHash("Activate");

	private static readonly int _activeId = Animator.StringToHash("Active");

	private static readonly int _disappearId = Animator.StringToHash("Disappear");

	private void OnEnable()
	{
		HeroController.instance.OnHazardRespawn += OnHeroHazardRespawn;
		if ((bool)appearTrigger)
		{
			appearTrigger.InsideStateChanged += OnAppearTriggerState;
		}
	}

	private void Start()
	{
		warmthRegion.SetBool(_activeId, value: false);
		warmthRegion.Play(_disappearId, 0, 1f);
		core.SetBool(_activeId, value: false);
		core.Play(_disappearId, 0, 1f);
	}

	private void OnDisable()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.OnHazardRespawn -= OnHeroHazardRespawn;
		}
		if ((bool)appearTrigger)
		{
			appearTrigger.InsideStateChanged -= OnAppearTriggerState;
		}
	}

	public void RefreshUnlocked()
	{
	}

	private void OnAppearTriggerState(bool isInside)
	{
		this.isInside = isInside;
		warmthRegion.SetBool(_activeId, isInside);
		core.SetBool(_activeId, isInside);
		if (isInside)
		{
			Activated();
			if ((bool)insideAudioLoop)
			{
				insideAudioLoop.Play();
			}
			return;
		}
		if (base.gameObject.scene.isLoaded)
		{
			exitSound.SpawnAndPlayOneShot(base.transform.position);
		}
		if ((bool)insideAudioLoop)
		{
			insideAudioLoop.Stop();
		}
	}

	public void HitActivate()
	{
		Activated();
	}

	private void Activated()
	{
		warmthRegion.SetTrigger(_activateId);
		core.SetTrigger(_activateId);
		if (snowFadeOffRoutine == null && (bool)snowFader && snowFader.AlphaSelf > Mathf.Epsilon)
		{
			snowFadeOffRoutine = StartCoroutine(SnowFadeOff());
		}
		enterSound.SpawnAndPlayOneShot(base.transform.position);
	}

	private IEnumerator SnowFadeOff()
	{
		float initialAlpha = snowFader.AlphaSelf;
		for (float elapsed = 0f; elapsed <= snowFadeOffDuration; elapsed += Time.deltaTime)
		{
			float t = snowFadeOffCurve.Evaluate(elapsed / snowFadeOffDuration);
			snowFader.AlphaSelf = Mathf.Lerp(0f, initialAlpha, t);
			yield return null;
		}
		snowFader.AlphaSelf = 0f;
	}

	private void OnHeroHazardRespawn()
	{
		if (!isInside)
		{
			core.SetBool(_activeId, value: false);
			core.Play(_disappearId, 0, 1f);
			warmthRegion.Play(_disappearId, 0, 1f);
		}
	}
}
