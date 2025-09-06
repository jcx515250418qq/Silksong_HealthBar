using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BossStatue : MonoBehaviour
{
	public delegate void StatueSwapEndEvent();

	public delegate void SeenNewStatueEvent();

	[Serializable]
	public struct Completion
	{
		public bool hasBeenSeen;

		public bool isUnlocked;

		public bool completedTier1;

		public bool completedTier2;

		public bool completedTier3;

		public bool seenTier3Unlock;

		public bool usingAltVersion;

		public static Completion None
		{
			get
			{
				Completion result = default(Completion);
				result.hasBeenSeen = false;
				result.isUnlocked = false;
				result.completedTier1 = false;
				result.completedTier2 = false;
				result.completedTier3 = false;
				result.seenTier3Unlock = false;
				result.usingAltVersion = false;
				return result;
			}
		}
	}

	[Serializable]
	public struct BossUIDetails
	{
		public string nameKey;

		public string nameSheet;

		public string descriptionKey;

		public string descriptionSheet;
	}

	[Header("Boss Data")]
	public BossScene bossScene;

	public BossScene dreamBossScene;

	[Header("Statue Data")]
	[FormerlySerializedAs("statueStateInt")]
	public string statueStatePD;

	public BossUIDetails bossDetails;

	[Space]
	[FormerlySerializedAs("dreamStatueStateInt")]
	public string dreamStatueStatePD;

	public BossUIDetails dreamBossDetails;

	[Space]
	public bool hasNoTiers;

	public bool dontCountCompletion;

	public bool isAlwaysUnlocked;

	public bool isAlwaysUnlockedDream;

	public float inspectCameraHeight = 5.5f;

	public bool isHidden;

	[Header("Prefab Stuff")]
	public PlayMakerFSM bossUIControlFSM;

	[Space]
	public GameObject[] disableIfLocked;

	public GameObject[] enableIfLocked;

	public BossStatueTrophyPlaque regularPlaque;

	public BossStatueTrophyPlaque altPlaqueL;

	public BossStatueTrophyPlaque altPlaqueR;

	public SpriteRenderer lockedPlaque;

	[Space]
	public GameObject dreamReturnGate;

	public TriggerEnterEvent lightTrigger;

	public CameraLockArea cameraLock;

	[Header("Animation")]
	public GameObject statueDisplay;

	public GameObject statueDisplayAlt;

	public ParticleSystem statueShakeParticles;

	public ParticleSystem statueUpParticles;

	public AudioSource statueShakeLoop;

	public AudioSource audioSourcePrefab;

	public AudioEvent statueDownSound;

	public float statueDownSoundDelay;

	public AudioEvent statueUpSound;

	public float statueUpSoundDelay = 0.3f;

	public float swapWaitTime = 0.25f;

	public float shakeTime = 1f;

	public float holdTime = 0.5f;

	public float upParticleDelay = 0.25f;

	private IBossStatueToggle dreamToggle;

	private bool wasUsingDreamVersion;

	public bool UsingDreamVersion
	{
		get
		{
			return StatueState.usingAltVersion;
		}
		private set
		{
			Completion statueState = StatueState;
			statueState.usingAltVersion = value;
			StatueState = statueState;
		}
	}

	public Completion StatueState
	{
		get
		{
			if (string.IsNullOrEmpty(statueStatePD))
			{
				return Completion.None;
			}
			Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<Completion>(statueStatePD);
			if (!playerDataVariable.isUnlocked && (bool)bossScene && (bossScene.IsUnlocked(BossSceneCheckSource.Statue) || isAlwaysUnlocked))
			{
				playerDataVariable.isUnlocked = true;
			}
			return playerDataVariable;
		}
		set
		{
			if (!string.IsNullOrEmpty(statueStatePD))
			{
				GameManager.instance.SetPlayerDataVariable(statueStatePD, value);
			}
		}
	}

	public Completion DreamStatueState
	{
		get
		{
			if (string.IsNullOrEmpty(dreamStatueStatePD))
			{
				return Completion.None;
			}
			Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<Completion>(dreamStatueStatePD);
			if (!playerDataVariable.isUnlocked && (bool)dreamBossScene && (dreamBossScene.IsUnlocked(BossSceneCheckSource.Statue) || isAlwaysUnlockedDream))
			{
				playerDataVariable.isUnlocked = true;
			}
			return playerDataVariable;
		}
		set
		{
			if (!string.IsNullOrEmpty(dreamStatueStatePD))
			{
				GameManager.instance.SetPlayerDataVariable(dreamStatueStatePD, value);
			}
		}
	}

	public bool HasRegularVersion
	{
		get
		{
			if ((bool)bossScene)
			{
				return !string.IsNullOrEmpty(statueStatePD);
			}
			return false;
		}
	}

	public bool HasDreamVersion
	{
		get
		{
			if ((bool)dreamBossScene)
			{
				return !string.IsNullOrEmpty(dreamStatueStatePD);
			}
			return false;
		}
	}

	public event StatueSwapEndEvent OnStatueSwapFinished;

	public event SeenNewStatueEvent OnSeenNewStatue;

	private void Awake()
	{
		dreamToggle = GetComponentInChildren<IBossStatueToggle>(includeInactive: false);
		if ((bool)dreamReturnGate)
		{
			GameObject obj = dreamReturnGate;
			obj.name = obj.name + "_" + base.gameObject.name;
		}
		if ((bool)cameraLock)
		{
			cameraLock.cameraYMin = (cameraLock.cameraYMax = base.transform.position.y + inspectCameraHeight);
		}
	}

	private void Start()
	{
		UpdateDetails();
		if (StatueState.isUnlocked)
		{
			GameObject[] array = disableIfLocked;
			foreach (GameObject gameObject in array)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(value: true);
				}
			}
			array = enableIfLocked;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			if ((bool)statueDisplay)
			{
				statueDisplay.SetActive(value: true);
			}
		}
		else
		{
			GameObject[] array = disableIfLocked;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			array = enableIfLocked;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			if ((bool)statueDisplay)
			{
				statueDisplay.SetActive(value: false);
			}
		}
		if ((bool)statueDisplayAlt)
		{
			statueDisplayAlt.SetActive(value: false);
		}
		if (dreamToggle != null)
		{
			dreamToggle.SetOwner(this);
		}
		if (DreamStatueState.isUnlocked)
		{
			if (dreamToggle != null)
			{
				dreamToggle.SetState(value: true);
			}
		}
		else if (dreamToggle != null)
		{
			dreamToggle.SetState(value: false);
		}
		Animator component = statueDisplay.GetComponent<Animator>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		component = statueDisplayAlt.GetComponent<Animator>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		if ((bool)lightTrigger)
		{
			lightTrigger.OnTriggerEntered += delegate
			{
				bool flag = false;
				Completion statueState = StatueState;
				if (statueState.isUnlocked && !statueState.hasBeenSeen && !isAlwaysUnlocked)
				{
					statueState.hasBeenSeen = true;
					StatueState = statueState;
					flag = true;
				}
				statueState = DreamStatueState;
				if (statueState.isUnlocked && !statueState.hasBeenSeen && !isAlwaysUnlockedDream)
				{
					statueState.hasBeenSeen = true;
					DreamStatueState = statueState;
					flag = true;
				}
				if (flag && this.OnSeenNewStatue != null)
				{
					this.OnSeenNewStatue();
				}
			};
		}
		SetPlaquesVisible((StatueState.isUnlocked && StatueState.hasBeenSeen) || isAlwaysUnlocked);
	}

	public void SetPlaquesVisible(bool isEnabled)
	{
		regularPlaque.gameObject.SetActive(value: false);
		altPlaqueL.gameObject.SetActive(value: false);
		altPlaqueR.gameObject.SetActive(value: false);
		if (isEnabled)
		{
			if ((bool)bossScene && !dreamBossScene)
			{
				regularPlaque.gameObject.SetActive(value: true);
				SetPlaqueState(StatueState, regularPlaque, statueStatePD);
			}
			else if ((bool)bossScene && (bool)dreamBossScene)
			{
				altPlaqueL.gameObject.SetActive(value: true);
				altPlaqueR.gameObject.SetActive(value: true);
				SetPlaqueState(StatueState, altPlaqueL, statueStatePD);
				SetPlaqueState(DreamStatueState, altPlaqueR, dreamStatueStatePD);
			}
		}
		lockedPlaque.enabled = !isEnabled;
	}

	public void SetPlaqueState(Completion statueState, BossStatueTrophyPlaque plaque, string playerDataKey)
	{
		PlayerData playerData = GameManager.instance.playerData;
		if (string.IsNullOrEmpty(playerData.currentBossStatueCompletionKey) || playerData.currentBossStatueCompletionKey != playerDataKey)
		{
			plaque.SetDisplay(BossStatueTrophyPlaque.GetDisplayType(statueState));
			return;
		}
		BossStatueTrophyPlaque.DisplayType displayType = BossStatueTrophyPlaque.GetDisplayType(statueState);
		plaque.SetDisplay(displayType);
		plaque.DoTierCompleteEffect((BossStatueTrophyPlaque.DisplayType)playerData.bossStatueTargetLevel);
		playerData.currentBossStatueCompletionKey = "";
		playerData.bossStatueTargetLevel = -1;
	}

	public void SetDreamVersion(bool value, bool useAltStatue = false, bool doAnim = true)
	{
		UsingDreamVersion = value;
		if (useAltStatue && (bool)statueDisplayAlt && (bool)statueDisplay)
		{
			StartCoroutine(SwapStatues(doAnim));
		}
		else if (this.OnStatueSwapFinished != null)
		{
			this.OnStatueSwapFinished();
		}
		wasUsingDreamVersion = value;
		UpdateDetails();
	}

	private void UpdateDetails()
	{
		if ((bool)bossUIControlFSM)
		{
			BossUIDetails bossUIDetails = (UsingDreamVersion ? dreamBossDetails : bossDetails);
			bossUIControlFSM.FsmVariables.FindFsmString("Boss Name Key").Value = bossUIDetails.nameKey;
			bossUIControlFSM.FsmVariables.FindFsmString("Boss Name Sheet").Value = bossUIDetails.nameSheet;
			bossUIControlFSM.FsmVariables.FindFsmString("Description Key").Value = bossUIDetails.descriptionKey;
			bossUIControlFSM.FsmVariables.FindFsmString("Description Sheet").Value = bossUIDetails.descriptionSheet;
		}
	}

	private IEnumerator SwapStatues(bool doAnim)
	{
		GameObject current = (wasUsingDreamVersion ? statueDisplayAlt : statueDisplay);
		GameObject next = (wasUsingDreamVersion ? statueDisplay : statueDisplayAlt);
		if (doAnim)
		{
			if ((bool)bossUIControlFSM)
			{
				FSMUtility.SendEventToGameObject(bossUIControlFSM.gameObject, "NPC CONTROL OFF");
			}
			yield return new WaitForSeconds(swapWaitTime);
			if ((bool)statueShakeParticles)
			{
				statueShakeParticles.Play();
			}
			if ((bool)statueShakeLoop)
			{
				statueShakeLoop.Play();
			}
			yield return StartCoroutine(Jitter(shakeTime, 0.1f, current));
			if ((bool)statueShakeLoop)
			{
				statueShakeLoop.Stop();
			}
			StartCoroutine(PlayAudioEventDelayed(statueDownSound, statueDownSoundDelay));
			yield return StartCoroutine(PlayAnimWait(current.GetComponent<Animator>(), "Down", 0f));
		}
		current.SetActive(value: false);
		if (doAnim)
		{
			yield return new WaitForSeconds(holdTime);
			StartCoroutine(PlayParticlesDelay(statueUpParticles, upParticleDelay));
			StartCoroutine(PlayAudioEventDelayed(statueUpSound, statueUpSoundDelay));
		}
		next.transform.position = current.transform.position;
		next.SetActive(value: true);
		if (doAnim)
		{
			yield return StartCoroutine(PlayAnimWait(next.GetComponent<Animator>(), "Up", 0f));
			if ((bool)bossUIControlFSM)
			{
				FSMUtility.SendEventToGameObject(bossUIControlFSM.gameObject, "CONVO CANCEL");
			}
		}
		if (this.OnStatueSwapFinished != null)
		{
			this.OnStatueSwapFinished();
		}
	}

	private IEnumerator Jitter(float duration, float magnitude, GameObject obj)
	{
		Transform sprite = obj.transform;
		Vector3 initialPos = sprite.position;
		float elapsed = 0f;
		float half = magnitude / 2f;
		for (; elapsed < duration; elapsed += Time.deltaTime)
		{
			sprite.position = initialPos + new Vector3(UnityEngine.Random.Range(0f - half, half), UnityEngine.Random.Range(0f - half, half), 0f);
			yield return null;
		}
		sprite.position = initialPos;
	}

	private IEnumerator PlayAnimWait(Animator animator, string stateName, float normalizedTime)
	{
		if ((bool)animator)
		{
			animator.enabled = true;
			animator.Play(stateName, 0, normalizedTime);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			animator.enabled = false;
		}
	}

	private IEnumerator PlayParticlesDelay(ParticleSystem system, float delay)
	{
		if ((bool)system)
		{
			yield return new WaitForSeconds(delay);
			system.Play();
		}
	}

	private IEnumerator PlayAudioEventDelayed(AudioEvent audioEvent, float delay)
	{
		yield return new WaitForSeconds(delay);
		audioEvent.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position + new Vector3(0f, inspectCameraHeight, 0f), 0.25f);
	}
}
