using System;
using GlobalEnums;
using UnityEngine;

public class HeroDamageSpawn : MonoBehaviour
{
	[Serializable]
	private class Spawn
	{
		public ToolItem EquippedTool;

		public GameObject Prefab;

		public bool SpawnOnDeath;

		public IgnoreFlags ignoreFlags;
	}

	[Serializable]
	[Flags]
	public enum IgnoreFlags
	{
		None = 0,
		Zap = 1,
		Sink = 2,
		Spike = 4
	}

	[SerializeField]
	private Spawn[] spawns;

	private double canSpawnTime;

	private HeroController hc;

	private void Awake()
	{
		hc = GetComponent<HeroController>();
		hc.OnTakenDamageExtra += OnTakenDamage;
	}

	private void OnDestroy()
	{
		if (hc != null)
		{
			hc.OnTakenDamageExtra -= OnTakenDamage;
		}
	}

	private void OnTakenDamage(HeroController.DamageInfo damageInfo)
	{
		if (Time.timeAsDouble < canSpawnTime)
		{
			return;
		}
		canSpawnTime = Time.timeAsDouble + (double)hc.INVUL_TIME;
		bool flag = hc.playerData.health <= 0;
		Spawn[] array = spawns;
		foreach (Spawn spawn in array)
		{
			switch (damageInfo.hazardType)
			{
			case HazardType.ZAP:
				if (spawn.ignoreFlags.HasFlag(IgnoreFlags.Zap))
				{
					continue;
				}
				break;
			case HazardType.SINK:
				if (spawn.ignoreFlags.HasFlag(IgnoreFlags.Sink))
				{
					continue;
				}
				break;
			case HazardType.SPIKES:
				if (spawn.ignoreFlags.HasFlag(IgnoreFlags.Spike))
				{
					continue;
				}
				break;
			}
			if ((bool)spawn.EquippedTool && spawn.EquippedTool.IsEquipped && !(!spawn.SpawnOnDeath && flag))
			{
				spawn.Prefab.Spawn(base.transform.position);
			}
		}
	}
}
