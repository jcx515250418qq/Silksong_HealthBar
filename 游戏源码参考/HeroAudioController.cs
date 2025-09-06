using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

public class HeroAudioController : MonoBehaviour
{
	private HeroController heroCtrl;

	private HeroVibrationController heroVibrationCtrl;

	[Header("Sound Effects")]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	public AudioSource softLanding;

	public AudioSource hardLanding;

	public AudioSource jump;

	public AudioSource takeHit;

	public AudioSource backDash;

	public AudioSource dash;

	public AudioSource dashSilk;

	public AudioSource updraftIdle;

	public AudioSource windyIdle;

	public AudioSource footSteps;

	[SerializeField]
	private RandomAudioClipTable runStartClips;

	[SerializeField]
	private RandomAudioClipTable runStartClipsCloakless;

	public AudioSource wallslide;

	public AudioSource nailArtCharge;

	public AudioSource nailArtReady;

	public AudioSource falling;

	public AudioSource walljump;

	private bool playedRunStart;

	private bool playedWalkStart;

	private Coroutine fallingCo;

	private double canPlayTime;

	private bool canPlayWalk;

	private bool canPlayRun;

	private bool canPlaySprint;

	private RandomAudioClipTable footstepsTable;

	private bool CanPlayFootsteps
	{
		get
		{
			if (!canPlayWalk && !canPlayRun && !canPlaySprint && !heroCtrl.cState.dashing)
			{
				return canPlayTime > Time.timeAsDouble;
			}
			return true;
		}
	}

	public bool BlockFootstepAudio { get; set; }

	public event Action OnPlayFootstep;

	private void Awake()
	{
		heroCtrl = GetComponent<HeroController>();
	}

	private void Start()
	{
		if ((bool)heroCtrl)
		{
			heroVibrationCtrl = heroCtrl.GetVibrationCtrl();
		}
	}

