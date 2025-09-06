using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class DamageHeroDirectlyV2 : FsmStateAction
	{
		public enum DamageDirection
		{
			FromDamageSource = 0,
			FromHeroFacingDirection = 1
		}

		public FsmOwnerDefault damager;

		public FsmInt damageAmount;

		[ObjectType(typeof(HazardType))]
		public FsmEnum hazardType;

		[ObjectType(typeof(DamageDirection))]
		public FsmEnum damageDirection;

		public FsmBool invertDirection;

		[ObjectType(typeof(CollisionSide))]
		public FsmEnum overrideDirection;

		public override void Reset()
		{
			damager = null;
			damageAmount = null;
			hazardType = null;
			damageDirection = null;
			invertDirection = null;
			overrideDirection = new FsmEnum
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(damager);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			HeroController instance = HeroController.instance;
			instance.CancelDownspikeInvulnerability();
			PlayerData.instance.isInvincible = false;
			instance.cState.parrying = false;
			HazardType hazardType = (HazardType)(object)this.hazardType.Value;
			if (overrideDirection.IsNone)
			{
				switch ((DamageDirection)(object)damageDirection.Value)
				{
				case DamageDirection.FromDamageSource:
					if (invertDirection.Value)
					{
						if (ownerDefaultTarget.transform.position.x < instance.gameObject.transform.position.x)
						{
							instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, damageAmount.Value, hazardType);
						}
						else
						{
							instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, damageAmount.Value, hazardType);
						}
					}
					else if (ownerDefaultTarget.transform.position.x > instance.gameObject.transform.position.x)
					{
						instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, damageAmount.Value, hazardType);
					}
					else
					{
						instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, damageAmount.Value, hazardType);
					}
					break;
				case DamageDirection.FromHeroFacingDirection:
					if (invertDirection.Value)
					{
						if (instance.transform.localScale.x > 0f)
						{
							instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, damageAmount.Value, hazardType);
						}
						else
						{
							instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, damageAmount.Value, hazardType);
						}
					}
					else if (instance.transform.localScale.x < 0f)
					{
						instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, damageAmount.Value, hazardType);
					}
					else
					{
						instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, damageAmount.Value, hazardType);
					}
					break;
				}
			}
			else
			{
				instance.TakeDamage(ownerDefaultTarget.gameObject, (CollisionSide)(object)overrideDirection.Value, damageAmount.Value, hazardType);
			}
			Finish();
		}
	}
}
