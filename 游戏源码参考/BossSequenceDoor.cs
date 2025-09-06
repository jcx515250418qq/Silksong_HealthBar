using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSequenceDoor : MonoBehaviour
{
	[Serializable]
	public struct Completion
	{
		public bool canUnlock;

		public bool unlocked;

		public bool completed;

		public bool allBindings;

		public bool noHits;

		public bool boundNail;

		public bool boundShell;

		public bool boundCharms;

		public bool boundSoul;

		public List<string> viewedBossSceneCompletions;

		public static Completion None
		{
			get
			{
				Completion result = default(Completion);
				result.canUnlock = false;
				result.unlocked = false;
				result.completed = false;
				result.allBindings = false;
				result.noHits = false;
				result.boundNail = false;
				result.boundShell = false;
				result.boundCharms = false;
				result.boundSoul = false;
				result.viewedBossSceneCompletions = new List<string>();
				return result;
			}
		}
	}

	[Header("Door-specific")]
	public string playerDataString;

	private Completion completion;

	public BossSequence bossSequence;

	[Space]
	public string titleSuperKey;

	public string titleSuperSheet;

	public string titleMainKey;

	public string titleMainSheet;

	public string descriptionKey;

	public string descriptionSheet;

	[Space]
	public BossSequenceDoor[] requiredComplete;

	[Header("Prefab")]
	public GameObject completedDisplay;

	public GameObject completedAllDisplay;

	public GameObject completedNoHitsDisplay;

	[Space]
	public GameObject boundNailDisplay;

	public GameObject boundHeartDisplay;

	public GameObject boundCharmsDisplay;

	public GameObject boundSoulDisplay;

	public GameObject boundAllDisplay;

	public GameObject boundAllBackboard;

	[Space]
	public GameObject lockSet;

	public GameObject lockInteractPrompt;

	public GameObject cameraLock;

	public GameObject unlockedSet;

	public PlayMakerFSM challengeFSM;

	public GameObject dreamReturnGate;

	[Header("Lock Break Effects")]
	public bool doLockBreakSequence = true;

	public GameObject lockBreakAnticEffects;

	public GameObject lockBreakRumbleSound;

	public SpriteRenderer[] glowSprites;

	public Material spriteFlashMaterial;

	public SpriteRenderer[] fadeSprites;

	public AnimationCurve glowCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public ParticleSystem glowParticles;

	public float lockBreakAnticTime = 3.3f;

	public GameObject lockBreakEffects;

	private bool doUnlockSequence;

	[Space]
	public GameObject lockedUIPrefab;

	private Material material;

	private static BossDoorLockUI lockedUI;

	public Completion CurrentCompletion
	{
		get
		{
			return completion;
		}
		set
		{
			completion = value;
			SaveState();
		}
	}

	private void Start()
	{
		completion = (string.IsNullOrEmpty(playerDataString) ? Completion.None : GameManager.instance.GetPlayerDataVariable<Completion>(playerDataString));
		if (IsUnlocked() || completion.canUnlock)
		{
			SetDisplayState(completion);
			if (completion.unlocked || !doLockBreakSequence)
			{
				if ((bool)lockSet)
				{
					lockSet.SetActive(value: false);
				}
				if ((bool)unlockedSet)
				{
					unlockedSet.SetActive(value: true);
				}
			}
			else
			{
				doUnlockSequence = true;
				if ((bool)lockInteractPrompt)
				{
					lockInteractPrompt.SetActive(value: false);
				}
				if ((bool)unlockedSet)
				{
					unlockedSet.SetActive(value: false);
				}
			}
		}
		else
		{
			SetDisplayState(Completion.None);
			if ((bool)lockSet)
			{
				lockSet.SetActive(value: true);
			}
			if ((bool)unlockedSet)
			{
				unlockedSet.SetActive(value: false);
			}
			if ((bool)lockedUIPrefab && !lockedUI)
			{
				GameObject obj = UnityEngine.Object.Instantiate(lockedUIPrefab);
				lockedUI = obj.GetComponent<BossDoorLockUI>();
				obj.SetActive(value: false);
			}
		}
		if ((bool)challengeFSM && (bool)bossSequence && bossSequence.Count > 0)
		{
			challengeFSM.FsmVariables.FindFsmString("To Scene").Value = bossSequence.GetSceneAt(0);
		}
		if ((bool)dreamReturnGate)
		{
			GameObject obj2 = dreamReturnGate;
			obj2.name = obj2.name + "_" + base.gameObject.name;
		}
	}

	private void OnDestroy()
	{
		if (material != null)
		{
			UnityEngine.Object.Destroy(material);
			material = null;
		}
	}

	private void SaveState()
	{
		GameManager.instance.SetPlayerDataVariable(playerDataString, completion);
	}

	private bool IsUnlocked()
	{
		if (completion.unlocked)
		{
			return true;
		}
		if ((bool)bossSequence && bossSequence.IsUnlocked())
		{
			return true;
		}
		return false;
	}

	private void SetDisplayState(Completion completion)
	{
		if ((bool)completedDisplay)
		{
			completedDisplay.SetActive(completion.completed);
		}
		if ((bool)completedAllDisplay)
		{
			completedAllDisplay.SetActive(completion.allBindings);
		}
		if ((bool)completedNoHitsDisplay)
		{
			completedNoHitsDisplay.SetActive(completion.noHits);
		}
		if ((bool)boundAllDisplay)
		{
			boundAllDisplay.SetActive(completion.allBindings);
		}
		if ((bool)boundAllBackboard)
		{
			boundAllBackboard.SetActive(completion.allBindings);
		}
		if ((bool)boundNailDisplay)
		{
			boundNailDisplay.SetActive(completion.boundNail && !completion.allBindings);
		}
		if ((bool)boundHeartDisplay)
		{
			boundHeartDisplay.SetActive(completion.boundShell && !completion.allBindings);
		}
		if ((bool)boundCharmsDisplay)
		{
			boundCharmsDisplay.SetActive(completion.boundCharms && !completion.allBindings);
		}
		if ((bool)boundSoulDisplay)
		{
			boundSoulDisplay.SetActive(completion.boundSoul && !completion.allBindings);
		}
	}

	public void ShowLockUI(bool value)
	{
		if ((bool)lockedUI)
		{
			if (value)
			{
				lockedUI.Show(this);
			}
			else
			{
				lockedUI.Hide();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!doUnlockSequence || !(collision.gameObject.tag == "Player") || !HeroController.instance.isHeroInPosition)
		{
			return;
		}
		BossSequenceDoor[] array = requiredComplete;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].CurrentCompletion.completed)
			{
				return;
			}
		}
		doUnlockSequence = false;
		completion.unlocked = true;
		SaveState();
		StartCoroutine(DoorUnlockSequence());
	}

	private void StartShake()
	{
		FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingMed", value: true);
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HeroController.instance.gameObject, "Roar and Wound States");
		if ((bool)playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Roar Object").Value = base.gameObject;
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR ENTER");
	}

	private void StopShake()
	{
		FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingMed", value: false);
		GameCameras.instance.cameraShakeFSM.SendEvent("StopRumble");
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR EXIT");
	}

	private IEnumerator DoorUnlockSequence()
	{
		StartShake();
		if ((bool)cameraLock)
		{
			cameraLock.SetActive(value: true);
		}
		if ((bool)lockBreakAnticEffects)
		{
			lockBreakAnticEffects.SetActive(value: true);
		}
		if ((bool)lockBreakRumbleSound)
		{
			lockBreakRumbleSound.SetActive(value: true);
		}
		if ((bool)glowParticles)
		{
			ParticleSystem.MainModule main = glowParticles.main;
			main.duration = lockBreakAnticTime;
			glowParticles.Play();
		}
		if (glowSprites.Length != 0)
		{
			if (material == null)
			{
				material = new Material(spriteFlashMaterial);
			}
			SpriteRenderer[] array = glowSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sharedMaterial = material;
			}
			Color[] startColors = new Color[fadeSprites.Length];
			for (int j = 0; j < startColors.Length; j++)
			{
				startColors[j] = fadeSprites[j].color;
				fadeSprites[j].color = Color.clear;
				fadeSprites[j].gameObject.SetActive(value: true);
			}
			for (float elapsed = 0f; elapsed < lockBreakAnticTime; elapsed += Time.deltaTime)
			{
				float num = glowCurve.Evaluate(elapsed / lockBreakAnticTime);
				material.SetFloat("_FlashAmount", num);
				for (int k = 0; k < startColors.Length; k++)
				{
					Color color = startColors[k];
					color.a *= num;
					fadeSprites[k].color = color;
				}
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(lockBreakAnticTime);
		}
		StopShake();
		if ((bool)cameraLock)
		{
			cameraLock.SetActive(value: false);
		}
		if ((bool)lockBreakRumbleSound)
		{
			lockBreakRumbleSound.SetActive(value: false);
		}
		if ((bool)lockSet)
		{
			lockSet.SetActive(value: false);
		}
		if ((bool)lockBreakEffects)
		{
			lockBreakEffects.SetActive(value: true);
		}
		if ((bool)unlockedSet)
		{
			unlockedSet.SetActive(value: true);
		}
		GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");
	}
}
