using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class MemoryOrbSceneMap : MonoBehaviour
{
	[SerializeField]
	private GameObject needolinScreenEdgePrefab;

	[SerializeField]
	private GameObject needolinScreenEdgePrefabLarge;

	[SerializeField]
	private float screenEdgePadding;

	private Dictionary<Transform, GameObject> otherOrbs;

	private List<GameObject> spawnedEffects;

	private List<ParticleSystem> temp;

	private void Awake()
	{
		base.gameObject.SetActiveChildren(value: false);
		if ((bool)needolinScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, needolinScreenEdgePrefab, 10);
		}
	}

	private void Start()
	{
		otherOrbs = new Dictionary<Transform, GameObject>();
		string sceneNameString = GameManager.instance.GetSceneNameString();
		PlayerData instance = PlayerData.instance;
		List<Transform> list = new List<Transform>();
		foreach (Transform item in base.transform)
		{
			list.Clear();
			if (item.name == sceneNameString)
			{
				continue;
			}
			foreach (Transform item2 in item)
			{
				MemoryOrbSceneMapConditional component = item2.GetComponent<MemoryOrbSceneMapConditional>();
				bool flag;
				if ((bool)component)
				{
					if (!component.IsActive)
					{
						continue;
					}
					flag = true;
				}
				else
				{
					string text = item2.name;
					if (text[0] == '!')
					{
						flag = true;
						string text2 = text;
						text = text2.Substring(1, text2.Length - 1);
					}
					else
					{
						flag = false;
					}
					if (VariableExtensions.VariableExists<PlayerData>(text, typeof(bool)))
					{
						if (instance.GetBool(text) != flag)
						{
							continue;
						}
					}
					else
					{
						if (!VariableExtensions.VariableExists<PlayerData>(text, typeof(ulong)))
						{
							Debug.LogError("Could not find matching variable for memory orb map child: " + text);
							continue;
						}
						if (instance.GetVariable<ulong>(text) != 0L)
						{
							continue;
						}
					}
				}
				bool flag2 = false;
				foreach (Transform item3 in list)
				{
					if (!(Vector2.Distance(item3.position, item2.position) > 1f))
					{
						flag2 = true;
						otherOrbs[item3] = needolinScreenEdgePrefabLarge;
						break;
					}
				}
				if (!flag2)
				{
					otherOrbs[item2] = (flag ? needolinScreenEdgePrefabLarge : needolinScreenEdgePrefab);
					list.Add(item2);
				}
			}
		}
	}

	private void OnEnable()
	{
		HeroPerformanceRegion.StartedPerforming += OnNeedolinStart;
		HeroPerformanceRegion.StoppedPerforming += OnNeedolinStop;
	}

	private void OnDisable()
	{
		HeroPerformanceRegion.StartedPerforming -= OnNeedolinStart;
		HeroPerformanceRegion.StoppedPerforming -= OnNeedolinStop;
	}

	private void OnNeedolinStart()
	{
		if (NeedolinMsgBox.IsBlocked)
		{
			return;
		}
		if (spawnedEffects == null)
		{
			spawnedEffects = new List<GameObject>();
		}
		foreach (KeyValuePair<Transform, GameObject> otherOrb in otherOrbs)
		{
			spawnedEffects.Add(MemoryOrbSource.SpawnScreenEdgeEffect(otherOrb.Value, otherOrb.Value, otherOrb.Key.position, screenEdgePadding));
		}
	}

	private void OnNeedolinStop()
	{
		if (spawnedEffects == null)
		{
			return;
		}
		if (temp == null)
		{
			temp = new List<ParticleSystem>();
		}
		for (int num = spawnedEffects.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = spawnedEffects[num];
			gameObject.GetComponentsInChildren(temp);
			foreach (ParticleSystem item in temp)
			{
				item.Stop(withChildren: true);
			}
			if (temp.Count == 0)
			{
				gameObject.Recycle();
			}
			temp.Clear();
			spawnedEffects.RemoveAt(num);
		}
	}
}
