using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetHeroAttackObject : FsmStateAction
	{
		public enum AttackObjects
		{
			NormalSlash = 0,
			AlternateSlash = 1,
			UpSlash = 2,
			DownSlash = 3,
			WallSlash = 4,
			DashStab = 5,
			DashStabAlt = 6,
			ChargeSlash = 7,
			TauntSlash = 9
		}

		public FsmOwnerDefault Target;

		[ObjectType(typeof(AttackObjects))]
		public FsmEnum Attack;

		public FsmGameObject StoreAttack;

		public override void Reset()
		{
			Target = null;
			Attack = null;
			StoreAttack = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				HeroController component = safe.GetComponent<HeroController>();
				if ((bool)component)
				{
					GameObject gameObject = GetGameObject(component);
					StoreAttack.Value = gameObject;
				}
			}
			Finish();
		}

		private GameObject GetGameObject(HeroController hc)
		{
			HeroController.ConfigGroup currentConfigGroup = hc.CurrentConfigGroup;
			if (currentConfigGroup == null)
			{
				return null;
			}
			return (AttackObjects)(object)Attack.Value switch
			{
				AttackObjects.NormalSlash => currentConfigGroup.NormalSlash.gameObject, 
				AttackObjects.AlternateSlash => currentConfigGroup.AlternateSlash.gameObject, 
				AttackObjects.UpSlash => currentConfigGroup.UpSlash.gameObject, 
				AttackObjects.DownSlash => currentConfigGroup.DownSlash.gameObject, 
				AttackObjects.WallSlash => currentConfigGroup.WallSlash.gameObject, 
				AttackObjects.DashStab => currentConfigGroup.DashStab, 
				AttackObjects.DashStabAlt => currentConfigGroup.DashStabAlt, 
				AttackObjects.ChargeSlash => currentConfigGroup.ChargeSlash, 
				AttackObjects.TauntSlash => currentConfigGroup.TauntSlash, 
				_ => throw new NotImplementedException(), 
			};
		}
	}
}
