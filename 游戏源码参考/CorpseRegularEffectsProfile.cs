using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Profiles/Corpse Effects Profile")]
public class CorpseRegularEffectsProfile : ScriptableObject
{
	[Serializable]
	public class EffectList
	{
		public List<GameObject> effects = new List<GameObject>();
	}

	[SerializeField]
	private GameObject[] spawnOnStart;

	[SerializeField]
	private float stunTime;

	[SerializeField]
	private GameObject loopingStunEffectPrefab;

	[SerializeField]
	private CameraShakeTarget stunEndShake;

	[SerializeField]
	private GameObject[] spawnOnStunEnd;

	[SerializeField]
	private GameObject[] spawnOnLand;

	[SerializeField]
	private BloodSpawner.Config explodeBlood;

	[SerializeField]
	private GameObject[] spawnOnExplode;

	[ArrayForEnum(typeof(ElementalEffectType))]
	[SerializeField]
	private EffectList[] elementalEffects = new EffectList[0];

	public GameObject[] SpawnOnStart => spawnOnStart;

	public float StunTime => stunTime;

	public GameObject LoopingStunEffectPrefab => loopingStunEffectPrefab;

	public CameraShakeTarget StunEndShake => stunEndShake;

	public GameObject[] SpawnOnStunEnd => spawnOnStunEnd;

	public GameObject[] SpawnOnLand => spawnOnLand;

	public BloodSpawner.Config ExplodeBlood => explodeBlood;

	public GameObject[] SpawnOnExplode => spawnOnExplode;

	public EffectList[] ElementalEffects
	{
		get
		{
			return elementalEffects;
		}
		set
		{
			elementalEffects = value;
		}
	}

	public void EnsurePersonalPool(GameObject gameObject)
	{
		bool flag = false;
		if (EnsureArray(spawnOnStart, gameObject))
		{
			flag = true;
		}
		if (loopingStunEffectPrefab != null)
		{
			PersonalObjectPool.EnsurePooledInScene(gameObject, loopingStunEffectPrefab, 1, finished: false);
			flag = true;
		}
		if (EnsureArray(spawnOnStunEnd, gameObject))
		{
			flag = true;
		}
		if (EnsureArray(spawnOnLand, gameObject))
		{
			flag = true;
		}
		if (EnsureArray(spawnOnExplode, gameObject))
		{
			flag = true;
		}
		if (ElementalEffects != null)
		{
			EffectList[] array = ElementalEffects;
			foreach (EffectList effectList in array)
			{
				if (effectList == null)
				{
					continue;
				}
				foreach (GameObject effect in effectList.effects)
				{
					PersonalObjectPool.EnsurePooledInScene(gameObject, effect, 1, finished: false);
					flag = true;
				}
			}
		}
		if (flag)
		{
			PersonalObjectPool.EnsurePooledInSceneFinished(gameObject);
		}
	}

	private bool EnsureArray(GameObject[] prefabArray, GameObject gameObject)
	{
		bool result = false;
		if (prefabArray != null)
		{
			foreach (GameObject prefab in prefabArray)
			{
				PersonalObjectPool.EnsurePooledInScene(gameObject, prefab, 1, finished: false);
				result = true;
			}
		}
		return result;
	}

	private void OnValidate()
	{
		if (ElementalEffects == null)
		{
			ElementalEffects = new EffectList[0];
		}
		ArrayForEnumAttribute.EnsureArraySize(ref elementalEffects, typeof(ElementalEffectType));
	}
}
