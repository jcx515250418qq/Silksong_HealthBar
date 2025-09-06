using System;
using UnityEngine;

[Serializable]
public struct HitInstance
{
	public enum HitDirection
	{
		Left = 0,
		Right = 1,
		Up = 2,
		Down = 3
	}

	public enum TargetType
	{
		Regular = 0,
		Corpse = 1,
		BouncePod = 2,
		Currency = 3
	}

	public GameObject Source;

	public bool IsFirstHit;

	public AttackTypes AttackType;

	public NailElements NailElement;

	public NailImbuementConfig NailImbuement;

	public bool IsUsingNeedleDamageMult;

	public ToolItem RepresentingTool;

	public int PoisonDamageTicks;

	public int ZapDamageTicks;

	public int DamageScalingLevel;

	public ToolDamageFlags ToolDamageFlags;

	public bool CircleDirection;

	public int DamageDealt;

	public float StunDamage;

	public bool CanWeakHit;

	public float Direction;

	public bool UseCorpseDirection;

	public float CorpseDirection;

	public bool CanTriggerBouncePod;

	public bool UseBouncePodDirection;

	public float BouncePodDirection;

	public float? ExtraUpDirection;

	public bool IgnoreInvulnerable;

	public float MagnitudeMultiplier;

	public bool UseCorpseMagnitudeMult;

	public float CorpseMagnitudeMultiplier;

	public bool UseCurrencyMagnitudeMult;

	public float CurrencyMagnitudeMult;

	public float MoveAngle;

	public bool MoveDirection;

	public float Multiplier;

	public SpecialTypes SpecialType;

	public GameObject[] SlashEffectOverrides;

	public EnemyHitEffectsProfile.EffectsTypes HitEffectsType;

	public HitSilkGeneration SilkGeneration;

	public bool NonLethal;

	public bool RageHit;

	public bool CriticalHit;

	public bool HunterCombo;

	public bool IsManualTrigger;

	public bool ForceNotWeakHit;

	public bool IsHeroDamage;

	public bool IsNailTag;

	public bool IgnoreNailPosition;

	public bool IsHarpoon;

	public bool IsNailDamage
	{
		get
		{
			if (!IsNailTag)
			{
				return AttackType == AttackTypes.Nail;
			}
			return true;
		}
	}

	public float GetActualDirection(Transform target, TargetType targetType)
	{
		if (Source != null && target != null && CircleDirection)
		{
			Vector2 vector = (Vector2)target.position - (Vector2)Source.transform.position;
			return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		}
		if (Source != null && target != null && MoveDirection)
		{
			Rigidbody2D component = Source.GetComponent<Rigidbody2D>();
			if (component == null)
			{
				component = Source.transform.parent.gameObject.GetComponent<Rigidbody2D>();
			}
			if (component == null)
			{
				return GetDirectionForType(targetType);
			}
			float num = 0f;
			float result = 0f;
			float result2 = 90f;
			if (component.linearVelocity.x < 0f)
			{
				result = 180f;
			}
			if (component.linearVelocity.y < 0f)
			{
				result2 = 270f;
			}
			if (Math.Abs(component.linearVelocity.x) > Math.Abs(component.linearVelocity.y))
			{
				return result;
			}
			return result2;
		}
		return GetDirectionForType(targetType);
	}

	public float GetDirectionForType(TargetType targetType)
	{
		switch (targetType)
		{
		case TargetType.Regular:
			return Direction;
		case TargetType.Corpse:
			if (!UseCorpseDirection)
			{
				return Direction;
			}
			return CorpseDirection;
		case TargetType.BouncePod:
			if (!CanTriggerBouncePod)
			{
				return Direction;
			}
			return 270f;
		case TargetType.Currency:
			return Direction;
		default:
			throw new ArgumentOutOfRangeException("targetType", targetType, null);
		}
	}

	public float GetMagnitudeMultForType(TargetType targetType)
	{
		switch (targetType)
		{
		case TargetType.Regular:
			return MagnitudeMultiplier;
		case TargetType.Corpse:
			if (!UseCorpseMagnitudeMult)
			{
				return MagnitudeMultiplier;
			}
			return CorpseMagnitudeMultiplier;
		case TargetType.Currency:
			if (!UseCurrencyMagnitudeMult)
			{
				return MagnitudeMultiplier;
			}
			return CurrencyMagnitudeMult;
		default:
			return MagnitudeMultiplier;
		}
	}

	public float GetOverriddenDirection(Transform target, TargetType targetType)
	{
		return ExtraUpDirection ?? GetActualDirection(target, targetType);
	}

	public HitDirection GetHitDirection(TargetType targetType)
	{
		float directionForType = GetDirectionForType(targetType);
		if (directionForType < 45f)
		{
			return HitDirection.Right;
		}
		if (directionForType < 135f)
		{
			return HitDirection.Up;
		}
		if (directionForType < 225f)
		{
			return HitDirection.Left;
		}
		return HitDirection.Down;
	}

	public float GetHitDirectionAsAngle(TargetType targetType)
	{
		float directionForType = GetDirectionForType(targetType);
		if (directionForType < 45f)
		{
			return 0f;
		}
		if (directionForType < 135f)
		{
			return 90f;
		}
		if (directionForType < 225f)
		{
			return 180f;
		}
		return 270f;
	}

	public HitDirection GetActualHitDirection(Transform target, TargetType targetType)
	{
		float num;
		for (num = GetActualDirection(target, targetType); num >= 360f; num -= 360f)
		{
		}
		for (; num < 0f; num += 360f)
		{
		}
		if (num < 135f)
		{
			if (num < 45f)
			{
				return HitDirection.Right;
			}
			return HitDirection.Up;
		}
		if (num < 225f)
		{
			return HitDirection.Left;
		}
		return HitDirection.Down;
	}

	public Vector2 GetHitDirectionAsVector(TargetType targetType)
	{
		return GetHitDirection(targetType) switch
		{
			HitDirection.Down => Vector2.down, 
			HitDirection.Up => Vector2.up, 
			HitDirection.Left => Vector2.left, 
			HitDirection.Right => Vector2.right, 
			_ => Vector2.zero, 
		};
	}
}
