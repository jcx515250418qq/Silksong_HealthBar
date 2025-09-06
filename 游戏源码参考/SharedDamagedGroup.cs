using System.Collections.Generic;
using UnityEngine;

public sealed class SharedDamagedGroup : MonoBehaviour
{
	private sealed class DamageInfo
	{
		public DamageEnemies damager;

		public bool started;
	}

	private readonly HashSet<Collider2D> damagedColliders = new HashSet<Collider2D>();

	private readonly HashSet<IHitResponder> damagePrevented = new HashSet<IHitResponder>();

	private readonly List<DamageEnemies> damagers = new List<DamageEnemies>();

	private readonly Dictionary<DamageEnemies, DamageInfo> damageInfos = new Dictionary<DamageEnemies, DamageInfo>();

	private int activeDamagerCount;

	public bool PreventDamage(Collider2D col)
	{
		return damagedColliders.Add(col);
	}

	public void PreventDamage(IHitResponder hitResponder)
	{
		damagePrevented.Add(hitResponder);
	}

	public void ClearDamagePrevented()
	{
		damagePrevented.Clear();
	}

	public bool HasDamaged(IHitResponder hitResponder)
	{
		if (damagePrevented.Contains(hitResponder))
		{
			return true;
		}
		foreach (DamageEnemies damager in damagers)
		{
			damager.TryClearRespondedList();
			if (damager.HasResponded(hitResponder))
			{
				return true;
			}
		}
		return false;
	}

	public void DamageStart(DamageEnemies damager)
	{
		if (!damageInfos.TryGetValue(damager, out var value))
		{
			value = (damageInfos[damager] = new DamageInfo());
			damagers.Add(damager);
		}
		bool num = !value.started;
		value.started = true;
		if (num)
		{
			if (activeDamagerCount == 0)
			{
				ClearDamagePrevented();
				damagedColliders.Clear();
			}
			activeDamagerCount++;
		}
	}

	public void DamageEnd(DamageEnemies damager)
	{
		if (!damageInfos.TryGetValue(damager, out var value))
		{
			return;
		}
		bool started = value.started;
		value.started = false;
		if (started)
		{
			activeDamagerCount--;
			if (activeDamagerCount <= 0)
			{
				activeDamagerCount = 0;
				ClearDamagePrevented();
				damagedColliders.Clear();
			}
		}
	}

	public void RemoveDamager(DamageEnemies damager)
	{
		DamageEnd(damager);
		if (damageInfos.Remove(damager))
		{
			damagers.Remove(damager);
		}
	}
}
