using System;
using System.Collections;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class BindOrbHudFrame : MonoBehaviour
{
	[Serializable]
	private class BasicFrameAnims
	{
		public string AppearFromNone;

		public string Appear;

		public string Idle;

		public string Disappear;

		public string ActivateEvent;
	}

	[Serializable]
	private class MeterAnims
	{
		public GameObject[] IncreaseEffects;
	}

	private delegate void FrameAnimEndDelegate(Action onFrameEnded);

	private delegate IEnumerator CoroutineFunction();

	[SerializeField]
	private BasicFrameAnims defaultFrameAnims;

	[SerializeField]
	private AudioEvent hudAppearAudio;

	[Header("Common")]
	[SerializeField]
	private float refreshDelay;

	[SerializeField]
	private ParticleSystem changeParticle;

	[SerializeField]
	private PlayMakerFSM activateEventsTarget;

	[SerializeField]
	private AudioEvent hudChangeAudio;

	[SerializeField]
	private Color lifebloodTint;

	[Header("Cloakless")]
	[SerializeField]
	private BasicFrameAnims cloaklessFrameAnims;

	[Header("Hunter")]
	[SerializeField]
	private BasicFrameAnims hunterV2FrameAnims;

	[SerializeField]
	private string hunterV2FullAnim;

	[SerializeField]
	private UiProgressBar hunterV2Bar;

	[SerializeField]
	private AudioEvent hunterV2ChargedAudio;

	[Space]
	[SerializeField]
	private BasicFrameAnims hunterV3FrameAnims;

	[SerializeField]
	private string hunterV3FullAnimA;

	[SerializeField]
	private string hunterV3FullAnimB;

	[SerializeField]
	private UiProgressBar hunterV3BarA;

	[SerializeField]
	private UiProgressBar hunterV3BarB;

	[SerializeField]
	private GameObject hunterV3ExtraHitEffect;

	[SerializeField]
	private AudioEvent hunterV3ChargedAudio;

	[Header("Warrior")]
	[SerializeField]
	private BasicFrameAnims warriorFrameAnims;

	[SerializeField]
	private MeterAnims[] warriorMeterAnims;

	[SerializeField]
	private string warriorRageAnim;

	[SerializeField]
	private string warriorRageEndAnim;

	[Header("Reaper")]
	[SerializeField]
	private BasicFrameAnims reaperFrameAnims;

	[SerializeField]
	private string reaperModeBeginAnim;

	[SerializeField]
	private string reaperModeEndAnim;

	[SerializeField]
	private NestedFadeGroupBase reaperModeEffect;

	[SerializeField]
	private float reaperModeEffectFadeOutTime;

	[Header("Wanderer")]
	[SerializeField]
	private BasicFrameAnims wandererFrameAnims;

	[SerializeField]
	private string wandererFullAnim;

	[SerializeField]
	private string wandererFullEndAnim;

	[SerializeField]
	private AudioEvent wandererHarpAppearAudio;

	[SerializeField]
	private AudioEvent wandererHarpDisappearAudio;

	[Header("Witch")]
	[SerializeField]
	private AudioEvent hudChangeCursedAudio;

	[SerializeField]
	private BasicFrameAnims cursedV1FrameAnims;

	[SerializeField]
	private BasicFrameAnims witchFrameAnims;

	[Header("Toolmaster")]
	[SerializeField]
	private BasicFrameAnims toolmasterFrameAnims;

	[SerializeField]
	private string toolmasterSilkGetAnim;

	[Header("Spell")]
	[SerializeField]
	private BasicFrameAnims spellFrameAnims;

	private bool queuedToolmasterSpin;

	private bool isActive;

	private bool isCursed;

	private BasicFrameAnims currentFrameAnims;

	private ToolCrest currentFrameCrest;

	private Coroutine animRoutine;

	private FrameAnimEndDelegate onEndFrameAnim;

	private tk2dSpriteAnimator animator;

	private SteelSoulAnimProxy animProxy;

	public static bool SkipToNextAppear { get; set; }

	public static bool ForceNextInstant { get; set; }

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		animProxy = GetComponent<SteelSoulAnimProxy>();
		EventRegister.GetRegisterGuaranteed(base.gameObject, "POST TOOL EQUIPS CHANGED").ReceivedEvent += delegate
		{
			Refresh(isInstant: false, isFirst: false);
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOLMASTER QUICK CRAFTING").ReceivedEvent += delegate
		{
			queuedToolmasterSpin = true;
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HEALTH UPDATE").ReceivedEvent += RefreshLifebloodTint;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "CHARM INDICATOR CHECK").ReceivedEvent += RefreshLifebloodTint;
	}

	private void OnEnable()
	{
		MeterAnims[] array = warriorMeterAnims;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject[] increaseEffects = array[i].IncreaseEffects;
			foreach (GameObject gameObject in increaseEffects)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(value: false);
				}
			}
		}
		hunterV2Bar.gameObject.SetActive(value: false);
		hunterV3BarA.gameObject.SetActive(value: false);
		hunterV3BarB.gameObject.SetActive(value: false);
		hunterV3ExtraHitEffect.SetActive(value: false);
	}

	private void OnDisable()
	{
		if (animRoutine != null)
		{
			StopCoroutine(animRoutine);
			animRoutine = null;
		}
		isActive = false;
	}

	public void FirstAppear()
	{
		if (!isActive && animRoutine == null)
		{
			isActive = true;
			Refresh(isInstant: false, isFirst: true);
		}
	}

	public void AlreadyAppeared()
	{
		if (!isActive && animRoutine == null)
		{
			isActive = true;
			Refresh(isInstant: true, isFirst: false);
		}
	}

	public void Disappeared()
	{
		isActive = false;
		if (animRoutine != null)
		{
			StopCoroutine(animRoutine);
			animRoutine = null;
		}
	}

	private void Refresh(bool isInstant, bool isFirst)
	{
		if (base.isActiveAndEnabled && !DoChangeFrame(isInstant, isFirst))
		{
			ChangeEnded();
		}
	}

	private void RefreshLifebloodTint()
	{
		tk2dSprite tk2dSprite2 = animator.Sprite as tk2dSprite;
		if ((bool)tk2dSprite2)
		{
			if (HeroController.instance.IsInLifebloodState)
			{
				tk2dSprite2.color = lifebloodTint;
				tk2dSprite2.EnableKeyword("RECOLOUR");
			}
			else
			{
				animator.Sprite.color = Color.white;
				tk2dSprite2.DisableKeyword("RECOLOUR");
			}
		}
	}

	private bool DoChangeFrame(bool isInstant, bool isFirst)
	{
		if (!isActive && !isFirst)
		{
			return false;
		}
		if (!isInstant && ForceNextInstant)
		{
			isInstant = true;
		}
		ToolCrest hunterCrest = Gameplay.HunterCrest;
		ToolCrest hunterCrest2 = Gameplay.HunterCrest2;
		ToolCrest hunterCrest3 = Gameplay.HunterCrest3;
		ToolCrest cloaklessCrest = Gameplay.CloaklessCrest;
		ToolCrest warriorCrest = Gameplay.WarriorCrest;
		ToolCrest reaperCrest = Gameplay.ReaperCrest;
		ToolCrest wandererCrest = Gameplay.WandererCrest;
		ToolCrest cursedCrest = Gameplay.CursedCrest;
		ToolCrest witchCrest = Gameplay.WitchCrest;
		ToolCrest toolmasterCrest = Gameplay.ToolmasterCrest;
		ToolCrest spellCrest = Gameplay.SpellCrest;
		BasicFrameAnims newFrameAnims = null;
		CoroutineFunction customAnimRoutine = null;
		isCursed = false;
		if (hunterCrest.IsEquipped)
		{
			if (currentFrameCrest == hunterCrest)
			{
				return false;
			}
			currentFrameCrest = hunterCrest;
			newFrameAnims = defaultFrameAnims;
		}
		else if (hunterCrest2.IsEquipped)
		{
			if (currentFrameCrest == hunterCrest2)
			{
				return false;
			}
			currentFrameCrest = hunterCrest2;
			newFrameAnims = hunterV2FrameAnims;
			customAnimRoutine = HunterCrestV2Routine;
		}
		else if (hunterCrest3.IsEquipped)
		{
			if (currentFrameCrest == hunterCrest3)
			{
				return false;
			}
			currentFrameCrest = hunterCrest3;
			newFrameAnims = hunterV3FrameAnims;
			customAnimRoutine = HunterCrestV3Routine;
		}
		else if (cloaklessCrest.IsEquipped)
		{
			if (currentFrameCrest == cloaklessCrest)
			{
				return false;
			}
			currentFrameCrest = cloaklessCrest;
			newFrameAnims = cloaklessFrameAnims;
		}
		else if (warriorCrest.IsEquipped)
		{
			if (currentFrameCrest == warriorCrest)
			{
				return false;
			}
			currentFrameCrest = warriorCrest;
			newFrameAnims = warriorFrameAnims;
			customAnimRoutine = WarriorCrestRoutine;
		}
		else if (reaperCrest.IsEquipped)
		{
			if (currentFrameCrest == reaperCrest)
			{
				return false;
			}
			currentFrameCrest = reaperCrest;
			newFrameAnims = reaperFrameAnims;
			customAnimRoutine = ReaperCrestRoutine;
		}
		else if (wandererCrest.IsEquipped)
		{
			if (currentFrameCrest == wandererCrest)
			{
				return false;
			}
			currentFrameCrest = wandererCrest;
			newFrameAnims = wandererFrameAnims;
			customAnimRoutine = WandererCrestRoutine;
		}
		else if (cursedCrest.IsEquipped)
		{
			if (!isFirst && currentFrameCrest == cursedCrest)
			{
				return false;
			}
			currentFrameCrest = cursedCrest;
			newFrameAnims = cursedV1FrameAnims;
			isCursed = true;
		}
		else if (witchCrest.IsEquipped)
		{
			if (currentFrameCrest == witchCrest)
			{
				return false;
			}
			currentFrameCrest = witchCrest;
			newFrameAnims = witchFrameAnims;
		}
		else if (toolmasterCrest.IsEquipped)
		{
			if (currentFrameCrest == toolmasterCrest)
			{
				return false;
			}
			currentFrameCrest = toolmasterCrest;
			newFrameAnims = toolmasterFrameAnims;
			customAnimRoutine = ToolmasterCrestRoutine;
		}
		else if (spellCrest.IsEquipped)
		{
			if (currentFrameCrest == spellCrest)
			{
				return false;
			}
			currentFrameCrest = spellCrest;
			newFrameAnims = spellFrameAnims;
		}
		else
		{
			if (!isFirst && currentFrameCrest == null)
			{
				return false;
			}
			currentFrameCrest = null;
			newFrameAnims = defaultFrameAnims;
		}
		if ((bool)activateEventsTarget)
		{
			activateEventsTarget.enabled = true;
			activateEventsTarget.SendEvent("DEACTIVATE");
		}
		BasicFrameAnims basicFrameAnims = currentFrameAnims;
		currentFrameAnims = newFrameAnims;
		if (!isFirst && basicFrameAnims != null && currentFrameAnims != null && basicFrameAnims.Idle == currentFrameAnims.Idle && customAnimRoutine == null)
		{
			return false;
		}
		if (animRoutine != null)
		{
			StopCoroutine(animRoutine);
		}
		if (isInstant || isFirst)
		{
			StartNextFrameAnims();
		}
		else if (onEndFrameAnim != null)
		{
			onEndFrameAnim(StartNextFrameAnims);
		}
		else
		{
			animRoutine = StartCoroutine(FrameDisappear(null, StartNextFrameAnims));
		}
		return true;
		void StartNextFrameAnims()
		{
			hunterV2Bar.gameObject.SetActive(value: false);
			hunterV3BarA.gameObject.SetActive(value: false);
			hunterV3BarB.gameObject.SetActive(value: false);
			if (newFrameAnims != null)
			{
				if (isInstant)
				{
					PlayFrameAnim(newFrameAnims.Idle);
					onEndFrameAnim = null;
					ChangeEnded();
					if (customAnimRoutine != null)
					{
						animRoutine = StartCoroutine(customAnimRoutine());
					}
				}
				else
				{
					onEndFrameAnim = delegate(Action nextHandler)
					{
						animRoutine = StartCoroutine(FrameDisappear(newFrameAnims, nextHandler));
					};
					animRoutine = StartCoroutine(FrameAppear(newFrameAnims, customAnimRoutine, isFirst));
				}
			}
			else
			{
				PlayFrameAnim(defaultFrameAnims.Idle);
				onEndFrameAnim = null;
				ChangeEnded();
			}
		}
	}

	private IEnumerator FrameAppear(BasicFrameAnims frameAnims, CoroutineFunction customAnimRoutine, bool isFirst)
	{
		string text;
		if (isFirst)
		{
			text = ((!string.IsNullOrEmpty(frameAnims.AppearFromNone)) ? frameAnims.AppearFromNone : frameAnims.Appear);
			if (GameCameras.instance.IsHudVisible)
			{
				(Gameplay.CursedCrest.IsEquipped ? hudChangeCursedAudio : hudAppearAudio).SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			}
		}
		else
		{
			text = frameAnims.Appear;
		}
		if (!string.IsNullOrEmpty(text))
		{
			tk2dSpriteAnimationClip clip = animProxy.GetClip(text);
			if (clip != null)
			{
				float seconds = animator.PlayAnimGetTime(clip);
				animator.PlayFromFrame(0);
				yield return new WaitForSeconds(seconds);
			}
		}
		PlayFrameAnim(frameAnims.Idle);
		if (isFirst)
		{
			isActive = true;
		}
		ChangeEnded();
		if (customAnimRoutine != null)
		{
			animRoutine = StartCoroutine(customAnimRoutine());
		}
	}

	private IEnumerator FrameDisappear(BasicFrameAnims frameAnims, Action startNextFrameAnims)
	{
		if (SkipToNextAppear)
		{
			PlayChangeEffects();
		}
		else
		{
			if (refreshDelay > 0f)
			{
				yield return new WaitForSeconds(refreshDelay);
			}
			if (!currentFrameCrest)
			{
				PlayChangeEffects();
			}
			if (frameAnims != null && !string.IsNullOrEmpty(frameAnims.Disappear))
			{
				tk2dSpriteAnimationClip clip = animProxy.GetClip(frameAnims.Disappear);
				if (clip != null)
				{
					float seconds = animator.PlayAnimGetTime(clip);
					animator.PlayFromFrame(0);
					yield return new WaitForSeconds(seconds);
				}
			}
			if ((bool)currentFrameCrest)
			{
				PlayChangeEffects();
			}
		}
		startNextFrameAnims();
	}

	private void PlayChangeEffects()
	{
		if ((bool)changeParticle)
		{
			if (changeParticle.IsAlive(withChildren: true))
			{
				changeParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			changeParticle.Play(withChildren: true);
		}
		if (GameCameras.instance.IsHudVisible)
		{
			(isCursed ? hudChangeCursedAudio : hudChangeAudio).SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		}
	}

	private void PlayFrameAnim(string animName, int frame = 0)
	{
		if (!string.IsNullOrEmpty(animName))
		{
			tk2dSpriteAnimationClip clip = animProxy.GetClip(animName);
			if (clip != null)
			{
				animator.PlayFromFrame(clip, frame);
			}
			RefreshLifebloodTint();
		}
	}

	private IEnumerator WarriorCrestRoutine()
	{
		HeroController hc = HeroController.instance;
		bool wasInRageMode = false;
		while (true)
		{
			if (hc.IsPaused())
			{
				yield return null;
				continue;
			}
			HeroController.WarriorCrestStateInfo warriorState = hc.WarriorState;
			if (warriorState.IsInRageMode)
			{
				if (!wasInRageMode)
				{
					PlayFrameAnim(warriorRageAnim);
				}
			}
			else if (wasInRageMode)
			{
				PlayFrameAnim(warriorRageEndAnim);
			}
			wasInRageMode = warriorState.IsInRageMode;
			yield return null;
		}
	}

	private IEnumerator ReaperCrestRoutine()
	{
		HeroController hc = HeroController.instance;
		bool wasInReaperMode = false;
		float reaperEffectTimeLeft = 0f;
		while (true)
		{
			if (hc.IsPaused())
			{
				yield return null;
				continue;
			}
			HeroController.ReaperCrestStateInfo reaperState = hc.ReaperState;
			if (reaperState.IsInReaperMode)
			{
				if (!wasInReaperMode)
				{
					PlayFrameAnim(reaperModeBeginAnim);
					if ((bool)reaperModeEffect)
					{
						reaperModeEffect.gameObject.SetActive(value: false);
						reaperModeEffect.gameObject.SetActive(value: true);
						reaperModeEffect.AlphaSelf = 1f;
						reaperEffectTimeLeft = 0f;
					}
				}
			}
			else if (wasInReaperMode)
			{
				PlayFrameAnim(reaperModeEndAnim);
				if ((bool)reaperModeEffect)
				{
					reaperEffectTimeLeft = reaperModeEffect.FadeTo(0f, reaperModeEffectFadeOutTime);
				}
			}
			if (reaperEffectTimeLeft > 0f)
			{
				reaperEffectTimeLeft -= Time.deltaTime;
				if (reaperEffectTimeLeft <= 0f)
				{
					reaperModeEffect.gameObject.SetActive(value: false);
				}
			}
			wasInReaperMode = reaperState.IsInReaperMode;
			yield return null;
		}
	}

	private IEnumerator WandererCrestRoutine()
	{
		HeroController hc = HeroController.instance;
		bool wasLucky = false;
		while (true)
		{
			if (hc.IsPaused())
			{
				yield return null;
				continue;
			}
			bool isWandererLucky = hc.IsWandererLucky;
			if (isWandererLucky && !wasLucky)
			{
				PlayFrameAnim(wandererFullAnim);
				if (!hc.IsRefillSoundsSuppressed && HudCanvas.IsVisible && ScreenFaderState.Alpha < 0.5f)
				{
					wandererHarpAppearAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
				}
			}
			else if (!isWandererLucky && wasLucky)
			{
				PlayFrameAnim(wandererFullEndAnim);
				if (!hc.IsRefillSoundsSuppressed && HudCanvas.IsVisible && ScreenFaderState.Alpha < 0.5f)
				{
					wandererHarpDisappearAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
				}
			}
			wasLucky = isWandererLucky;
			yield return null;
		}
	}

	private IEnumerator ToolmasterCrestRoutine()
	{
		PlayerData pd = PlayerData.instance;
		HeroController hc = HeroController.instance;
		bool couldBind = (float)pd.silk >= SilkSpool.BindCost;
		while (true)
		{
			if (hc.IsPaused())
			{
				yield return null;
				continue;
			}
			bool flag = (float)pd.silk >= SilkSpool.BindCost;
			if (flag != couldBind || queuedToolmasterSpin)
			{
				PlayFrameAnim(toolmasterSilkGetAnim);
			}
			couldBind = flag;
			queuedToolmasterSpin = false;
			yield return null;
		}
	}

	private IEnumerator HunterCrestV2Routine()
	{
		return HunterCrestUpgradedRoutine(Gameplay.HunterComboHits, 0, hunterV2Bar, null, hunterV2FullAnim, null, hunterV2FrameAnims.Idle);
	}

	private IEnumerator HunterCrestV3Routine()
	{
		return HunterCrestUpgradedRoutine(Gameplay.HunterCombo2Hits, Gameplay.HunterCombo2ExtraHits, hunterV3BarA, hunterV3BarB, hunterV3FullAnimA, hunterV3FullAnimB, hunterV3FrameAnims.Idle);
	}

	private IEnumerator HunterCrestUpgradedRoutine(int maxHits, int extraMaxHits, UiProgressBar bar, UiProgressBar extraBar, string fullAnimA, string fullAnimB, string idleAnim)
	{
		HeroController hc = HeroController.instance;
		bar.Value = 0f;
		bar.gameObject.SetActive(value: true);
		if (extraBar != null)
		{
			extraBar.Value = 0f;
			extraBar.gameObject.SetActive(value: true);
		}
		int previousHits = -1;
		bool wasFull = false;
		bool wasFullExtra = false;
		while (true)
		{
			if (hc.IsPaused())
			{
				yield return null;
				continue;
			}
			HeroController.HunterUpgCrestStateInfo hunterUpgState = hc.HunterUpgState;
			bool flag = hunterUpgState.CurrentMeterHits >= maxHits;
			int num = hunterUpgState.CurrentMeterHits - maxHits;
			bool flag2 = extraMaxHits > 0 && num >= extraMaxHits;
			if (num > 0 && extraMaxHits > 0 && hunterUpgState.CurrentMeterHits != previousHits)
			{
				hunterV3ExtraHitEffect.SetActive(value: false);
				hunterV3ExtraHitEffect.SetActive(value: true);
			}
			if (flag)
			{
				if (!wasFull)
				{
					bar.gameObject.SetActive(value: false);
					PlayFrameAnim(fullAnimA);
					hunterV2ChargedAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
				}
				if (flag2)
				{
					if (!wasFullExtra)
					{
						if ((bool)extraBar)
						{
							extraBar.gameObject.SetActive(value: false);
						}
						PlayFrameAnim(fullAnimB);
						hunterV3ChargedAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
					}
				}
				else if (extraBar != null)
				{
					if (!extraBar.gameObject.activeSelf)
					{
						extraBar.gameObject.SetActive(value: true);
					}
					if (hunterUpgState.CurrentMeterHits != previousHits)
					{
						extraBar.Value = (float)num / (float)extraMaxHits;
					}
				}
			}
			else
			{
				if (wasFull)
				{
					PlayFrameAnim(idleAnim);
					if (extraBar != null)
					{
						extraBar.SetValueInstant(0f);
					}
				}
				if (hunterUpgState.CurrentMeterHits > previousHits)
				{
					bar.Value = (float)hunterUpgState.CurrentMeterHits / (float)maxHits;
				}
				else if (hunterUpgState.CurrentMeterHits < previousHits)
				{
					bar.SetValueInstant(0f);
				}
				if (wasFull)
				{
					bar.gameObject.SetActive(value: true);
				}
			}
			previousHits = hunterUpgState.CurrentMeterHits;
			wasFull = flag;
			wasFullExtra = flag2;
			yield return null;
		}
	}

	private void ChangeEnded()
	{
		EventRegister.SendEvent(EventRegisterEvents.HudFrameChanged);
		if ((bool)activateEventsTarget && currentFrameAnims != null && !string.IsNullOrEmpty(currentFrameAnims.ActivateEvent))
		{
			activateEventsTarget.enabled = true;
			activateEventsTarget.SendEvent(currentFrameAnims.ActivateEvent);
		}
	}
}
