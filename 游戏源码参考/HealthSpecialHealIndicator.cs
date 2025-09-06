using System;
using HutongGames.PlayMaker;
using UnityEngine;

public class HealthSpecialHealIndicator : MonoBehaviour
{
	private enum HealCapTypes
	{
		Warrior = 0,
		Witch = 1
	}

	[SerializeField]
	private EventRegister appearEvent;

	[SerializeField]
	private EventRegister[] updateEvents;

	[SerializeField]
	private EventRegister disappearEvent;

	[SerializeField]
	private HealCapTypes healCapType;

	[SerializeField]
	private string upAnim;

	[SerializeField]
	private string downAnim;

	private MeshRenderer meshRenderer;

	private tk2dSpriteAnimator animator;

	private int healthNumber;

	private HeroController hc;

	private bool isStateActive;

	private bool isUp;

	private int frozenHealTarget;

	private int CurrentHealCap => healCapType switch
	{
		HealCapTypes.Warrior => hc.GetRageModeHealCap() - hc.WarriorState.RageModeHealCount, 
		HealCapTypes.Witch => hc.GetWitchHealCap(), 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	private bool ShouldBeUp
	{
		get
		{
			if (!isStateActive)
			{
				return false;
			}
			PlayerData playerData = hc.playerData;
			int health = playerData.health;
			int currentMaxHealth = playerData.CurrentMaxHealth;
			if (healthNumber <= health || healthNumber > currentMaxHealth)
			{
				return false;
			}
			int num = healCapType switch
			{
				HealCapTypes.Warrior => health + CurrentHealCap, 
				HealCapTypes.Witch => frozenHealTarget, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			return healthNumber <= num;
		}
	}

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.enabled = false;
		animator = GetComponent<tk2dSpriteAnimator>();
		tk2dSpriteAnimator obj = animator;
		obj.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(obj.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
		FsmInt fsmInt = FSMUtility.LocateFSM(base.transform.parent.gameObject, "health_display").FsmVariables.FindFsmInt("Health Number");
		healthNumber = fsmInt.Value;
		hc = HeroController.instance;
		hc.OnTakenDamage += OnUpdateEvent;
		appearEvent.ReceivedEvent += delegate
		{
			if (healCapType != HealCapTypes.Witch || !hc.cState.isMaggoted)
			{
				isStateActive = true;
				frozenHealTarget = hc.playerData.health + CurrentHealCap;
				OnUpdateEvent();
			}
		};
		EventRegister[] array = updateEvents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ReceivedEvent += OnUpdateEvent;
		}
		disappearEvent.ReceivedEvent += delegate
		{
			isStateActive = false;
			OnUpdateEvent();
		};
	}

	private void OnDestroy()
	{
		if ((bool)hc)
		{
			hc.OnTakenDamage -= OnUpdateEvent;
			hc = null;
		}
	}

	private void OnUpdateEvent()
	{
		if (ShouldBeUp)
		{
			if (!isUp)
			{
				isUp = true;
				meshRenderer.enabled = true;
				animator.Play(upAnim);
			}
		}
		else if (isUp)
		{
			isUp = false;
			animator.Play(downAnim);
		}
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == downAnim)
		{
			meshRenderer.enabled = false;
		}
	}
}