	public void PlaySound(HeroSounds soundEffect, bool playVibration = false)
	{
		if (heroCtrl.cState.isPaused)
		{
			return;
		}
		bool flag = false;
		switch (soundEffect)
		{
		case HeroSounds.SOFT_LANDING:
			RandomizePitch(jump, 0.9f, 1.1f);
			softLanding.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(softLanding, soundEffect);
			}
			break;
		case HeroSounds.HARD_LANDING:
			hardLanding.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(hardLanding, soundEffect);
			}
			break;
		case HeroSounds.JUMP:
			RandomizePitch(jump, 0.9f, 1.1f);
			jump.Play();
			break;
		case HeroSounds.WALLJUMP:
			RandomizePitch(walljump, 0.9f, 1.1f);
			walljump.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(walljump, soundEffect);
			}
			break;
		case HeroSounds.TAKE_HIT:
			takeHit.Play();
			break;
		case HeroSounds.DASH:
			dash.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(dash, soundEffect);
			}
			break;
		case HeroSounds.DASH_SILK:
			dashSilk.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(dashSilk, soundEffect);
			}
			break;
		case HeroSounds.WALLSLIDE:
			wallslide.Play();
			if (playVibration)
			{
				heroVibrationCtrl.StartWallSlide();
			}
			break;
		case HeroSounds.BACKDASH:
			backDash.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(backDash, soundEffect);
			}
			break;
		case HeroSounds.FOOTSTEPS_WALK:
		{
			if (BlockFootstepAudio)
			{
				break;
			}
			bool canPlayFootsteps = CanPlayFootsteps;
			canPlayWalk = true;
			if (!playedWalkStart && !canPlayFootsteps)
			{
				playedWalkStart = true;
				flag = true;
				if (!playVibration)
				{
				}
			}
			break;
		}
		case HeroSounds.FOOTSTEPS_RUN:
		{
			if (BlockFootstepAudio)
			{
				break;
			}
			bool canPlayFootsteps2 = CanPlayFootsteps;
			canPlayRun = true;
			if (!playedRunStart && !canPlayFootsteps2)
			{
				playedRunStart = true;
				flag = true;
				if (!playVibration)
				{
				}
			}
			break;
		}
		case HeroSounds.FOOTSTEPS_SPRINT:
			canPlaySprint = true;
			break;
		case HeroSounds.NAIL_ART_CHARGE:
			nailArtCharge.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(nailArtCharge, soundEffect, loop: true);
			}
			break;
		case HeroSounds.NAIL_ART_READY:
			nailArtReady.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(nailArtReady, soundEffect);
			}
			break;
		case HeroSounds.FALLING:
			fallingCo = StartCoroutine(FadeInVolume(falling, 0.7f));
			falling.Play();
			if (playVibration)
			{
				heroVibrationCtrl.PlayVibration(falling, soundEffect, loop: true);
			}
			break;
		case HeroSounds.UPDRAFT_IDLE:
			if (!updraftIdle.isPlaying)
			{
				updraftIdle.Play();
				if (playVibration)
				{
					heroVibrationCtrl.PlayVibration(updraftIdle, soundEffect, loop: true);
				}
			}
			break;
		case HeroSounds.WINDY_IDLE:
			if (!windyIdle.isPlaying)
			{
				windyIdle.Play();
				if (playVibration)
				{
					heroVibrationCtrl.PlayVibration(windyIdle, soundEffect, loop: true);
				}
			}
			break;
		}
		if (flag && !heroCtrl.cState.transitioning)
		{
			if (heroCtrl.Config.ForceBareInventory)
			{
				runStartClipsCloakless.SpawnAndPlayOneShot(audioSourcePrefab, footSteps.transform.position);
			}
			else
			{
				runStartClips.SpawnAndPlayOneShot(audioSourcePrefab, footSteps.transform.position);
			}
		}
	}

	public void StopSound(HeroSounds soundEffect, bool resetStarts = true)
	{
		if ((bool)heroVibrationCtrl)
		{
			heroVibrationCtrl.StopVibration(soundEffect);
		}
		switch (soundEffect)
		{
		case HeroSounds.FOOTSTEPS_RUN:
			canPlayRun = false;
			if (resetStarts)
			{
				playedRunStart = false;
			}
			break;
		case HeroSounds.FOOTSTEPS_SPRINT:
			canPlaySprint = false;
			break;
		case HeroSounds.FOOTSTEPS_WALK:
			canPlayWalk = false;
			if (resetStarts)
			{
				playedWalkStart = false;
			}
			break;
		case HeroSounds.WALLSLIDE:
			wallslide.Stop();
			heroVibrationCtrl.StopWallSlide();
			break;
		case HeroSounds.NAIL_ART_CHARGE:
			nailArtCharge.Stop();
			break;
		case HeroSounds.NAIL_ART_READY:
			nailArtReady.Stop();
			break;
		case HeroSounds.FALLING:
			falling.Stop();
			if (fallingCo != null)
			{
				StopCoroutine(fallingCo);
			}
			break;
		case HeroSounds.UPDRAFT_IDLE:
			updraftIdle.Stop();
			break;
		case HeroSounds.WINDY_IDLE:
			windyIdle.Stop();
			break;
		case HeroSounds.JUMP:
		case HeroSounds.WALLJUMP:
		case HeroSounds.SOFT_LANDING:
		case HeroSounds.HARD_LANDING:
		case HeroSounds.BACKDASH:
		case HeroSounds.DASH:
		case HeroSounds.TAKE_HIT:
		case HeroSounds.DASH_SILK:
			break;
		}
	}

	public void StopAllSounds()
	{
		if ((bool)heroVibrationCtrl)
		{
			heroVibrationCtrl.StopAllVibrations();
		}
		softLanding.Stop();
		hardLanding.Stop();
		jump.Stop();
		takeHit.Stop();
		backDash.Stop();
		dash.Stop();
		dashSilk.Stop();
		footSteps.Stop();
		playedRunStart = false;
		wallslide.Stop();
		nailArtCharge.Stop();
		nailArtReady.Stop();
		falling.Stop();
	}

	public void PauseAllSounds()
	{
		softLanding.Pause();
		hardLanding.Pause();
		jump.Pause();
		takeHit.Pause();
		backDash.Pause();
		dash.Pause();
		dashSilk.Pause();
		footSteps.Pause();
		wallslide.Pause();
		nailArtCharge.Pause();
		nailArtReady.Pause();
		falling.Pause();
		BlockFootstepAudio = true;
	}

	public void UnPauseAllSounds()
	{
		softLanding.UnPause();
		hardLanding.UnPause();
		jump.UnPause();
		takeHit.UnPause();
		backDash.UnPause();
		dash.UnPause();
		dashSilk.UnPause();
		footSteps.UnPause();
		wallslide.UnPause();
		nailArtCharge.UnPause();
		nailArtReady.UnPause();
		falling.UnPause();
		BlockFootstepAudio = false;
	}

	public void SetFootstepsTable(RandomAudioClipTable newTable)
	{
		footstepsTable = newTable;
	}

	public void PlayFootstep()
	{
		if (CanPlayFootsteps && !BlockFootstepAudio)
		{
			footstepsTable.PlayOneShot(footSteps);
			this.OnPlayFootstep?.Invoke();
			if (canPlaySprint)
			{
				heroVibrationCtrl.PlayFootStep();
			}
		}
	}

	public void AllowFootstepsGrace()
	{
		canPlayTime = Time.timeAsDouble + 0.10000000149011612;
	}

	private void RandomizePitch(AudioSource src, float minPitch, float maxPitch)
	{
		float pitch = UnityEngine.Random.Range(minPitch, maxPitch);
		src.pitch = pitch;
	}

	private void ResetPitch(AudioSource src)
	{
		src.pitch = 1f;
	}

	private IEnumerator FadeInVolume(AudioSource src, float duration)
	{
		float elapsedTime = 0f;
		src.volume = 0f;
		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			src.volume = Mathf.Lerp(0f, 1f, t);
			yield return null;
		}
	}
}
