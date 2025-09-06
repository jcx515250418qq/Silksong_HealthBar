using System;
using UnityEngine;

public class DashStabNailAttack : NailAttackBase
{
	[Space]
	[SerializeField]
	private bool waitForBounceTrigger;

	[SerializeField]
	private bool waitForEndDamage;

	[SerializeField]
	private bool doEnemyHitRecoil;

	private bool isWaitingForTrigger;

	private bool isHitQueued;

	private tk2dSpriteAnimator animator;

	private HeroController heroController;

	private DamageEnemies damager;

	protected override void Awake()
	{
		base.Awake();
		heroController = GetComponentInParent<HeroController>();
		animator = GetComponent<tk2dSpriteAnimator>();
		if (waitForBounceTrigger)
		{
			tk2dSpriteAnimator obj = animator;
			obj.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(obj.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
		}
		damager = GetComponent<DamageEnemies>();
		if (waitForEndDamage)
		{
			damager.EndedDamage += OnHitTrigger;
		}
		damager.WillDamageEnemyOptions += OnWillDamageEnemy;
		damager.WillDamageEnemyCollider += OnWillDamageEnemyCollider;
	}

	private void OnWillDamageEnemy(HealthManager healthManager, HitInstance hitInstance)
	{
		if (!healthManager.GetComponent<NonBouncer>() || !healthManager.GetComponent<NonBouncer>().active)
		{
			DoRecoilHit();
		}
	}

	private void OnWillDamageEnemyCollider(Collider2D otherCollider)
	{
		if (otherCollider.CompareTag("Recoiler") && (bool)otherCollider.gameObject.GetComponent<IsCoralCrustWall>())
		{
			DoRecoilHit();
		}
	}

	private void DoRecoilHit()
	{
		if (doEnemyHitRecoil)
		{
			heroController.sprintFSM.SendEventSafe("DASH RECOIL");
		}
		DoHitResponse();
	}

	public override void OnSlashStarting()
	{
		base.OnSlashStarting();
		if (waitForBounceTrigger || waitForEndDamage)
		{
			isWaitingForTrigger = true;
		}
	}

	private void OnAnimationEventTriggered(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip, int frame)
	{
		OnHitTrigger(didHit: true);
	}

	private void DoHitResponse()
	{
		if (isWaitingForTrigger)
		{
			isHitQueued = true;
			return;
		}
		isHitQueued = false;
		heroController.sprintFSM.SendEventSafe("DASH HIT");
	}

	private void OnHitTrigger(bool didHit)
	{
		if (didHit)
		{
			isWaitingForTrigger = false;
			if (isHitQueued)
			{
				DoHitResponse();
			}
		}
	}
}
