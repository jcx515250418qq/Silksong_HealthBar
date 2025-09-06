using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class ReceivedDamageBase : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		public FsmEvent sendEventHeavy;

		public FsmEvent sendEventSpikes;

		public FsmEvent sendEventLava;

		public FsmEvent sendEventLightning;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeGameObject;

		public FsmBool ignoreAcid;

		public FsmBool ignoreLava;

		public FsmBool ignoreWater;

		public FsmBool ignoreHunterWeapon;

		public FsmBool ignoreTraps;

		public FsmBool ignoreNail;

		public FsmBool ignoreSpikes;

		[UIHint(UIHint.Variable)]
		public FsmInt storeDamageDealt;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeDirection;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeMagnitudeMultiplier;

		public FsmBool firstHitOnly;

		private ReceivedDamageProxy proxy;

		public override void Reset()
		{
			Target = null;
			collideTag = new FsmString
			{
				UseVariable = true
			};
			sendEvent = null;
			sendEventHeavy = null;
			sendEventSpikes = null;
			sendEventLightning = null;
			storeGameObject = null;
			ignoreAcid = null;
			ignoreLava = null;
			ignoreWater = null;
			ignoreHunterWeapon = null;
			ignoreTraps = null;
			ignoreNail = null;
			ignoreSpikes = null;
			storeDamageDealt = null;
			storeDirection = null;
			storeMagnitudeMultiplier = null;
			firstHitOnly = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			proxy = safe.GetComponent<ReceivedDamageProxy>() ?? safe.AddComponent<ReceivedDamageProxy>();
			proxy.AddHandler(this);
		}

		public override void OnExit()
		{
			proxy.RemoveHandler(this);
		}

		public bool CanRespondToHit(HitInstance damageInstance)
		{
			AttackTypes attackType = damageInstance.AttackType;
			GameObject source = damageInstance.Source;
			if (!source)
			{
				return false;
			}
			if (firstHitOnly.Value && !damageInstance.IsFirstHit)
			{
				return false;
			}
			if (!collideTag.IsNone && !string.IsNullOrEmpty(collideTag.Value) && !source.CompareTag(collideTag.Value))
			{
				return false;
			}
			if ((ignoreHunterWeapon.Value && attackType == AttackTypes.Hunter) || (ignoreTraps.Value && attackType == AttackTypes.Trap) || (ignoreLava.Value && attackType == AttackTypes.Lava) || (ignoreNail.Value && attackType == AttackTypes.Nail) || (ignoreSpikes.Value && attackType == AttackTypes.Spikes) || (ignoreAcid.Value && source.CompareTag("Acid")) || (ignoreWater.Value && source.CompareTag("Water Surface")))
			{
				return false;
			}
			if (!CustomCanRespond(damageInstance))
			{
				return false;
			}
			return true;
		}

		public virtual bool CustomCanRespond(HitInstance damageInstance)
		{
			if (damageInstance.DamageDealt > 0)
			{
				if (damageInstance.IsManualTrigger)
				{
					return damageInstance.IsNailDamage;
				}
				return true;
			}
			return false;
		}

		public bool RespondToHit(HitInstance damageInstance)
		{
			if (!CanRespondToHit(damageInstance))
			{
				return false;
			}
			AttackTypes attackType = damageInstance.AttackType;
			GameObject source = damageInstance.Source;
			storeGameObject.Value = source;
			storeDamageDealt.Value = damageInstance.DamageDealt;
			storeDirection.Value = damageInstance.Direction;
			storeMagnitudeMultiplier.Value = damageInstance.MagnitudeMultiplier;
			bool flag = false;
			switch (attackType)
			{
			case AttackTypes.Spell:
			case AttackTypes.Heavy:
				base.Fsm.Event(sendEventHeavy);
				break;
			case AttackTypes.Spikes:
				base.Fsm.Event(sendEventSpikes);
				break;
			case AttackTypes.Lava:
				base.Fsm.Event(sendEventLava);
				break;
			case AttackTypes.Lightning:
				flag = true;
				break;
			}
			if (damageInstance.ZapDamageTicks > 0)
			{
				flag = true;
			}
			if (flag)
			{
				base.Fsm.Event(sendEventLightning);
			}
			base.Fsm.Event(sendEvent);
			return true;
		}

		public override string ErrorCheck()
		{
			string text = string.Empty;
			GameObject safe = Target.GetSafe(this);
			if (safe != null && safe.GetComponent<Collider2D>() == null && safe.GetComponent<Rigidbody2D>() == null)
			{
				text += "Target requires a RigidBody2D or Collider2D!\n";
			}
			return text;
		}
	}
}
