using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public class InventorySilkGrub : CustomInventoryItemCollectableDisplay
{
	private static readonly int _idleAnim = Animator.StringToHash("Idle");

	private static readonly int _bounceAnim = Animator.StringToHash("Bounce");

	private static readonly int _singAnim = Animator.StringToHash("Sing Start");

	private static readonly int _suckCocoonAnim = Animator.StringToHash("Suck");

	private static readonly int _suckNoCocoonAnim = Animator.StringToHash("Suck No Cocoon");

	private static readonly int _suckCursedAnim = Animator.StringToHash("Suck Cursed Start");

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private MinMaxFloat doBounceTime;

	[SerializeField]
	private MinMaxFloat selectedBounceTime;

	[SerializeField]
	private MinMaxFloat doSingTime;

	[SerializeField]
	private AudioSource selectedLoop;

	[SerializeField]
	private AudioSource[] chargeLoopSources;

	[SerializeField]
	private AudioEvent chargeStopAudio;

	[SerializeField]
	private GameObject enableWhileSelected;

	[SerializeField]
	private AudioEvent consumeCompleteSilkAudio;

	[SerializeField]
	private AudioEvent consumeCompleteNoSilkAudio;

	private bool isSelected;

	private float timeUntilBounce;

	private float timeUntilSing;

	private bool hasCocoon;

	private void Awake()
	{
		if ((bool)enableWhileSelected)
		{
			enableWhileSelected.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (timeUntilBounce > 0f)
		{
			timeUntilBounce -= Time.unscaledDeltaTime;
			if (timeUntilBounce <= 0f)
			{
				PlayBounce();
			}
		}
		if (timeUntilSing > 0f)
		{
			timeUntilSing -= Time.unscaledDeltaTime;
			if (timeUntilSing <= 0f)
			{
				animator.Play(_singAnim);
			}
		}
	}

	protected override void OnActivate()
	{
		animator.Play(_idleAnim);
		StartTimers();
	}

	protected override void OnDeactivate()
	{
		StopTimers();
	}

	public override void OnSelect()
	{
		isSelected = true;
		timeUntilBounce = 0.001f;
		if ((bool)selectedLoop)
		{
			selectedLoop.Play();
		}
		if ((bool)enableWhileSelected)
		{
			enableWhileSelected.SetActive(value: true);
		}
	}

	public override void OnDeselect()
	{
		isSelected = false;
		timeUntilBounce = doBounceTime.GetRandomValue();
		if ((bool)selectedLoop)
		{
			selectedLoop.Stop();
		}
		if ((bool)enableWhileSelected)
		{
			enableWhileSelected.SetActive(value: false);
		}
	}

	public override void OnConsumeStart()
	{
		PlayerData instance = PlayerData.instance;
		hasCocoon = !string.IsNullOrEmpty(instance.HeroCorpseScene);
		bool flag = hasCocoon && (instance.HeroCorpseType & HeroDeathCocoonTypes.Cursed) == HeroDeathCocoonTypes.Cursed;
		if (hasCocoon)
		{
			animator.Play(flag ? _suckCursedAnim : _suckCocoonAnim);
		}
		else
		{
			animator.Play(_suckNoCocoonAnim);
		}
		if ((bool)selectedLoop)
		{
			selectedLoop.Stop();
		}
		AudioSource[] array = chargeLoopSources;
		foreach (AudioSource audioSource in array)
		{
			if ((bool)audioSource)
			{
				audioSource.Play();
			}
		}
		StopTimers();
	}

	public override void OnConsumeEnd()
	{
		ResetToIdle();
		if (isSelected && (bool)selectedLoop)
		{
			selectedLoop.Play();
		}
		chargeStopAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
	}

	private void ResetToIdle()
	{
		animator.Play(_idleAnim);
		AudioSource[] array = chargeLoopSources;
		foreach (AudioSource audioSource in array)
		{
			if ((bool)audioSource)
			{
				audioSource.Stop();
			}
		}
		StartTimers();
	}

	public override void OnConsumeComplete()
	{
		ResetToIdle();
		if (hasCocoon)
		{
			consumeCompleteSilkAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		}
		else
		{
			consumeCompleteNoSilkAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		}
	}

	public override void OnConsumeBlocked()
	{
		PlayBounce();
	}

	private void PlayBounce()
	{
		timeUntilBounce = (isSelected ? selectedBounceTime : doBounceTime).GetRandomValue();
		animator.Play(_bounceAnim);
	}

	private void StartTimers()
	{
		timeUntilBounce = doBounceTime.GetRandomValue();
		if (SilkGrubCocoon.IsAnyActive)
		{
			timeUntilSing = doSingTime.GetRandomValue();
		}
	}

	private void StopTimers()
	{
		timeUntilBounce = 0f;
		timeUntilSing = 0f;
	}
}
