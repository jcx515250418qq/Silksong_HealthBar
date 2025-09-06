using UnityEngine;

public class HeroControllerConfigWarrior : HeroControllerConfig
{
	[Header("Warrior")]
	[SerializeField]
	private float rageAttackDuration;

	[SerializeField]
	private float rageAttackRecoveryTime;

	[SerializeField]
	private float rageAttackCooldownTime;

	[SerializeField]
	private float rageQuickAttackCooldownTime;

	public override float AttackDuration
	{
		get
		{
			if (!HeroController.instance.WarriorState.IsInRageMode)
			{
				return base.AttackDuration;
			}
			return rageAttackDuration;
		}
	}

	public override float AttackRecoveryTime
	{
		get
		{
			if (!HeroController.instance.WarriorState.IsInRageMode)
			{
				return base.AttackRecoveryTime;
			}
			return rageAttackRecoveryTime;
		}
	}

	public override float QuickAttackCooldownTime
	{
		get
		{
			if (!HeroController.instance.WarriorState.IsInRageMode)
			{
				return base.QuickAttackCooldownTime;
			}
			return rageQuickAttackCooldownTime;
		}
	}

	public override float AttackCooldownTime
	{
		get
		{
			if (!HeroController.instance.WarriorState.IsInRageMode)
			{
				return base.AttackCooldownTime;
			}
			return rageAttackCooldownTime;
		}
	}
}
