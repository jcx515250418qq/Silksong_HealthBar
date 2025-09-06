using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroWispLantern : MonoBehaviour
{
	[SerializeField]
	private ToolItem tool;

	[Space]
	[SerializeField]
	private ParticleSystem idlePt;

	[SerializeField]
	private GameObject haze;

	[SerializeField]
	private TriggerEnterEvent enemyRange;

	[SerializeField]
	private GameObject wispPrefab;

	[Space]
	[SerializeField]
	private MinMaxFloat wispSpawnStartDelay;

	[SerializeField]
	private MinMaxFloat wispSpawnCooldown;

	private Coroutine spawnRoutine;

	private readonly List<HealthManager> trackingEnemies = new List<HealthManager>();

	private bool effectsCleared;

	private bool isPaused;

	private void Awake()
	{
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += OnEquipsChanged;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO ENTERED SCENE").ReceivedEvent += OnEquipsChanged;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "CLEAR EFFECTS").ReceivedEvent += EffectsCleared;
		enemyRange.OnTriggerEntered += OnEnemyRangeEntered;
		enemyRange.OnTriggerExited += OnEnemyRangeExited;
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		OnEquipsChanged();
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		spawnRoutine = null;
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		isPaused = true;
	}

	private void OnEquipsChanged()
	{
		isPaused = false;
		bool isEquipped = tool.IsEquipped;
		ToggleWispLantern(isEquipped);
	}

	private void ToggleWispLantern(bool isEquipped)
	{
		ToggleLanternEffects(isEquipped);
		enemyRange.gameObject.SetActive(isEquipped);
		if (!isEquipped)
		{
			trackingEnemies.Clear();
		}
	}

	private void ToggleLanternEffects(bool isEquipped)
	{
		if (isEquipped)
		{
			idlePt.Play(withChildren: true);
		}
		else
		{
			idlePt.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		haze.SetActive(isEquipped);
	}

	private void EffectsCleared()
	{
		effectsCleared = true;
		ToggleLanternEffects(isEquipped: false);
	}

	private void OnEnemyRangeEntered(Collider2D col, GameObject sender)
	{
		if (TryGetHealthManager(col, out var healthManager))
		{
			if (!trackingEnemies.Contains(healthManager))
			{
				trackingEnemies.Add(healthManager);
			}
			if (spawnRoutine == null)
			{
				spawnRoutine = StartCoroutine(SpawnWispsRoutine());
			}
		}
	}

	private static bool TryGetHealthManager(Collider2D col, out HealthManager healthManager)
	{
		Rigidbody2D attachedRigidbody = col.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			healthManager = attachedRigidbody.GetComponent<HealthManager>();
			return healthManager != null;
		}
		healthManager = col.GetComponent<HealthManager>();
		return healthManager != null;
	}

	private void OnEnemyRangeExited(Collider2D col, GameObject sender)
	{
		if (TryGetHealthManager(col, out var healthManager))
		{
			trackingEnemies.Remove(healthManager);
		}
	}

	private IEnumerator SpawnWispsRoutine()
	{
		HeroController hc = HeroController.instance;
		while (isPaused)
		{
			yield return null;
		}
		yield return new WaitForSeconds(wispSpawnStartDelay.GetRandomValue());
		bool wasPaused = isPaused;
		while (trackingEnemies.Count > 0)
		{
			if (effectsCleared)
			{
				yield return null;
				if (!hc.controlReqlinquished)
				{
					ToggleLanternEffects(isEquipped: true);
					effectsCleared = false;
				}
				continue;
			}
			if (hc.playerData.silk <= 0)
			{
				yield return null;
				continue;
			}
			if (isPaused)
			{
				yield return null;
				continue;
			}
			if (wasPaused)
			{
				yield return new WaitForSeconds(wispSpawnCooldown.GetRandomValue());
			}
			wasPaused = isPaused;
			Vector3 position = base.transform.position;
			HealthManager enemyInRange = GetEnemyInRange();
			if ((bool)enemyInRange)
			{
				if (Helper.LineCast2DHit(position, enemyInRange.TargetPoint, 256, out var _))
				{
					yield return null;
					continue;
				}
				PlayMakerFSM component = wispPrefab.Spawn(base.transform.position).GetComponent<PlayMakerFSM>();
				component.FsmVariables.FindFsmGameObject("Target").Value = enemyInRange.gameObject;
				component.FsmVariables.FindFsmGameObject("Spawner").Value = base.gameObject;
				hc.TakeSilk(1, SilkSpool.SilkTakeSource.Wisp);
				yield return new WaitForSeconds(wispSpawnCooldown.GetRandomValue());
			}
			else
			{
				yield return null;
			}
		}
		spawnRoutine = null;
	}

	public HealthManager GetEnemyInRange()
	{
		trackingEnemies.RemoveAll((HealthManager o) => o == null);
		if (trackingEnemies.Count == 0)
		{
			return null;
		}
		HealthManager healthManager = trackingEnemies[Random.Range(0, trackingEnemies.Count)];
		if (!healthManager)
		{
			return null;
		}
		return healthManager;
	}
}
