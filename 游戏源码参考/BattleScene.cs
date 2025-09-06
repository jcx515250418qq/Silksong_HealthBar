using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;

public class BattleScene : MonoBehaviour
{
	public List<BattleWave> waves;

	public GameObject camLocks;

	public GameObject gates;

	public bool openGatesOnEnd = true;

	public GameObject activeBeforeBattle;

	[ModifiableProperty]
	[Conditional("activeBeforeBattle", true, true, true)]
	public bool disableActiveBeforeBattleAtEnd;

	public GameObject activeAfterBattle;

	public GameObject inactiveAfterBattle;

	public GameObject activeDuringBattle;

	public PlayMakerFSM endScene;

	[PlayerDataField(typeof(bool), false)]
	public string setPDBoolOnEnd;

	[PlayerDataField(typeof(bool), false)]
	public string setExtraPDBoolOnEnd;

	public string battleStartEventRegister;

	public string finalEnemyKillEventRegister;

	public string battleEndEventRegister;

	public float battleStartPause;

	public float battleEndPause = 2f;

	public MusicCue musicCueStart;

	public MusicCue musicCueNone;

	public AudioMixerSnapshot snapshotStart;

	public float snapshotStartTime = 1f;

	public MusicCue musicCueEnd;

	public AudioMixerSnapshot snapshotEnd;

	public AudioMixerSnapshot snapshotSilent;

	public AudioClip battleStartClip;

	public float battleStartClipPause;

	public AudioClip battleEndClip;

	public AudioClip slomoEffectClip;

	public bool noStartSilence;

	public bool noEndSilence;

	public bool noEndSlomo;

	public bool garbageCollectOnEnd;

	public bool dontDisableCamlocksOnEnd;

	[SerializeField]
	private bool toggleWavesAwake;

	private AudioSource audioSource;

	private bool started;

	private bool completed;

	public int currentWave;

	public int currentEnemies;

	public int enemiesToNext;

	private int loopsUntilDeactivate = 2;

	private BoxCollider2D boxCollider2D;

	private PolygonCollider2D polygonCollider2D;

	private List<PlayMakerFSM> gateFsms = new List<PlayMakerFSM>();

	private IInitialisable[] initialisables;

	private int lastDecrementFrame;

	private Coroutine checkEnemyRoutine;

	private void Awake()
	{
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if (!string.IsNullOrEmpty(setPDBoolOnEnd))
		{
			if ((bool)component)
			{
				Object.Destroy(component);
			}
		}
		else if (component != null)
		{
			component.OnGetSaveState += delegate(out bool val)
			{
				val = completed;
			};
			component.OnSetSaveState += delegate(bool val)
			{
				if (val)
				{
					completed = true;
					BattleCompleted();
				}
			};
		}
		if ((bool)musicCueStart)
		{
			musicCueStart.Preload(base.gameObject);
		}
		if ((bool)musicCueEnd)
		{
			musicCueEnd.Preload(base.gameObject);
		}
		audioSource = GetComponent<AudioSource>();
		foreach (BattleWave wave in waves)
		{
			wave.Init(this);
			if (toggleWavesAwake)
			{
				wave.SetActive(value: true);
			}
		}
		initialisables = GetComponentsInChildren<IInitialisable>(includeInactive: true);
		if (initialisables != null)
		{
			for (int i = 0; i < initialisables.Length; i++)
			{
				initialisables[i].OnAwake();
			}
		}
		boxCollider2D = GetComponent<BoxCollider2D>();
		polygonCollider2D = GetComponent<PolygonCollider2D>();
		foreach (Transform item in gates.transform)
		{
			PlayMakerFSM component2 = item.GetComponent<PlayMakerFSM>();
			if ((bool)component2)
			{
				gateFsms.Add(component2);
			}
		}
	}

