using System.Collections.Generic;
using UnityEngine;

public class AudioLoopMaster : MonoBehaviour
{
	private sealed class AudioSyncInfo
	{
		private const float ALLOWABLE_TIME_DIF = 0.025f;

		private const double ALLOWABLE_SAMPLE_DIF = 0.03;

		private const int STAGE_1_SYNC_ATTEMPTS = 3;

		private const int STAGE_2_SYNC_ATTEMPTS = 6;

		private const float STAGE_2_WAIT = 5f;

		private AudioLoopMaster audioLoopMaster;

		private AudioSource audioSource;

		private bool shouldSync;

		private int maxSamples;

		private int frequency;

		private int sampleDif;

		private int syncCount;

		private bool useTime;

		private bool resync;

		private bool hasSynced;

		public AudioSource AudioSource => audioSource;

		public AudioSyncInfo(AudioLoopMaster audioLoopMaster, AudioSource audioSource)
		{
			this.audioLoopMaster = audioLoopMaster;
			this.audioSource = audioSource;
		}

		public void ResetSyncStatus()
		{
			syncCount = 0;
			resync = false;
			useTime = false;
		}

		public void AllowResync()
		{
			syncCount = 0;
			resync = true;
		}

		public void SetSync(bool shouldSync)
		{
			this.shouldSync = shouldSync;
			hasSynced = false;
			if (shouldSync)
			{
				ResetSyncStatus();
				if ((bool)AudioSource.clip)
				{
					maxSamples = AudioSource.clip.samples;
					frequency = AudioSource.clip.frequency;
					sampleDif = Mathf.CeilToInt((float)((double)frequency * 0.03));
				}
				else
				{
					maxSamples = 0;
					this.shouldSync = false;
					sampleDif = Mathf.CeilToInt(1230f);
				}
			}
		}

		public bool Sync()
		{
			if (!shouldSync)
			{
				return false;
			}
			if (!useTime)
			{
				useTime = frequency != audioLoopMaster.clipFrequency;
			}
			if (!useTime)
			{
				int timeSamples = audioLoopMaster.audioSource.timeSamples;
				if (Mathf.Abs(AudioSource.timeSamples - timeSamples) <= sampleDif)
				{
					syncCount = 0;
					return false;
				}
				if (timeSamples > maxSamples)
				{
					syncCount = 0;
					return false;
				}
			}
			else
			{
				float time = audioLoopMaster.audioSource.time;
				float time2 = audioSource.time;
				if (Mathf.Abs(time - time2) < 0.025f)
				{
					syncCount = 0;
					return false;
				}
				if (time > audioSource.clip.length)
				{
					syncCount = 0;
					return false;
				}
			}
			if (hasSynced && resync)
			{
				resync = false;
				syncCount = 3;
			}
			else if (syncCount >= 6)
			{
				syncCount = 0;
			}
			hasSynced = true;
			resync = false;
			syncCount++;
			if (!useTime)
			{
				AudioSource.timeSamples = audioLoopMaster.audioSource.timeSamples;
			}
			else
			{
				AudioSource.time = audioLoopMaster.audioSource.time;
			}
			return true;
		}
	}

	private AudioSource audioSource;

	public AudioSource action;

	public AudioSource sub;

	public AudioSource mainAlt;

	public AudioSource tension;

	public AudioSource extra;

	private float syncCooldown;

	private AudioSyncInfo syncAction;

	private AudioSyncInfo syncSub;

	private AudioSyncInfo syncMainAlt;

	private AudioSyncInfo syncTension;

	private AudioSyncInfo syncExtra;

	private bool isSyncing;

	private float lastSyncTime;

	private int clipFrequency;

	private GameManager gm;

	private bool fullSync;

	private List<AudioSyncInfo> syncList = new List<AudioSyncInfo>();

	private int index;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		gm = GameManager.instance;
		syncAction = new AudioSyncInfo(this, action);
		syncSub = new AudioSyncInfo(this, sub);
		syncMainAlt = new AudioSyncInfo(this, mainAlt);
		syncTension = new AudioSyncInfo(this, tension);
		syncExtra = new AudioSyncInfo(this, extra);
		syncList.Add(syncAction);
		syncList.Add(syncSub);
		syncList.Add(syncMainAlt);
		syncList.Add(syncTension);
		syncList.Add(syncExtra);
		gm.NextSceneWillActivate += OnNextSceneWillActivate;
	}

	private void OnDestroy()
	{
		if ((bool)gm)
		{
			gm.NextSceneWillActivate -= OnNextSceneWillActivate;
		}
	}

	private void OnNextSceneWillActivate()
	{
		ReSync();
		AllowSync();
	}

	private void ReSync()
	{
		syncAction.AllowResync();
		syncSub.AllowResync();
		syncMainAlt.AllowResync();
		syncTension.AllowResync();
		syncExtra.AllowResync();
	}

	private void Update()
	{
		if (CheatManager.DisableMusicSync)
		{
			return;
		}
		if (!isSyncing)
		{
			if (syncCooldown > 0f)
			{
				syncCooldown -= Time.deltaTime;
				return;
			}
			if (gm.IsInSceneTransition)
			{
				return;
			}
			ReSync();
			syncCooldown = 10f;
		}
		else if (Time.realtimeSinceStartup < lastSyncTime)
		{
			return;
		}
		if (!isSyncing || fullSync)
		{
			AudioClip clip = audioSource.clip;
			if ((bool)clip)
			{
				clipFrequency = clip.frequency;
			}
			else
			{
				clipFrequency = -1;
			}
		}
		isSyncing = false;
		for (int i = 0; i < syncList.Count; i++)
		{
			if (syncList[(i + index) % syncList.Count].Sync())
			{
				isSyncing = true;
				if (!fullSync)
				{
					index = (index + i) % syncList.Count;
					break;
				}
			}
		}
		lastSyncTime = Time.realtimeSinceStartup + 1f;
		fullSync = false;
	}

	public void AllowSync()
	{
		syncCooldown = 0f;
		fullSync = true;
	}

	public void SetSyncAction(bool set)
	{
		syncAction.SetSync(set);
	}

	public void SetSyncSub(bool set)
	{
		syncSub.SetSync(set);
	}

	public void SetSyncMainAlt(bool set)
	{
		syncMainAlt.SetSync(set);
	}

	public void SetSyncTension(bool set)
	{
		syncTension.SetSync(set);
	}

	public void SetSyncExtra(bool set)
	{
		syncExtra.SetSync(set);
	}
}
