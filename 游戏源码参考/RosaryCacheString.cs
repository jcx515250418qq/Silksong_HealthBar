using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

public class RosaryCacheString : RosaryCacheHanging
{
	[Serializable]
	private class RosaryGroup
	{
		public GameObject[] RepresentingObjects;

		public GameObject SpawnPrefab;

		public MinMaxFloat Speed;

		public MinMaxFloat Angle;

		public void FlingPrefab(Transform spawnPoint, Vector3 offset)
		{
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = SpawnPrefab;
			config.AmountMin = 1;
			config.AmountMax = 1;
			config.SpeedMin = Speed.Start;
			config.SpeedMax = Speed.End;
			config.AngleMin = Angle.Start;
			config.AngleMax = Angle.End;
			FlingUtils.SpawnAndFling(config, spawnPoint, offset);
		}
	}

	private struct FlintObject
	{
		public GameObject Obj;

		public RosaryGroup Group;

		public FlintObject(GameObject obj, RosaryGroup group)
		{
			Obj = obj;
			Group = group;
		}
	}

	[Header("String")]
	[SerializeField]
	private RosaryGroup[] rosaryGroups;

	[SerializeField]
	private int maxHits;

	private List<FlintObject> flingObjects = new List<FlintObject>();

	protected override int HitCount => maxHits;

	private void OnValidate()
	{
		if (maxHits <= 0)
		{
			maxHits = 1;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		HitState hitState = hitStates[0];
		for (int i = 0; i < maxHits - 1; i++)
		{
			hitStates.Insert(0, hitState.Duplicate());
		}
		flingObjects = rosaryGroups.SelectMany((RosaryGroup group) => group.RepresentingObjects.Select((GameObject obj) => new FlintObject(obj, group))).ToList();
		for (int j = 0; j < maxHits; j++)
		{
			bool isLast = j == maxHits - 1;
			hitStates[j].OnActivated.AddListener(delegate
			{
				FlingRosaries(isLast);
			});
		}
	}

	private void FlingRosaries(bool isLast)
	{
		int num = Mathf.FloorToInt((float)flingObjects.Count / (float)maxHits);
		num -= UnityEngine.Random.Range(-1, 2);
		if (!isLast)
		{
			for (int i = 0; i < num; i++)
			{
				num = Mathf.Min(num, flingObjects.Count);
				if (num != 0)
				{
					int index = UnityEngine.Random.Range(0, num);
					FlintObject flintObject = flingObjects[index];
					flingObjects.RemoveAt(index);
					flintObject.Obj.SetActive(value: false);
					if (!base.IsReactivating)
					{
						flintObject.Group.FlingPrefab(flintObject.Obj.transform, Vector3.zero);
					}
					continue;
				}
				break;
			}
			return;
		}
		foreach (FlintObject flingObject in flingObjects)
		{
			flingObject.Obj.SetActive(value: false);
			if (!base.IsReactivating)
			{
				flingObject.Group.FlingPrefab(flingObject.Obj.transform, Vector3.zero);
			}
		}
	}
}