	private void Start()
	{
		if (toggleWavesAwake)
		{
			foreach (BattleWave wave in waves)
			{
				wave.SetActive(value: false);
			}
		}
		if (initialisables != null)
		{
			for (int i = 0; i < initialisables.Length; i++)
			{
				initialisables[i].OnStart();
			}
			PersonalObjectPool[] componentsInChildren = GetComponentsInChildren<PersonalObjectPool>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].OnStart();
			}
		}
		initialisables = null;
		if ((bool)activeDuringBattle)
		{
			activeDuringBattle.SetActive(value: false);
		}
		string.IsNullOrEmpty(setPDBoolOnEnd);
		PlayerData.instance.GetBool(setPDBoolOnEnd);
		if (!string.IsNullOrEmpty(setPDBoolOnEnd) && PlayerData.instance.GetBool(setPDBoolOnEnd))
		{
			completed = true;
			BattleCompleted();
		}
		StartCoroutine(CheckCompletion());
	}

	private void OnDisable()
	{
		checkEnemyRoutine = null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			HeroController component = collision.GetComponent<HeroController>();
			if ((bool)component && component.isHeroInPosition && currentWave == 0 && !completed && loopsUntilDeactivate <= 0)
			{
				StartBattle();
			}
		}
	}

	public void Update()
	{
		if (loopsUntilDeactivate <= 0)
		{
			return;
		}
		loopsUntilDeactivate--;
		if (loopsUntilDeactivate > 0)
		{
			return;
		}
		foreach (BattleWave wave in waves)
		{
			wave.SetActive(value: false);
		}
	}

	public void SendEventToChildren(string eventName)
	{
		foreach (PlayMakerFSM gateFsm in gateFsms)
		{
			gateFsm.SendEvent(eventName);
		}
	}

	public void LockInBattle()
	{
		if (camLocks != null)
		{
			camLocks.SetActive(value: true);
		}
		SendEventToChildren("BG CLOSE");
	}

	public void StartBattle()
	{
		StartCoroutine(DoStartBattle());
	}

	public IEnumerator DoStartBattle()
	{
		if (!started)
		{
			if (boxCollider2D != null)
			{
				boxCollider2D.enabled = false;
			}
			if (polygonCollider2D != null)
			{
				polygonCollider2D.enabled = false;
			}
			LockInBattle();
			TrackingTrail.FadeDownAll();
			if (snapshotSilent != null && !noStartSilence)
			{
				snapshotSilent.TransitionTo(0.5f);
			}
			yield return new WaitForSeconds(battleStartPause);
			if ((bool)activeDuringBattle)
			{
				activeDuringBattle.SetActive(value: true);
			}
			EventRegister.SendEvent(battleStartEventRegister);
			started = true;
			StartCoroutine(StartWave(0));
			if (audioSource != null && battleStartClip != null)
			{
				audioSource.PlayOneShot(battleStartClip);
			}
			yield return new WaitForSeconds(battleStartClipPause);
			if (musicCueNone != null)
			{
				GameManager.instance.AudioManager.ApplyMusicCue(musicCueNone, 0f, 0f, applySnapshot: false);
			}
			if (musicCueStart != null)
			{
				GameManager.instance.AudioManager.ApplyMusicCue(musicCueStart, 0f, 0f, applySnapshot: false);
			}
			if (snapshotStart != null)
			{
				snapshotStart.TransitionTo(snapshotStartTime);
			}
		}
	}

	private IEnumerator StartWave(int waveNumber)
	{
		BattleWave wave = waves[waveNumber];
		wave.SetActive(value: true);
		wave.WaveStarting();
		PlayMakerFSM waveFSM = wave.Fsm;
		bool hasWaveFSM = wave.HasFSM;
		if (hasWaveFSM)
		{
			waveFSM.SendEvent("WAVE QUEUED");
		}
		yield return new WaitForSeconds(wave.startDelay);
		if (hasWaveFSM)
		{
			waveFSM.SendEvent("WAVE START");
		}
		wave.WaveStarted(waves[waveNumber].activateEnemiesOnStart, ref currentEnemies);
		enemiesToNext = waves[waveNumber].remainingEnemyToEnd;
		currentWave = waveNumber;
		yield return new WaitForSeconds(1f);
		if (checkEnemyRoutine != null)
		{
			StopCoroutine(checkEnemyRoutine);
			checkEnemyRoutine = null;
		}
		DoCheckEnemies();
	}

	public void DecrementEnemy()
	{
		currentEnemies--;
		lastDecrementFrame = Time.frameCount;
		if (currentEnemies <= enemiesToNext)
		{
			WaveEnd();
		}
		else
		{
			DoCheckEnemies();
		}
	}

	public void DecrementBigEnemy()
	{
		StartCoroutine(DoDecrementBigEnemy());
	}

	public IEnumerator DoDecrementBigEnemy()
	{
		yield return new WaitForSeconds(1f);
		currentEnemies--;
		if (currentEnemies <= enemiesToNext)
		{
			WaveEnd();
		}
		else
		{
			DoCheckEnemies();
		}
	}

	public void WaveEnd()
	{
		if (started)
		{
			if (currentWave >= waves.Count - 1)
			{
				DoEndBattle();
			}
			else
			{
				StartCoroutine(StartWave(currentWave + 1));
			}
		}
	}

	public void CheckEnemies()
	{
		if (started)
		{
			DoCheckEnemies();
		}
	}

	private void DoCheckEnemies()
	{
		DoCheckEnemiesNew();
	}

	private void DoCheckEnemiesNew()
	{
		if (checkEnemyRoutine != null)
		{
			StopCoroutine(checkEnemyRoutine);
			checkEnemyRoutine = null;
		}
		int num = 0;
		for (int i = 0; i < currentWave + 1; i++)
		{
			num += waves[i].transform.childCount;
		}
		if (num <= enemiesToNext)
		{
			currentEnemies = num;
			WaveEnd();
		}
		else if (checkEnemyRoutine == null)
		{
			checkEnemyRoutine = StartCoroutine(CheckEnemyCount());
		}
	}

	public IEnumerator CheckEnemyCount()
	{
		yield return new WaitForSeconds(0.1f);
		int num = 0;
		for (int i = 0; i < currentWave + 1; i++)
		{
			num += waves[i].transform.childCount;
		}
		if (num <= enemiesToNext)
		{
			currentEnemies = num;
			WaveEnd();
		}
		checkEnemyRoutine = null;
	}

	public void DoEndBattle()
	{
		StartCoroutine(EndBattle(Time.frameCount == lastDecrementFrame));
	}

	private IEnumerator EndBattle(bool waitExtra)
	{
		if (completed)
		{
			yield break;
		}
		GameManager gm = GameManager.instance;
		started = false;
		completed = true;
		if (snapshotSilent != null && !noEndSilence)
		{
			snapshotSilent.TransitionTo(0.25f);
		}
		if (!noEndSlomo)
		{
			gm.FreezeMoment(FreezeMomentTypes.EnemyBattleEndSlow);
			if (audioSource != null && slomoEffectClip != null)
			{
				audioSource.PlayOneShot(slomoEffectClip);
			}
		}
		if (!string.IsNullOrEmpty(finalEnemyKillEventRegister))
		{
			EventRegister.SendEvent(finalEnemyKillEventRegister);
		}
		if (garbageCollectOnEnd)
		{
			if (waitExtra)
			{
				yield return null;
			}
			yield return null;
			TimeManager.TimeControlInstance timeControl = TimeManager.CreateTimeControl(0f);
			try
			{
				GCManager.Collect();
				yield return null;
			}
			finally
			{
				timeControl.Release();
			}
		}
		yield return new WaitForSeconds(1f);
		if (battleEndClip != null)
		{
			AudioSource fanfareEnemyBattleClear = GameManager.instance.AudioManager.FanfareEnemyBattleClear;
			fanfareEnemyBattleClear.clip = battleEndClip;
			fanfareEnemyBattleClear.Play();
		}
		yield return new WaitForSeconds(battleEndPause);
		if (!string.IsNullOrEmpty(setPDBoolOnEnd))
		{
			gm.playerData.SetBool(setPDBoolOnEnd, value: true);
		}
		if (!string.IsNullOrEmpty(setExtraPDBoolOnEnd))
		{
			gm.playerData.SetBool(setExtraPDBoolOnEnd, value: true);
		}
		if (camLocks != null && !dontDisableCamlocksOnEnd)
		{
			camLocks.SetActive(value: false);
		}
		if (openGatesOnEnd)
		{
			SendEventToChildren("BG OPEN");
		}
		if ((bool)endScene)
		{
			endScene.SendEvent("BATTLE END");
		}
		if ((bool)activeDuringBattle)
		{
			activeDuringBattle.SetActive(value: false);
		}
		if (disableActiveBeforeBattleAtEnd)
		{
			activeBeforeBattle.SetActive(value: false);
		}
		if (!string.IsNullOrEmpty(battleEndEventRegister))
		{
			EventRegister.SendEvent(battleEndEventRegister);
		}
		if (musicCueEnd != null)
		{
			gm.AudioManager.ApplyMusicCue(musicCueEnd, 0f, 0f, applySnapshot: false);
		}
		if (snapshotEnd != null)
		{
			snapshotEnd.TransitionTo(2f);
		}
	}

	public IEnumerator CheckCompletion()
	{
		yield return new WaitForSeconds(0.5f);
		if (activeBeforeBattle != null)
		{
			if (completed)
			{
				activeBeforeBattle.SetActive(value: false);
			}
			else
			{
				activeBeforeBattle.SetActive(value: true);
			}
		}
		if (activeAfterBattle != null)
		{
			if (completed)
			{
				activeAfterBattle.SetActive(value: true);
			}
			else
			{
				activeAfterBattle.SetActive(value: false);
			}
		}
		if (inactiveAfterBattle != null)
		{
			if (completed)
			{
				inactiveAfterBattle.SetActive(value: false);
			}
			else
			{
				inactiveAfterBattle.SetActive(value: true);
			}
		}
	}

	public void BattleCompleted()
	{
		if (boxCollider2D != null)
		{
			boxCollider2D.enabled = false;
		}
		if (polygonCollider2D != null)
		{
			polygonCollider2D.enabled = false;
		}
		if ((bool)endScene)
		{
			endScene.SendEvent("BATTLE COMPLETED");
		}
		if (openGatesOnEnd)
		{
			SendEventToChildren("BG QUICK OPEN");
		}
		foreach (BattleWave wave in waves)
		{
			wave.gameObject.SetActive(value: false);
		}
	}
}
