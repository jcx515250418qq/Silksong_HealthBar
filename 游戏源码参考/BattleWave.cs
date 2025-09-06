using System.Collections.Generic;
using UnityEngine;

public class BattleWave : MonoBehaviour
{
	private struct ChildInfo
	{
		public GameObject gameObject;

		public HealthManager hm;

		public bool hasHM;

		public bool hasFSM;

		public ChildInfo(Transform transform, GameObject battleScene)
		{
			gameObject = transform.gameObject;
			hm = transform.GetComponent<HealthManager>();
			hasHM = hm != null;
			if (hasHM)
			{
				hm.ignorePersistence = true;
				hm.SetBattleScene(battleScene.gameObject);
			}
			hasFSM = transform.GetComponent<PlayMakerFSM>() != null;
			SpecialQuestItemVariant[] componentsInChildren = transform.GetComponentsInChildren<SpecialQuestItemVariant>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetInactive();
			}
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.DisableSpecialQuestDrops();
			}
		}
	}

	public float startDelay;

	public int remainingEnemyToEnd;

	public bool activateEnemiesOnStart;

	[SerializeField]
	private string startWaveEventRegister;

	[SerializeField]
	private bool clearDeathDrops = true;

	private BattleScene battleScene;

	private List<ChildInfo> children = new List<ChildInfo>();

	private BattleSceneEnemy[] enemies;

	private bool init;

	private int childCount;

	public bool HasFSM { get; private set; }

	public PlayMakerFSM Fsm { get; private set; }

	private void Start()
	{
		Init(null);
	}

	public void Init(BattleScene battleScene)
	{
		if (init)
		{
			UpdateChildInfo();
			return;
		}
		if (battleScene == null)
		{
			battleScene = GetComponentInParent<BattleScene>();
		}
		this.battleScene = battleScene;
		init = true;
		enemies = GetComponentsInChildren<BattleSceneEnemy>();
		Fsm = GetComponent<PlayMakerFSM>();
		HasFSM = Fsm != null;
		UpdateChildInfo();
	}

	private void UpdateChildInfo()
	{
		if (childCount == base.transform.childCount)
		{
			return;
		}
		childCount = base.transform.childCount;
		children.Clear();
		if (children.Capacity < childCount)
		{
			children.Capacity = childCount;
		}
		foreach (Transform item in base.transform)
		{
			children.Add(new ChildInfo(item, battleScene.gameObject));
		}
	}

	public void SetActive(bool value)
	{
		Init(null);
		BattleSceneEnemy[] array = enemies;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value);
		}
	}

	public void WaveStarting()
	{
		if (!string.IsNullOrEmpty(startWaveEventRegister))
		{
			EventRegister.SendEvent(startWaveEventRegister);
		}
	}

	public void WaveStarted(bool activateEnemies, ref int currentEnemies)
	{
		Init(null);
		foreach (ChildInfo child in children)
		{
			if (child.hasHM)
			{
				HealthManager hm = child.hm;
				if (clearDeathDrops)
				{
					hm.SetGeoSmall(0);
					hm.SetGeoMedium(0);
					hm.SetGeoLarge(0);
					hm.SetShellShards(0);
					hm.ClearItemDropsBattleScene();
				}
				currentEnemies++;
			}
			if (activateEnemiesOnStart)
			{
				child.gameObject.SetActive(value: true);
			}
			if (child.hasFSM)
			{
				FSMUtility.SendEventToGameObject(child.gameObject, "BATTLE START");
			}
		}
	}

	private void OnTransformChildrenChanged()
	{
		if (base.transform.childCount == 0 && (bool)battleScene)
		{
			battleScene.CheckEnemies();
		}
	}
}
