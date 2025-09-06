using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DamageHeroDirectly : FsmStateAction
	{
		public FsmOwnerDefault damager;

		public FsmInt damageAmount;

		public bool spikeHazard;

		public bool sinkHazard;

		public override void Reset()
		{
			damager = null;
			damageAmount = null;
			spikeHazard = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(damager);
			if (ownerDefaultTarget == null)
			{
				Finish();
				return;
			}
			HeroController.instance.CancelDownspikeInvulnerability();
			PlayerData.instance.isInvincible = false;
			HeroController.instance.cState.parrying = false;
			HazardType hazardType = HazardType.ENEMY;
			if (spikeHazard)
			{
				hazardType = HazardType.SPIKES;
			}
			else if (sinkHazard)
			{
				hazardType = HazardType.SINK;
			}
			if (ownerDefaultTarget.transform.position.x > HeroController.instance.gameObject.transform.position.x)
			{
				HeroController.instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, damageAmount.Value, hazardType);
			}
			else
			{
				HeroController.instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, damageAmount.Value, hazardType);
			}
			Finish();
		}
	}
}
