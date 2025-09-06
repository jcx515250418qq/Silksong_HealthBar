using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class CorpseRegular : Corpse, AntRegion.ICheck
{
	[Space]
	[SerializeField]
	private FlingUtils.ChildrenConfig flingChildren = new FlingUtils.ChildrenConfig
	{
		SpeedMin = 15f,
		SpeedMax = 20f,
		AngleMin = 60f,
		AngleMax = 120f
	};

	[Space]
	[SerializeField]
	private CorpseRegularEffectsProfile profile;

	[SerializeField]
	private bool doLandInstantly;

	[SerializeField]
	private bool explodeOnLand;

	[SerializeField]
	private GameObject activateOnExplode;

	[SerializeField]
	private GameObject activateOnStunEnd;

	[SerializeField]
	private bool desaturateOnLand;

	[SerializeField]
	private bool spriteFacesRight;

	[SerializeField]
	private RandomAudioClipTable startVoiceAudioTable;

	[SerializeField]
	private RandomAudioClipTable stunEndVoiceAudioTable;

	[SerializeField]
	private RandomAudioClipTable landVoiceAudioTable;

	[SerializeField]
	private AudioSource audioLoopVoice;

	private GameObject loopingStunEffect;

	protected override bool DoLandEffectsInstantly => doLandInstantly;

	public bool CanEnterAntRegion { get; private set; } = true;

	protected override bool DesaturateOnLand => desaturateOnLand;

	public override bool OnAwake()
	{
		if (base.OnAwake() && (bool)profile)
		{
			profile.EnsurePersonalPool(base.gameObject);
		}
		return false;
	}

	private new void OnDisable()
	{
		if (loopingStunEffect != null)
		{
			if (loopingStunEffect.activeSelf)
			{
				loopingStunEffect.Recycle();
			}
			loopingStunEffect = null;
		}
	}

	protected override void Begin()
	{
		if ((bool)startVoiceAudioTable && (bool)audioLoopVoice)
		{
			audioLoopVoice.Stop();
			audioLoopVoice.loop = false;
			audioLoopVoice.clip = startVoiceAudioTable.SelectClip();
			audioLoopVoice.volume = startVoiceAudioTable.SelectVolume();
			audioLoopVoice.pitch = startVoiceAudioTable.SelectPitch();
			audioLoopVoice.Play();
		}
		if ((bool)profile)
		{
			Vector2 pos = base.transform.position;
			GameObject[] spawnOnStart = profile.SpawnOnStart;
			foreach (GameObject effectPrefab in spawnOnStart)
			{
				SpawnEffect(effectPrefab, pos);
			}
			if (profile.StunTime > 0f)
			{
				PlayStartAudio();
				StartCoroutine(DoStunEffects());
				return;
			}
		}
		base.Begin();
	}

	public bool SpawnElementalEffects(ElementalEffectType elementType)
	{
		if (elementType == ElementalEffectType.None)
		{
			return false;
		}
		if (profile == null)
		{
			return false;
		}
		if (elementType < ElementalEffectType.None || (int)elementType >= profile.ElementalEffects.Length)
		{
			return false;
		}
		CorpseRegularEffectsProfile.EffectList effectList = profile.ElementalEffects[(int)elementType];
		if (effectList == null)
		{
			return false;
		}
		Vector2 pos = base.transform.position;
		foreach (GameObject effect in effectList.effects)
		{
			SpawnEffect(effect, pos);
		}
		return effectList.effects.Count > 0;
	}

	protected override void LandEffects()
	{
		base.LandEffects();
		StartCoroutine(DoLandEffects(splashed));
	}

	private IEnumerator DoStunEffects()
	{
		CanEnterAntRegion = false;
		if ((bool)body)
		{
			body.isKinematic = true;
			body.linearVelocity = Vector2.zero;
		}
		if ((bool)spriteFlash)
		{
			spriteFlash.FlashingSuperDash();
		}
		if ((bool)spriteAnimator && !spriteAnimator.TryPlay("Death Stun"))
		{
			spriteAnimator.Play("Death Air");
		}
		if (loopingStunEffect != null)
		{
			loopingStunEffect.Recycle();
		}
		loopingStunEffect = (profile.LoopingStunEffectPrefab ? profile.LoopingStunEffectPrefab.Spawn(base.transform.position) : null);
		yield return new WaitForSeconds(profile.StunTime);
		if (loopingStunEffect != null)
		{
			ParticleSystem[] componentsInChildren = loopingStunEffect.GetComponentsInChildren<ParticleSystem>();
			loopingStunEffect = null;
			ParticleSystem[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
			}
		}
		CanEnterAntRegion = true;
		if ((bool)body)
		{
			body.isKinematic = false;
			bool flag = false;
			if ((!spriteFacesRight && base.transform.localScale.x < 0f) || (spriteFacesRight && base.transform.localScale.x > 0f))
			{
				flag = true;
			}
			if (!flag)
			{
				body.linearVelocity = new Vector2(10f, 15f);
			}
			else
			{
				body.linearVelocity = new Vector2(-10f, 15f);
			}
		}
		if ((bool)spriteFlash)
		{
			spriteFlash.CancelFlash();
		}
		if ((bool)activateOnStunEnd)
		{
			activateOnStunEnd.SetActive(value: true);
		}
		if ((bool)stunEndVoiceAudioTable && (bool)audioLoopVoice)
		{
			audioLoopVoice.Stop();
			audioLoopVoice.loop = false;
			audioLoopVoice.clip = stunEndVoiceAudioTable.SelectClip();
			audioLoopVoice.volume = stunEndVoiceAudioTable.SelectVolume();
			audioLoopVoice.pitch = stunEndVoiceAudioTable.SelectPitch();
			audioLoopVoice.Play();
		}
		profile.StunEndShake.DoShake(this);
		Vector2 pos = base.transform.position;
		GameObject[] spawnOnStunEnd = profile.SpawnOnStunEnd;
		foreach (GameObject effectPrefab in spawnOnStunEnd)
		{
			SpawnEffect(effectPrefab, pos);
		}
		base.Begin();
	}

	private IEnumerator DoLandEffects(bool didSplash)
	{
		if ((bool)flingChildren.Parent)
		{
			FlingUtils.ChildrenConfig config = flingChildren;
			config.Parent.SetActive(value: true);
			config.Parent.transform.SetParent(null, worldPositionStays: true);
			if (base.transform.localScale.x < 0f)
			{
				config.AngleMin = Helper.GetReflectedAngle(config.AngleMin, reflectHorizontal: true, reflectVertical: false);
				config.AngleMax = Helper.GetReflectedAngle(config.AngleMax, reflectHorizontal: true, reflectVertical: false);
			}
			FlingUtils.FlingChildren(config, base.transform, Vector3.zero, new MinMaxFloat(0f, 0.001f));
		}
		Vector2 spawnPos = base.transform.position;
		if ((bool)profile)
		{
			GameObject[] spawnOnLand = profile.SpawnOnLand;
			foreach (GameObject effectPrefab in spawnOnLand)
			{
				SpawnEffect(effectPrefab, spawnPos);
			}
		}
		if ((bool)landVoiceAudioTable && (bool)audioLoopVoice)
		{
			audioLoopVoice.Stop();
			audioLoopVoice.loop = false;
			audioLoopVoice.clip = landVoiceAudioTable.SelectClip();
			audioLoopVoice.volume = landVoiceAudioTable.SelectVolume();
			audioLoopVoice.pitch = landVoiceAudioTable.SelectPitch();
			audioLoopVoice.Play();
		}
		if (!explodeOnLand)
		{
			yield break;
		}
		if (didSplash && splashLandDelay > 0f)
		{
			yield return new WaitForSeconds(splashLandDelay);
		}
		if ((bool)body)
		{
			body.linearVelocity = Vector2.zero;
			body.angularVelocity = 0f;
		}
		if ((bool)profile)
		{
			BloodSpawner.SpawnBlood(profile.ExplodeBlood, base.transform, bloodColorOverride);
		}
		if ((bool)splatAudioClipTable)
		{
			splatAudioClipTable.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		}
		if ((bool)activateOnExplode)
		{
			activateOnExplode.SetActive(value: true);
		}
		if (audioLoopVoice != null)
		{
			audioLoopVoice.Stop();
		}
		if ((bool)spriteAnimator)
		{
			yield return StartCoroutine(spriteAnimator.PlayAnimWait("Death Land"));
		}
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = false;
		}
		if (profile != null && profile.SpawnOnExplode != null)
		{
			GameObject[] spawnOnLand = profile.SpawnOnExplode;
			foreach (GameObject effectPrefab2 in spawnOnLand)
			{
				SpawnEffect(effectPrefab2, spawnPos);
			}
		}
		BoxCollider2D component = GetComponent<BoxCollider2D>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		if ((bool)body)
		{
			body.isKinematic = true;
		}
	}

	private void SpawnEffect(GameObject effectPrefab, Vector2 pos)
	{
		GameObject gameObject = effectPrefab.Spawn();
		gameObject.transform.SetPosition2D(pos);
		float blackThreadAmount = GetBlackThreadAmount();
		if (blackThreadAmount > 0f)
		{
			BlackThreadEffectRendererGroup component = gameObject.GetComponent<BlackThreadEffectRendererGroup>();
			if (component != null)
			{
				component.SetBlackThreadAmount(blackThreadAmount);
			}
		}
		if (bloodColorOverride.HasValue)
		{
			SpawnedCorpseEffectTint component2 = gameObject.GetComponent<SpawnedCorpseEffectTint>();
			if ((bool)component2)
			{
				component2.SetTint(bloodColorOverride.Value);
			}
		}
	}
}
