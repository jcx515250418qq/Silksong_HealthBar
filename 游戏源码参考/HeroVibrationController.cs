using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public sealed class HeroVibrationController : MonoBehaviour
{
	[Serializable]
	private class AudioClipVibration
	{
		public AudioClip audioClip;

		public VibrationDataAsset vibrationDataAsset;
	}

	private class RampingVibration
	{
		public HeroVibrationController owner;

		public VibrationDataAsset vibrationDataAsset;

		public float duration;

		public float strength;

		public VibrationEmission emission;

		public Coroutine coroutine;

		public void StartVibration()
		{
			StopVibration();
			if (!(owner == null) && duration > 0f)
			{
				emission = VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset, null, isLooping: true);
				if (emission != null)
				{
					emission.SetStrength(0f);
					coroutine = owner.StartCoroutine(RampRoutine());
				}
			}
		}

		public void StopVibration()
		{
			emission?.Stop();
			emission = null;
			if (!(owner == null) && coroutine != null)
			{
				owner.StopCoroutine(coroutine);
				coroutine = null;
			}
		}

		private IEnumerator RampRoutine()
		{
			if (duration > 0f)
			{
				strength = 0f;
				if (emission == null)
				{
					coroutine = null;
					yield break;
				}
				float t = 0f;
				float multiplier = 1f / duration;
				while (t < 1f)
				{
					t += Time.deltaTime * multiplier;
					emission.SetStrength(Mathf.Lerp(0f, 1f, t));
					yield return null;
				}
			}
			emission.SetStrength(1f);
			coroutine = null;
		}
	}

	[SerializeField]
	private VibrationDataAsset softLandVibration;

	[SerializeField]
	private VibrationDataAsset footStepVibration;

	[SerializeField]
	private VibrationDataAsset wallJumpVibration;

	[SerializeField]
	private VibrationDataAsset dashVibration;

	[SerializeField]
	private VibrationDataAsset airDashVibration;

	[SerializeField]
	private VibrationDataAsset shadowDashVibration;

	[SerializeField]
	private VibrationDataAsset doubleJumpVibration;

	[SerializeField]
	private VibrationDataAsset heroDeathVibration;

	[SerializeField]
	private VibrationDataAsset heroHazardDeathVibration;

	[SerializeField]
	private VibrationDataAsset heroDamage;

	[SerializeField]
	private VibrationDataAsset swimEnter;

	[SerializeField]
	private VibrationDataAsset swimExit;

	[SerializeField]
	private VibrationDataAsset swimLoop;

	[SerializeField]
	private VibrationDataAsset swimLoopFast;

	[SerializeField]
	private VibrationDataAsset fastSwimStart;

	[SerializeField]
	private VibrationDataAsset toolThrowVibration;

	[Space]
	[SerializeField]
	private VibrationPlayer wallSlideVibrationPlayer;

	[SerializeField]
	private VibrationDataAsset shuttleCockJumpVibration;

	[Space]
	[SerializeField]
	private VibrationDataAsset rosaryCannonCharge;

	[SerializeField]
	private float rosaryCannonChargeDuration = 0.3f;

	[NonReorderable]
	[ArrayForEnum(typeof(HeroSounds))]
	[SerializeField]
	private VibrationDataAsset[] heroSoundVibrations = new VibrationDataAsset[0];

	[Space]
	[SerializeField]
	private List<AudioClipVibration> audioClipVibrations = new List<AudioClipVibration>();

	private VibrationEmission swimLoopEmission;

	private VibrationEmission swimLoopFastEmission;

	private VibrationEmission[] emissions;

	private Dictionary<AudioClip, AudioClipVibration> audioClipVibrationLookup = new Dictionary<AudioClip, AudioClipVibration>();

	private RampingVibration rosaryCannonVibration;

	private VibrationEmission shuttleClockEmission;

	private bool isSwimming;

	private bool isSwimSprint;

	private void Awake()
	{
		InitialiseVibrations();
		InitialiseLookup();
	}

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref heroSoundVibrations, typeof(HeroSounds));
	}

	private void InitialiseLookup()
	{
		audioClipVibrationLookup.Clear();
		for (int i = 0; i < audioClipVibrations.Count; i++)
		{
			AudioClipVibration audioClipVibration = audioClipVibrations[i];
			if (audioClipVibration.audioClip == null)
			{
				Debug.LogError($"Audio clip vibration element {i} is missing an audio clip.", this);
			}
			else if (audioClipVibration.vibrationDataAsset == null)
			{
				Debug.LogError($"Audio clip vibration element {i} is missing vibration data asset.", this);
			}
			else if (!audioClipVibrationLookup.TryAdd(audioClipVibration.audioClip, audioClipVibration))
			{
				Debug.LogError($"{audioClipVibration.audioClip} (i) multiple vibration data assets assigned.", this);
			}
		}
	}

	private void InitialiseVibrations()
	{
		emissions = new VibrationEmission[Enum.GetValues(typeof(HeroSounds)).Length];
	}

	public void PlayVibration(AudioSource audioSource, HeroSounds sounds, bool loop = false, float strength = 1f)
	{
		StopVibration(sounds);
		bool flag = false;
		if (loop && !audioSource.loop)
		{
			flag = true;
			audioSource.loop = true;
		}
		VibrationEmission vibrationEmission = null;
		AudioClipVibration value;
		if (TryGetHeroSoundClipVibration(sounds, out var vibrationDataAsset))
		{
			vibrationEmission = VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset.VibrationData, null, loop);
		}
		else if (audioClipVibrationLookup.TryGetValue(audioSource.clip, out value) && (bool)value.vibrationDataAsset)
		{
			vibrationEmission = VibrationManager.PlayVibrationClipOneShot(value.vibrationDataAsset, null, loop);
		}
		vibrationEmission?.SetStrength(strength);
		emissions[(int)sounds] = vibrationEmission;
		if (flag)
		{
			audioSource.loop = false;
		}
	}

	public void StopVibration(HeroSounds sounds)
	{
		emissions[(int)sounds]?.Stop();
	}

	public void StopAllVibrations()
	{
		for (int i = 0; i < emissions.Length; i++)
		{
			emissions[i]?.Stop();
			emissions[i] = null;
		}
	}

	private bool TryGetHeroSoundClipVibration(HeroSounds sound, out VibrationDataAsset vibrationDataAsset)
	{
		if (sound < HeroSounds.FOOTSTEPS_RUN || (int)sound >= heroSoundVibrations.Length)
		{
			vibrationDataAsset = null;
			return false;
		}
		vibrationDataAsset = heroSoundVibrations[(int)sound];
		return vibrationDataAsset != null;
	}

	public void PlaySoftLand()
	{
		VibrationManager.PlayVibrationClipOneShot(softLandVibration, null);
	}

	public void PlayFootStep()
	{
		VibrationManager.PlayVibrationClipOneShot(footStepVibration, null);
	}

	public void PlayWallJump()
	{
		VibrationManager.PlayVibrationClipOneShot(wallJumpVibration, null);
	}

	public void PlayDash()
	{
		VibrationManager.PlayVibrationClipOneShot(dashVibration, null);
	}

	public void PlayAirDash()
	{
		VibrationManager.PlayVibrationClipOneShot(airDashVibration, null);
	}

	public void PlayDoubleJump()
	{
		VibrationManager.PlayVibrationClipOneShot(doubleJumpVibration, null);
	}

	public void PlayShadowDash()
	{
		VibrationManager.PlayVibrationClipOneShot(shadowDashVibration, null);
	}

	public void StartWallSlide()
	{
		wallSlideVibrationPlayer.Play();
	}

	public void StopWallSlide()
	{
		wallSlideVibrationPlayer.Stop();
	}

	public void StartShuttlecock()
	{
		shuttleClockEmission = VibrationManager.PlayVibrationClipOneShot(shuttleCockJumpVibration, null);
	}

	public void StopShuttlecock()
	{
		shuttleClockEmission?.Stop();
		shuttleClockEmission = null;
	}

	public void PlayHeroDeath()
	{
		VibrationManager.PlayVibrationClipOneShot(heroDeathVibration, null);
	}

	public void PlayHeroHazardDeath()
	{
		VibrationManager.PlayVibrationClipOneShot(heroHazardDeathVibration, null);
	}

	public void PlayHeroDamage()
	{
	}

	public void PlaySwimEnter()
	{
		VibrationManager.PlayVibrationClipOneShot(swimEnter, null);
		swimLoopEmission?.Stop();
		swimLoopFastEmission?.Stop();
		swimLoopEmission = VibrationManager.PlayVibrationClipOneShot(swimLoop, null, isLooping: true);
		swimLoopFastEmission = VibrationManager.PlayVibrationClipOneShot(swimLoopFast, null, isLooping: true);
		isSwimming = false;
		isSwimSprint = false;
		UpdateSwimSpeed(isSwimming, isSwimSprint);
	}

	public void PlaySwimExit()
	{
		VibrationManager.PlayVibrationClipOneShot(swimExit, null);
		swimLoopEmission?.Stop();
		swimLoopFastEmission?.Stop();
		swimLoopEmission = null;
		swimLoopFastEmission = null;
	}

	public void PlayToolThrow()
	{
		VibrationManager.PlayVibrationClipOneShot(toolThrowVibration, null);
	}

	public void SetSwimAndSprint(bool swimming, bool isSprinting)
	{
		bool flag = false;
		if (isSwimming != swimming)
		{
			isSwimming = swimming;
			flag = true;
		}
		if (isSwimSprint != isSprinting)
		{
			isSwimSprint = isSprinting;
			flag = true;
		}
		if (flag)
		{
			UpdateSwimSpeed(isSwimming, isSwimSprint);
		}
	}

	public void SetSwimming(bool swimming)
	{
		if (isSwimming != swimming)
		{
			isSwimming = swimming;
			UpdateSwimSpeed(isSwimming, isSwimSprint);
		}
	}

	public void SetSwimSprint(bool isSprinting)
	{
		if (isSwimSprint != isSprinting)
		{
			isSwimSprint = isSprinting;
			UpdateSwimSpeed(isSwimming, isSwimSprint);
		}
	}

	private void UpdateSwimSpeed(bool isSwimming, bool isSprinting)
	{
		if (isSwimming)
		{
			if (isSprinting)
			{
				swimLoopEmission?.SetStrength(0f);
				swimLoopFastEmission?.SetStrength(1f);
				VibrationManager.PlayVibrationClipOneShot(fastSwimStart, null);
			}
			else
			{
				swimLoopEmission?.SetStrength(1f);
				swimLoopFastEmission?.SetStrength(0f);
			}
		}
		else
		{
			swimLoopEmission?.SetStrength(0f);
			swimLoopFastEmission?.SetStrength(0f);
		}
	}

	public void StartRosaryCannonCharge()
	{
		if (rosaryCannonVibration == null)
		{
			rosaryCannonVibration = new RampingVibration
			{
				duration = rosaryCannonChargeDuration,
				owner = this,
				vibrationDataAsset = rosaryCannonCharge
			};
		}
		rosaryCannonVibration.StartVibration();
	}

	public void StopRosaryCannonCharge()
	{
		rosaryCannonVibration?.StopVibration();
	}
}
