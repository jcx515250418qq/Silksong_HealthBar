using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public static class FlingUtils
{
	[Serializable]
	public struct Config
	{
		public GameObject Prefab;

		public float SpeedMin;

		public float SpeedMax;

		public float AngleMin;

		public float AngleMax;

		public float OriginVariationX;

		public float OriginVariationY;

		public int AmountMin;

		public int AmountMax;
	}

	[Serializable]
	public struct ChildrenConfig
	{
		public GameObject Parent;

		public int AmountMin;

		public int AmountMax;

		public float SpeedMin;

		public float SpeedMax;

		public float AngleMin;

		public float AngleMax;

		public float OriginVariationX;

		public float OriginVariationY;
	}

	public struct SelfConfig
	{
		public GameObject Object;

		public float SpeedMin;

		public float SpeedMax;

		public float AngleMin;

		public float AngleMax;
	}

	[Serializable]
	public struct ObjectFlingParams
	{
		public float SpeedMin;

		public float SpeedMax;

		public float AngleMin;

		public float AngleMax;

		public SelfConfig GetSelfConfig(GameObject flingObject)
		{
			SelfConfig result = default(SelfConfig);
			result.Object = flingObject;
			result.SpeedMin = SpeedMin;
			result.SpeedMax = SpeedMax;
			result.AngleMin = AngleMin;
			result.AngleMax = AngleMax;
			return result;
		}
	}

	public static void SpawnAndFling(Config config, Transform spawnPoint, Vector3 positionOffset, List<GameObject> addToList = null, float blackThreadAmount = -1f)
	{
		if (config.Prefab == null)
		{
			return;
		}
		int num = UnityEngine.Random.Range(config.AmountMin, config.AmountMax + 1);
		Vector3 vector = ((spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset);
		bool flag = addToList != null && num > 0;
		if (flag)
		{
			int capacity = addToList.Capacity;
			int num2 = capacity - addToList.Count;
			if (num > num2)
			{
				addToList.Capacity = capacity + (num - num2);
			}
		}
		for (int i = 0; i < num; i++)
		{
			Vector3 position = vector + new Vector3(UnityEngine.Random.Range(0f - config.OriginVariationX, config.OriginVariationX), UnityEngine.Random.Range(0f - config.OriginVariationY, config.OriginVariationY), 0f);
			GameObject gameObject = config.Prefab.Spawn(position);
			if (blackThreadAmount > 0f)
			{
				BlackThreadEffectRendererGroup component = gameObject.GetComponent<BlackThreadEffectRendererGroup>();
				if (component != null)
				{
					component.SetBlackThreadAmount(blackThreadAmount);
				}
			}
			Rigidbody2D component2 = gameObject.GetComponent<Rigidbody2D>();
			if (component2 != null)
			{
				float num3 = UnityEngine.Random.Range(config.SpeedMin, config.SpeedMax);
				float num4 = UnityEngine.Random.Range(config.AngleMin, config.AngleMax);
				component2.linearVelocity = new Vector2(Mathf.Cos(num4 * (MathF.PI / 180f)), Mathf.Sin(num4 * (MathF.PI / 180f))) * num3;
			}
			if (flag)
			{
				addToList.Add(gameObject);
			}
		}
	}

	public static void SpawnAndFlingShellShards(Config config, Transform spawnPoint, Vector3 positionOffset, List<GameObject> addToList = null)
	{
		if (config.Prefab == null)
		{
			return;
		}
		int num = UnityEngine.Random.Range(config.AmountMin, config.AmountMax + 1);
		Vector3 vector = ((spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset);
		bool flag = addToList != null && num > 0;
		if (flag)
		{
			int capacity = addToList.Capacity;
			int num2 = capacity - addToList.Count;
			if (num > num2)
			{
				addToList.Capacity = capacity + (num - num2);
			}
		}
		int num3 = num / 5;
		int num4 = 0;
		while (num3 > 0 && num >= 5 + (num4 + 1) * 2)
		{
			if (UnityEngine.Random.value > 0.5f)
			{
				num4++;
				num -= 5;
			}
			num3--;
		}
		if (num4 > 0)
		{
			GameObject shellShardMidPrefab = Gameplay.ShellShardMidPrefab;
			if (shellShardMidPrefab != null)
			{
				for (int i = 0; i < num4; i++)
				{
					Vector3 position = vector + new Vector3(UnityEngine.Random.Range(0f - config.OriginVariationX, config.OriginVariationX), UnityEngine.Random.Range(0f - config.OriginVariationY, config.OriginVariationY), 0f);
					GameObject gameObject = shellShardMidPrefab.Spawn(position);
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					if (component != null)
					{
						float num5 = UnityEngine.Random.Range(config.SpeedMin, config.SpeedMax);
						float num6 = UnityEngine.Random.Range(config.AngleMin, config.AngleMax);
						component.linearVelocity = new Vector2(Mathf.Cos(num6 * (MathF.PI / 180f)), Mathf.Sin(num6 * (MathF.PI / 180f))) * num5;
					}
					if (flag)
					{
						addToList.Add(gameObject);
					}
				}
			}
			else
			{
				num += num4 * 5;
			}
		}
		for (int j = 0; j < num; j++)
		{
			Vector3 position2 = vector + new Vector3(UnityEngine.Random.Range(0f - config.OriginVariationX, config.OriginVariationX), UnityEngine.Random.Range(0f - config.OriginVariationY, config.OriginVariationY), 0f);
			GameObject gameObject2 = config.Prefab.Spawn(position2);
			Rigidbody2D component2 = gameObject2.GetComponent<Rigidbody2D>();
			if (component2 != null)
			{
				float num7 = UnityEngine.Random.Range(config.SpeedMin, config.SpeedMax);
				float num8 = UnityEngine.Random.Range(config.AngleMin, config.AngleMax);
				component2.linearVelocity = new Vector2(Mathf.Cos(num8 * (MathF.PI / 180f)), Mathf.Sin(num8 * (MathF.PI / 180f))) * num7;
			}
			if (flag)
			{
				addToList.Add(gameObject2);
			}
		}
	}

	public static void EnsurePersonalPool(Config config, GameObject gameObject)
	{
		if (config.Prefab != null)
		{
			PersonalObjectPool.EnsurePooledInScene(gameObject, config.Prefab, config.AmountMax, finished: false);
		}
	}

	public static void FlingChildren(ChildrenConfig config, Transform spawnPoint, Vector3 positionOffset, MinMaxFloat? randomiseZ = null)
	{
		if (config.Parent == null)
		{
			return;
		}
		Vector3 vector = ((spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset);
		int childCount = config.Parent.transform.childCount;
		int num = ((config.AmountMax > 0) ? UnityEngine.Random.Range(config.AmountMin, config.AmountMax) : childCount);
		if (num > childCount)
		{
			num = childCount;
		}
		for (int i = 0; i < num; i++)
		{
			Transform child = config.Parent.transform.GetChild(i);
			if (randomiseZ.HasValue)
			{
				child.SetLocalPositionZ(randomiseZ.Value.GetRandomValue());
			}
			child.gameObject.SetActive(value: true);
			_ = vector + new Vector3(UnityEngine.Random.Range(0f - config.OriginVariationX, config.OriginVariationX), UnityEngine.Random.Range(0f - config.OriginVariationY, config.OriginVariationY), 0f);
			Rigidbody2D component = child.GetComponent<Rigidbody2D>();
			if (component != null)
			{
				float num2 = UnityEngine.Random.Range(config.SpeedMin, config.SpeedMax);
				float num3 = UnityEngine.Random.Range(config.AngleMin, config.AngleMax);
				component.linearVelocity = new Vector2(Mathf.Cos(num3 * (MathF.PI / 180f)), Mathf.Sin(num3 * (MathF.PI / 180f))) * num2;
			}
		}
	}

	public static void FlingObject(SelfConfig config, Transform spawnPoint, Vector3 positionOffset)
	{
		if (!(config.Object == null))
		{
			Vector3 position = ((spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset);
			config.Object.transform.position = position;
			Rigidbody2D component = config.Object.GetComponent<Rigidbody2D>();
			if (!(component == null))
			{
				float num = UnityEngine.Random.Range(config.SpeedMin, config.SpeedMax);
				float num2 = UnityEngine.Random.Range(config.AngleMin, config.AngleMax);
				component.linearVelocity = new Vector2(Mathf.Cos(num2 * (MathF.PI / 180f)), Mathf.Sin(num2 * (MathF.PI / 180f))) * num;
			}
		}
	}
}
