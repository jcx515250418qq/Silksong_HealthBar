using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class RangeAttacker : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects trigger;

	[SerializeField]
	private bool targetInsideState = true;

	[Space]
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingMecanim", false, true, false)]
	private tk2dSpriteAnimator tk2dAnimator;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingMecanim", false, true, false)]
	private tk2dSprite tk2dSprite;

	[Space]
	[SerializeField]
	private bool dontFlipX;

	[SerializeField]
	[Range(0f, 1f)]
	private float appearChance = 1f;

	[SerializeField]
	private MinMaxFloat appearDelay;

	[SerializeField]
	private string appearAnim;

	[SerializeField]
	private AudioEventRandom appearAudio;

	[SerializeField]
	private string loopAnim;

	[SerializeField]
	private MinMaxFloat minLoopTime;

	[SerializeField]
	private MinMaxFloat disappearDelay;

	[SerializeField]
	private string disappearAnim;

	[SerializeField]
	private AudioEventRandom disappearAudio;

	[Space]
	[SerializeField]
	private GameObject damager;

	[SerializeField]
	private TriggerEnterEvent explosionTrigger;

	[SerializeField]
	private GameObject explosionPrefab;

	[SerializeField]
	private AudioSource loopAudioSource;

	[Space]
	[SerializeField]
	private EnemyJournalRecord journalRecord;

	[SerializeField]
	private MinMaxInt journalAmountPerKill = new MinMaxInt(1, 1);

	[Space]
	[SerializeField]
	private TriggerEnterEvent customDamageTrigger;

	[SerializeField]
	private string customDamageEventRegister;

	[SerializeField]
	private Transform sinkTarget;

	[Space]
	[SerializeField]
	private TrackTriggerObjects singRange;

	[SerializeField]
	private TrackTriggerObjects singExcludeRange;

	[SerializeField]
	private MinMaxFloat singAppearDelay;

	[SerializeField]
	private MinMaxFloat singStartDelay;

	[SerializeField]
	private MinMaxFloat singEndDelay;

	[SerializeField]
	private string singAnim;

	[SerializeField]
	private string singEndAnim;

	[Header("Voice")]
	[SerializeField]
	private AudioSource voiceSource;

	[SerializeField]
	private RandomAudioClipTable appearVoice;

	[SerializeField]
	private RandomAudioClipTable hideVoice;

	[SerializeField]
	private RandomAudioClipTable singVoice;

	private MeshRenderer tk2dSpriteRenderer;

	private Collider2D collider;

	private int appearAnimId;

	private int loopAnimId;

	private int disappearAnimId;

	private int singAnimId;

	private int singEndAnimId;

	private Coroutine animRoutine;

	private bool isTargetTrigger;

	private bool isHeroPerforming;

	private bool isOut;

	private bool explosionDisappear;

	private HealthManager healthManager;

	private NeedolinTextOwner needolinTextOwner;

	private static readonly HashSet<AudioSource> _queuedLoopSources = new HashSet<AudioSource>();

	private static AudioSource _previousLoopSource;

	private bool hasSprite;

	private bool hasAnimator;

	private bool hasTk2dSprite;

	private bool hasTk2dSpriteRenderer;

	private bool hasTk2dAnimator;

	private bool hasDamager;

	private bool hasHealthManager;

	private bool hasCollider;

	private bool isChildAttacker;

	private bool hasOrigin;

	private Vector3 origin;

	private bool hasVoiceSource;

	private bool hasAppearVoice;

	private bool hasHideVoice;

	private bool hasSingVoice;

	private bool isSinging;

	private bool isInside;

	private int insideMask;

	public bool HasAppearTrigger { get; private set; }

	public bool IsOut => isOut;

	public static Vector2 LastDamageSinkDirection { get; set; }

	private bool IsUsingMecanim()
	{
		return animator;
	}

	private void Awake()
	{
		appearAnimId = Animator.StringToHash(appearAnim);
		loopAnimId = Animator.StringToHash(loopAnim);
		disappearAnimId = Animator.StringToHash(disappearAnim);
		singAnimId = Animator.StringToHash(singAnim);
		singEndAnimId = Animator.StringToHash(singEndAnim);
		HasAppearTrigger = trigger != null;
		if (HasAppearTrigger)
		{
			trigger.InsideStateChanged += OnInsideStateChanged;
		}
		HeroPerformanceRegion.StartedPerforming += OnHeroStartedPerforming;
		HeroPerformanceRegion.StoppedPerforming += OnHeroStoppedPerforming;
		if (!isChildAttacker)
		{
			if ((bool)explosionTrigger)
			{
				explosionTrigger.OnTriggerEntered += OnExplosionTriggerEntered;
			}
			if ((bool)customDamageTrigger)
			{
				customDamageTrigger.OnTriggerEntered += OnCustomDamageTriggerEntered;
			}
			collider = GetComponent<Collider2D>();
		}
		if ((bool)tk2dSprite)
		{
			tk2dSpriteRenderer = tk2dSprite.GetComponent<MeshRenderer>();
		}
		hasCollider = collider != null;
		healthManager = GetComponent<HealthManager>();
		hasHealthManager = healthManager != null;
		if (hasHealthManager)
		{
			healthManager.TookDamage += delegate
			{
				explosionDisappear = true;
			};
		}
		needolinTextOwner = GetComponent<NeedolinTextOwner>();
		hasVoiceSource = voiceSource != null;
		hasAppearVoice = appearVoice != null;
		hasHideVoice = hideVoice != null;
		hasSingVoice = singVoice != null;
	}

	private void Start()
	{
		hasAnimator = animator;
		hasSprite = sprite;
		hasTk2dAnimator = tk2dAnimator;
		hasTk2dSprite = tk2dSprite;
		hasTk2dSpriteRenderer = tk2dSpriteRenderer;
		if (hasAnimator)
		{
			animator.enabled = true;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.Play(disappearAnim, 0, 1f);
			animator.enabled = false;
			animator.Update(0f);
		}
		else if (hasTk2dAnimator)
		{
			tk2dSpriteAnimationClip clipByName = tk2dAnimator.GetClipByName(disappearAnim);
			if (clipByName != null)
			{
				tk2dAnimator.PlayFromFrame(clipByName, clipByName.frames.Length - 1);
			}
		}
		if (hasSprite)
		{
			sprite.enabled = false;
		}
		else if (hasTk2dSpriteRenderer)
		{
			tk2dSpriteRenderer.enabled = false;
		}
		if (!isChildAttacker)
		{
			hasDamager = damager != null;
		}
		if (hasCollider)
		{
			collider.enabled = false;
		}
		if ((bool)loopAudioSource)
		{
			loopAudioSource.Stop();
		}
		if (isChildAttacker)
		{
			OnInsideStateChanged(isInside: false);
		}
		else if ((bool)trigger)
		{
			OnInsideStateChanged(trigger.IsInside);
		}
	}

	private void OnDestroy()
	{
		if ((bool)trigger)
		{
			trigger.InsideStateChanged -= OnInsideStateChanged;
		}
		HeroPerformanceRegion.StartedPerforming -= OnHeroStartedPerforming;
		HeroPerformanceRegion.StoppedPerforming -= OnHeroStoppedPerforming;
		SetAudioLoopActive(value: false);
	}

	private void OnDisable()
	{
		insideMask = 0;
	}

	private void OnInsideStateChanged(bool isInside)
	{
		this.isInside = isInside;
		bool flag = isInside == targetInsideState;
		if (!flag || !(Random.Range(0f, 1f) > appearChance))
		{
			isTargetTrigger = flag;
			if (isTargetTrigger)
			{
				EnsureAnimStarted();
			}
		}
	}

	public void SetInsideState(int groupMask, bool isInside)
	{
		if (isInside)
		{
			insideMask |= groupMask;
		}
		else
		{
			insideMask &= ~groupMask;
		}
		SetInsideState(insideMask != 0);
	}

	public void SetInsideState(bool isInside)
	{
		if (this.isInside != isInside)
		{
			OnInsideStateChanged(isInside);
		}
	}

	private void OnHeroStartedPerforming()
	{
		if ((bool)singRange && singRange.IsInside && (!singExcludeRange || !singExcludeRange.transform.IsOnHeroPlane() || !singExcludeRange.IsInside))
		{
			isHeroPerforming = true;
			EnsureAnimStarted();
		}
	}

	private void OnHeroStoppedPerforming()
	{
		isHeroPerforming = false;
		if ((bool)needolinTextOwner && needolinTextOwner.RangeCheck == NeedolinTextOwner.NeedolinRangeCheckTypes.Manual)
		{
			needolinTextOwner.RemoveNeedolinText();
		}
	}

	private void EnsureAnimStarted()
	{
		if (animRoutine == null)
		{
			animRoutine = StartCoroutine(Anim());
		}
	}

	private IEnumerator Anim()
	{
		HeroController hc = HeroController.SilentInstance;
		if (!hc)
		{
			yield break;
		}
		while (isTargetTrigger || isHeroPerforming)
		{
			if (isHeroPerforming)
			{
				float waitTimeLeft = singAppearDelay.GetRandomValue();
				while (waitTimeLeft > 0f && isHeroPerforming)
				{
					waitTimeLeft -= Time.deltaTime;
					yield return null;
				}
				if (!isHeroPerforming && !isTargetTrigger)
				{
					break;
				}
			}
			explosionDisappear = false;
			if (!dontFlipX && Random.Range(0, 2) == 0)
			{
				base.transform.FlipLocalScale(x: true);
			}
			float appearDelayLeft = appearDelay.GetRandomValue();
			while (appearDelayLeft > 0f)
			{
				appearDelayLeft -= Time.deltaTime;
				yield return null;
			}
			if (hasSprite)
			{
				sprite.enabled = true;
			}
			else if (hasTk2dSpriteRenderer)
			{
				tk2dSpriteRenderer.enabled = true;
			}
			isOut = true;
			PlayAppearAudio();
			SetAudioLoopActive(value: true);
			yield return StartCoroutine(PlayAnimWait(appearAnim, appearAnimId));
			if (base.transform.IsOnHeroPlane() && hasCollider)
			{
				collider.enabled = true;
			}
			PlayAnim(loopAnim, loopAnimId);
			yield return null;
			if (hasAnimator)
			{
				animator.cullingMode = AnimatorCullingMode.CullCompletely;
			}
			while (true)
			{
				float waitTimeLeft;
				if (isHeroPerforming)
				{
					waitTimeLeft = singStartDelay.GetRandomValue();
					while (waitTimeLeft > 0f)
					{
						waitTimeLeft -= Time.deltaTime;
						yield return null;
					}
					PlayAnim(singAnim, singAnimId);
					ToggleSingLoop(sing: true);
					if (isHeroPerforming && (bool)needolinTextOwner && needolinTextOwner.RangeCheck == NeedolinTextOwner.NeedolinRangeCheckTypes.Manual)
					{
						needolinTextOwner.AddNeedolinText();
					}
					while (isHeroPerforming)
					{
						yield return null;
					}
					waitTimeLeft = singEndDelay.GetRandomValue();
					while (waitTimeLeft > 0f && !explosionDisappear)
					{
						waitTimeLeft -= Time.deltaTime;
						yield return null;
					}
					ToggleSingLoop(sing: false);
					if (!explosionDisappear)
					{
						yield return StartCoroutine(PlayAnimWait(singEndAnim, singEndAnimId));
					}
					PlayAnim(loopAnim, loopAnimId);
					yield return null;
					if (hasAnimator)
					{
						animator.cullingMode = AnimatorCullingMode.CullCompletely;
					}
				}
				waitTimeLeft = minLoopTime.GetRandomValue();
				while (waitTimeLeft > 0f && !explosionDisappear)
				{
					waitTimeLeft -= Time.deltaTime;
					yield return null;
				}
				do
				{
					IL_041a:
					if (isTargetTrigger && !explosionDisappear)
					{
						yield return null;
						continue;
					}
					bool skipDisappear = false;
					while (hc.cState.hazardDeath || hc.cState.dead)
					{
						skipDisappear = true;
						yield return null;
					}
					if (skipDisappear)
					{
						if (hasCollider)
						{
							collider.enabled = false;
						}
					}
					else
					{
						if (!explosionDisappear)
						{
							waitTimeLeft = disappearDelay.GetRandomValue();
							while (waitTimeLeft > 0f)
							{
								waitTimeLeft -= Time.deltaTime;
								yield return null;
							}
							if (isTargetTrigger)
							{
								goto IL_041a;
							}
						}
						if (hasCollider)
						{
							collider.enabled = false;
						}
						PlayHideAudio();
						yield return StartCoroutine(PlayAnimWait(disappearAnim, disappearAnimId));
					}
					SetAudioLoopActive(value: false);
					if (hasAnimator)
					{
						animator.enabled = false;
					}
					if (hasSprite)
					{
						sprite.enabled = false;
					}
					else if (hasTk2dSpriteRenderer)
					{
						tk2dSpriteRenderer.enabled = false;
					}
					isOut = false;
					if ((bool)healthManager)
					{
						healthManager.RefillHP();
					}
					goto end_IL_021b;
				}
				while (!isHeroPerforming);
				continue;
				end_IL_021b:
				break;
			}
		}
		animRoutine = null;
	}

	private void PlayAnim(string animName, int animId)
	{
		if (hasAnimator)
		{
			animator.enabled = true;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.Play(animId);
		}
		else if (hasTk2dAnimator)
		{
			tk2dAnimator.Play(animName);
		}
	}

	private IEnumerator PlayAnimWait(string animName, int animId)
	{
		if (hasAnimator)
		{
			animator.enabled = true;
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.Play(animId);
			yield return null;
			float waitTimeLeft = animator.GetCurrentAnimatorStateInfo(0).length;
			while (waitTimeLeft > 0f)
			{
				waitTimeLeft -= Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			if (!hasTk2dAnimator)
			{
				yield break;
			}
			tk2dSpriteAnimationClip clipByName = tk2dAnimator.GetClipByName(animName);
			if (clipByName != null)
			{
				tk2dAnimator.Play(clipByName);
				float waitTimeLeft = clipByName.Duration;
				while (waitTimeLeft > 0f)
				{
					waitTimeLeft -= Time.deltaTime;
					yield return null;
				}
			}
		}
	}

	private bool CanDamagerShred(DamageEnemies otherDamager)
	{
		if (otherDamager.attackType == AttackTypes.Explosion || otherDamager.CompareTag("Explosion"))
		{
			return true;
		}
		ToolItem representingTool = otherDamager.RepresentingTool;
		if ((bool)representingTool && (representingTool.DamageFlags & ToolDamageFlags.Shredding) != 0)
		{
			return true;
		}
		return false;
	}

	private void OnExplosionTriggerEntered(Collider2D collision, GameObject sender)
	{
		if (isOut)
		{
			DamageEnemies component = collision.GetComponent<DamageEnemies>();
			if ((bool)component && CanDamagerShred(component))
			{
				ReactToExplosion();
			}
		}
	}

	public void ReactToExplosion()
	{
		if (!base.transform.IsOnHeroPlane())
		{
			return;
		}
		explosionDisappear = true;
		if ((bool)journalRecord)
		{
			int randomValue = journalAmountPerKill.GetRandomValue();
			for (int i = 0; i < randomValue; i++)
			{
				EnemyJournalManager.RecordKill(journalRecord);
			}
		}
		if ((bool)explosionPrefab)
		{
			explosionPrefab.Spawn(base.transform).transform.SetParentReset(base.transform);
		}
	}

	private void OnCustomDamageTriggerEntered(Collider2D collision, GameObject sender)
	{
		if (collision.gameObject.layer == 20 && (!collision.gameObject.GetComponentInParent<HeroController>() || CheatManager.Invincibility != CheatManager.InvincibilityStates.FullInvincible))
		{
			if ((bool)sinkTarget)
			{
				Vector2 vector = base.transform.position;
				LastDamageSinkDirection = ((Vector2)sinkTarget.position - vector).normalized;
			}
			EventRegister.SendEvent(customDamageEventRegister);
		}
	}

	private void SetAudioLoopActive(bool value)
	{
		if (!loopAudioSource || !(value ? _queuedLoopSources.Add(loopAudioSource) : _queuedLoopSources.Remove(loopAudioSource)))
		{
			return;
		}
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		AudioSource audioSource = null;
		float num = float.MaxValue;
		Vector3 position = silentInstance.transform.position;
		foreach (AudioSource queuedLoopSource in _queuedLoopSources)
		{
			float num2 = Vector3.Distance(queuedLoopSource.transform.position, position);
			if (!(num2 > num))
			{
				num = num2;
				audioSource = queuedLoopSource;
			}
		}
		if (audioSource == _previousLoopSource)
		{
			return;
		}
		float num3 = 0f;
		if (audioSource != null)
		{
			double num4 = AudioSettings.dspTime;
			float num5 = 0f;
			if ((bool)_previousLoopSource)
			{
				num5 = 0.3f;
				num4 += (double)num5;
				num3 = _previousLoopSource.time;
				_previousLoopSource.SetScheduledEndTime(num4);
				_previousLoopSource = null;
			}
			float num6 = num3 + num5;
			if ((bool)audioSource.clip)
			{
				float length = audioSource.clip.length;
				if (length > 0f)
				{
					while (num6 > length)
					{
						num6 -= length;
					}
				}
				else
				{
					num6 = 0f;
				}
			}
			audioSource.time = num6;
			audioSource.PlayScheduled(num4);
			_previousLoopSource = audioSource;
		}
		else if ((bool)_previousLoopSource)
		{
			num3 = _previousLoopSource.time;
			_previousLoopSource.Stop();
			_previousLoopSource = null;
		}
	}

	private void PlayVoiceTableUnsafe(RandomAudioClipTable table)
	{
		if (!hasVoiceSource)
		{
			table.SpawnAndPlayOneShot(base.transform.position);
			return;
		}
		AudioClip audioClip = table.SelectClip();
		if (!(audioClip == null))
		{
			voiceSource.pitch = table.SelectPitch();
			float volumeScale = table.SelectVolume();
			voiceSource.PlayOneShot(audioClip, volumeScale);
			table.ReportPlayed(audioClip, null);
		}
	}

	private void PlayAppearAudio()
	{
		appearAudio.SpawnAndPlayOneShot(base.transform.position);
		if (hasAppearVoice)
		{
			PlayVoiceTableUnsafe(appearVoice);
		}
	}

	private void PlayHideAudio()
	{
		disappearAudio.SpawnAndPlayOneShot(base.transform.position);
		if (hasHideVoice)
		{
			PlayVoiceTableUnsafe(hideVoice);
		}
	}

	private void ToggleSingLoop(bool sing)
	{
		if (isSinging == sing)
		{
			return;
		}
		isSinging = sing;
		if (hasVoiceSource && hasSingVoice)
		{
			if (sing)
			{
				voiceSource.pitch = singVoice.SelectPitch();
				voiceSource.volume = singVoice.SelectVolume();
				voiceSource.clip = singVoice.SelectClip();
				voiceSource.Play();
			}
			else
			{
				voiceSource.Stop();
				voiceSource.volume = 1f;
			}
		}
	}

	public Vector3 GetOrigin()
	{
		if (hasOrigin)
		{
			return origin;
		}
		return base.transform.position;
	}

	public void SetOrigin(Vector3 localPosition)
	{
		hasOrigin = true;
		origin = base.transform.TransformPoint(localPosition);
	}

	public void MarkChild()
	{
		isChildAttacker = true;
		if (HasAppearTrigger)
		{
			trigger.InsideStateChanged -= OnInsideStateChanged;
			trigger.gameObject.SetActive(value: false);
			HasAppearTrigger = false;
		}
		trigger = null;
		if ((bool)explosionTrigger)
		{
			explosionTrigger.OnTriggerEntered -= OnExplosionTriggerEntered;
			explosionTrigger.gameObject.SetActive(value: false);
			explosionTrigger = null;
		}
		if ((bool)customDamageTrigger)
		{
			customDamageTrigger.OnTriggerEntered -= OnCustomDamageTriggerEntered;
			customDamageTrigger.gameObject.SetActive(value: false);
			customDamageTrigger = null;
		}
		if (hasCollider)
		{
			collider.enabled = false;
			hasCollider = false;
		}
	}

	public void CleanChild()
	{
		if (!Application.isPlaying)
		{
			if ((bool)trigger)
			{
				DestroyGameObject(trigger.gameObject);
			}
			trigger = null;
			if ((bool)explosionTrigger)
			{
				DestroyGameObject(explosionTrigger.gameObject);
			}
			explosionTrigger = null;
			if ((bool)customDamageTrigger)
			{
				DestroyGameObject(customDamageTrigger.gameObject);
			}
			customDamageTrigger = null;
			if ((bool)damager)
			{
				DestroyGameObject(damager.gameObject);
			}
			damager = null;
		}
		void DestroyGameObject(GameObject target)
		{
			_ = target != base.gameObject;
		}
	}
}
