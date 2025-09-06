using System.Collections;
using UnityEngine;

public class DreamPlant : MonoBehaviour
{
	public HeroDetect heroDetector;

	public AudioClip glowSound;

	private AudioSource audioSource;

	public ParticleSystem wiltedParticles;

	[Space]
	public ColorFader glowFader;

	public ColorFader completeGlowFader;

	[Space]
	public AudioClip hitSound;

	public GameObject dreamImpact;

	public GameObject dreamAreaEffect;

	private GameObject spawnedDreamAreaEffect;

	public ParticleSystem activateParticles;

	public ParticleSystem activatedParticles;

	public GameObject whiteFlash;

	[Space]
	public AudioClip growChargeSound;

	public AudioClip growSound;

	public ParticleSystem completeChargeParticles;

	public ParticleSystem growParticles;

	public GameObject dreamDialogue;

	[Space]
	public string playerdataBool;

	private tk2dSpriteAnimator anim;

	private bool activated;

	private bool completed;

	private bool hasDreamNail;

	private bool seenDreamNailPrompt;

	private int spawnedOrbs;

	private Coroutine checkOrbRoutine;

	private DreamPlantOrb[] dreamOrbs;

	private SpriteFlash spriteFlash;

	private void Awake()
	{
		spriteFlash = GetComponent<SpriteFlash>();
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if ((bool)component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = completed;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				completed = value;
				if (completed)
				{
					activated = true;
					if ((bool)anim)
					{
						anim.Play("Completed");
					}
					if ((bool)dreamDialogue)
					{
						dreamDialogue.SetActive(value: true);
					}
				}
			};
		}
		audioSource = GetComponent<AudioSource>();
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	private void Start()
	{
		hasDreamNail = GameManager.instance.GetPlayerDataBool("hasDreamNail");
		seenDreamNailPrompt = GameManager.instance.GetPlayerDataBool("seenDreamNailPrompt");
		if ((bool)heroDetector && hasDreamNail)
		{
			heroDetector.OnEnter += delegate
			{
				ShowPrompt(show: true);
			};
			heroDetector.OnExit += delegate
			{
				ShowPrompt(show: false);
			};
		}
		if (completed && playerdataBool != "")
		{
			GameManager.instance.SetPlayerDataBool(playerdataBool, value: true);
		}
		if (hasDreamNail && !activated && (bool)dreamAreaEffect)
		{
			spawnedDreamAreaEffect = Object.Instantiate(dreamAreaEffect);
			spawnedDreamAreaEffect.SetActive(value: false);
		}
		if ((bool)whiteFlash)
		{
			whiteFlash.SetActive(value: true);
			whiteFlash.SetActive(value: false);
		}
		dreamOrbs = Object.FindObjectsOfType<DreamPlantOrb>();
		DreamPlantOrb.plant = this;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (activated || !(collision.tag == "Dream Attack"))
		{
			return;
		}
		activated = true;
		DreamPlantOrb[] array = dreamOrbs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Show();
		}
		if ((bool)spriteFlash)
		{
			spriteFlash.flashFocusHeal();
		}
		if ((bool)glowFader)
		{
			glowFader.Fade(up: false);
		}
		if ((bool)anim)
		{
			anim.Play("Activate");
		}
		if ((bool)audioSource && (bool)hitSound)
		{
			audioSource.PlayOneShot(hitSound);
		}
		if ((bool)spawnedDreamAreaEffect)
		{
			spawnedDreamAreaEffect.SetActive(value: true);
		}
		if ((bool)whiteFlash)
		{
			whiteFlash.SetActive(value: true);
		}
		if ((bool)activateParticles)
		{
			activateParticles.gameObject.SetActive(value: true);
		}
		if ((bool)activatedParticles)
		{
			activatedParticles.gameObject.SetActive(value: true);
		}
		if ((bool)dreamImpact)
		{
			Vector3 center = collision.bounds.center;
			Collider2D component = GetComponent<Collider2D>();
			if ((bool)component)
			{
				center += component.bounds.center;
				center /= 2f;
			}
			dreamImpact.Spawn(center);
		}
		GameCameras instance = GameCameras.instance;
		if ((bool)instance)
		{
			instance.cameraShakeFSM.SendEvent("AverageShake");
		}
		EventRegister.SendEvent(EventRegisterEvents.DreamPlantHit);
	}

	public void AddOrbCount()
	{
		spawnedOrbs++;
		if (checkOrbRoutine == null)
		{
			checkOrbRoutine = StartCoroutine(CheckOrbs());
		}
	}

	public void RemoveOrbCount()
	{
		spawnedOrbs--;
	}

	private void ShowPrompt(bool show)
	{
		if (activated)
		{
			return;
		}
		if (show)
		{
			if (!seenDreamNailPrompt)
			{
				seenDreamNailPrompt = true;
				GameManager.instance.SetPlayerDataBool("seenDreamNailPrompt", value: true);
				PlayMakerFSM.BroadcastEvent("REMINDER DREAM NAIL");
			}
			if ((bool)audioSource && (bool)glowSound)
			{
				audioSource.PlayOneShot(glowSound);
			}
			if ((bool)wiltedParticles)
			{
				wiltedParticles.Play();
			}
			SendMessage("flashWhitePulse");
			if ((bool)glowFader)
			{
				glowFader.Fade(up: true);
			}
		}
		else if ((bool)glowFader)
		{
			glowFader.Fade(up: false);
		}
	}

	private IEnumerator CheckOrbs()
	{
		while (spawnedOrbs > 0)
		{
			yield return null;
		}
		completed = true;
		if (playerdataBool != "")
		{
			GameManager.instance.SetPlayerDataBool(playerdataBool, value: true);
		}
		GameManager.instance.SendMessage("AddToDreamPlantCList");
		yield return new WaitForSeconds(1f);
		PlayMakerFSM.BroadcastEvent("DREAM AREA DISABLE");
		if ((bool)activatedParticles)
		{
			activatedParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		if ((bool)completeGlowFader)
		{
			completeGlowFader.Fade(up: true);
		}
		if ((bool)audioSource && (bool)growChargeSound)
		{
			audioSource.PlayOneShot(growChargeSound);
		}
		if ((bool)completeChargeParticles)
		{
			completeChargeParticles.gameObject.SetActive(value: true);
		}
		yield return new WaitForSeconds(1f);
		if ((bool)completeChargeParticles)
		{
			completeChargeParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		if ((bool)audioSource && (bool)growSound)
		{
			audioSource.PlayOneShot(growSound);
		}
		if ((bool)anim)
		{
			anim.Play("Complete");
		}
		if ((bool)whiteFlash)
		{
			whiteFlash.SetActive(value: true);
		}
		if ((bool)completeGlowFader)
		{
			completeGlowFader.Fade(up: false);
		}
		if ((bool)growParticles)
		{
			growParticles.gameObject.SetActive(value: true);
		}
		GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
		if ((bool)gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("AverageShake");
		}
		if ((bool)dreamDialogue)
		{
			dreamDialogue.SetActive(value: true);
		}
	}
}
