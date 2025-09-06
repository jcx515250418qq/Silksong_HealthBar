using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LifebloodPustule : MonoBehaviour, IHitResponder
{
	[SerializeField]
	private PersistentBoolItem persistentBroken;

	[Space]
	[SerializeField]
	private int hitsToBreak;

	[SerializeField]
	private GameObject pustule;

	[SerializeField]
	private SpriteFlash spriteFlash;

	[SerializeField]
	private float regenerateDelay;

	[SerializeField]
	private RandomAudioClipTable regenAudioClipTable;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float extractedBreakAnticTime;

	[SerializeField]
	private CameraShakeTarget extractedBreakAnticRumble;

	[SerializeField]
	private CameraShakeTarget extractedBreakShake;

	[SerializeField]
	private RandomAudioClipTable extractedBreakAudioClipTable;

	[SerializeField]
	private GameObject extractedBreakDisable;

	[SerializeField]
	private GameObject extractedBreakEnable;

	[SerializeField]
	private GameObject extractedBreakEffects;

	[SerializeField]
	private GameObject witherParent;

	[Space]
	[SerializeField]
	private GameObject[] hitEffectPrefabs;

	[SerializeField]
	private BloodSpawner.Config hitBlood;

	[SerializeField]
	private RandomAudioClipTable hitAudioClipTable;

	[SerializeField]
	private BloodSpawner.Config breakBlood;

	[SerializeField]
	private RandomAudioClipTable breakAudioClipTable;

	[SerializeField]
	private Color bloodColor;

	[SerializeField]
	private CameraShakeTarget breakCameraShake;

	[SerializeField]
	private Vector2 heroPosOffset;

	[Space]
	[SerializeField]
	private UnityEvent onHit;

	[SerializeField]
	private UnityEvent onBreak;

	[SerializeField]
	private UnityEvent onRegenerateStart;

	private bool isBroken;

	private bool isExtracting;

	private bool isExtracted;

	private int hitCount;

	private NonBouncer nonBouncer;

	private LifebloodPustuleSubWither[] witherSubs;

	private List<JitterSelf> jitters;

	private static readonly int _regenerateAnimId = Animator.StringToHash("Regenerate");

	private Collider2D collider2D;

	private Coroutine extractPositionRoutine;

	private Transform EffectSpawnTransform
	{
		get
		{
			if (!pustule)
			{
				return base.transform;
			}
			return pustule.transform;
		}
	}

	private void Awake()
	{
		nonBouncer = GetComponent<NonBouncer>();
		jitters = new List<JitterSelf>();
		List<JitterSelf> list = new List<JitterSelf>();
		if ((bool)witherParent)
		{
			witherSubs = witherParent.GetComponentsInChildren<LifebloodPustuleSubWither>();
			witherParent.GetComponentsInChildren(list);
			jitters.AddRange(list);
		}
		if ((bool)extractedBreakDisable)
		{
			extractedBreakDisable.GetComponentsInChildren(list);
			jitters.AddRange(list);
		}
		if ((bool)persistentBroken)
		{
			persistentBroken.OnGetSaveState += delegate(out bool value)
			{
				value = isExtracted;
			};
			persistentBroken.OnSetSaveState += delegate(bool value)
			{
				isExtracted = value;
				UpdateInitialState();
			};
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "EXTRACT FINISH").ReceivedEvent += delegate
		{
			if (isExtracting)
			{
				isExtracting = false;
				isExtracted = true;
				Break(EffectSpawnTransform, isExtracted: true);
			}
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "EXTRACT CANCEL").ReceivedEvent += delegate
		{
			if (isExtracting)
			{
				isExtracting = false;
			}
		};
		UpdateInitialState();
		if ((bool)extractedBreakEffects)
		{
			extractedBreakEffects.SetActive(value: false);
		}
		collider2D = GetComponent<Collider2D>();
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (isExtracted || isBroken)
		{
			return IHitResponder.Response.None;
		}
		Transform effectSpawnTransform = EffectSpawnTransform;
		Vector3 position = effectSpawnTransform.position;
		bool flag = damageInstance.AttackType == AttackTypes.ExtractMoss;
		if (flag)
		{
			isExtracting = true;
		}
		if (isExtracting)
		{
			EventRegister.SendEvent(EventRegisterEvents.StartExtractB);
			HeroController instance = HeroController.instance;
			Vector2 position2 = instance.Body.position;
			float num = base.transform.position.y + heroPosOffset.y;
			bool flag2 = false;
			if (collider2D != null)
			{
				float x = collider2D.bounds.center.x;
				if (instance.cState.facingRight)
				{
					position2.x = x - heroPosOffset.x;
				}
				else
				{
					position2.x = x + heroPosOffset.x;
				}
				flag2 = true;
			}
			if (flag2 || num > position2.y)
			{
				Vector2 position3 = new Vector2(position2.x, num);
				instance.Body.MovePosition(position3);
				instance.Body.linearVelocity = Vector2.zero;
				EnsureExtractPosition(instance, position3);
			}
		}
		hitAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		hitCount++;
		if (hitCount < hitsToBreak || isExtracting)
		{
			if ((bool)spriteFlash)
			{
				spriteFlash.flashFocusGet();
			}
			BloodSpawner.SpawnBlood(hitBlood, effectSpawnTransform, bloodColor);
			onHit.Invoke();
		}
		else
		{
			Break(effectSpawnTransform, flag);
		}
		GameObject[] array = hitEffectPrefabs;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.Spawn(position);
			}
		}
		return IHitResponder.Response.GenericHit;
	}

	private void Break(Transform effectSpawnTransform, bool isExtracted)
	{
		if (!isExtracted)
		{
			isExtracting = false;
			EventRegister.SendEvent(EventRegisterEvents.ExtractCancel);
		}
		if ((bool)pustule)
		{
			pustule.SetActive(value: false);
		}
		BloodSpawner.SpawnBlood(breakBlood, effectSpawnTransform, bloodColor);
		isBroken = true;
		if ((bool)nonBouncer)
		{
			nonBouncer.active = true;
		}
		breakCameraShake.DoShake(this);
		onBreak.Invoke();
		breakAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		StartCoroutine(isExtracted ? ExtractedBreak() : Regenerate());
	}

	private void UpdateInitialState()
	{
		if ((bool)pustule)
		{
			pustule.SetActive(!isExtracted);
		}
		if ((bool)nonBouncer)
		{
			nonBouncer.active = isExtracted;
		}
		if ((bool)extractedBreakDisable)
		{
			extractedBreakDisable.SetActive(!isExtracted);
		}
		if ((bool)extractedBreakEnable)
		{
			extractedBreakEnable.SetActive(isExtracted);
		}
		if (isExtracted && witherSubs != null)
		{
			LifebloodPustuleSubWither[] array = witherSubs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StartWithered();
			}
		}
	}

	private IEnumerator Regenerate()
	{
		yield return new WaitForSeconds(regenerateDelay);
		if ((bool)pustule)
		{
			pustule.SetActive(value: true);
		}
		if ((bool)nonBouncer)
		{
			nonBouncer.active = false;
		}
		onRegenerateStart.Invoke();
		regenAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		animator.Play(_regenerateAnimId, 0, 0f);
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		yield return null;
		animator.cullingMode = AnimatorCullingMode.CullCompletely;
		isBroken = false;
		hitCount = 0;
	}

	private IEnumerator ExtractedBreak()
	{
		extractedBreakAnticRumble.DoShake(this);
		foreach (JitterSelf jitter in jitters)
		{
			jitter.StartJitter();
		}
		yield return new WaitForSeconds(extractedBreakAnticTime);
		foreach (JitterSelf jitter2 in jitters)
		{
			jitter2.StopJitterWithDecay();
		}
		extractedBreakAnticRumble.CancelShake();
		extractedBreakShake.DoShake(this);
		extractedBreakAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		if ((bool)extractedBreakDisable)
		{
			extractedBreakDisable.SetActive(value: false);
		}
		if ((bool)extractedBreakEnable)
		{
			extractedBreakEnable.SetActive(value: true);
		}
		if (witherSubs != null)
		{
			LifebloodPustuleSubWither[] array = witherSubs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].BeginWither(base.transform);
			}
		}
		if ((bool)extractedBreakEffects)
		{
			extractedBreakEffects.SetActive(value: true);
		}
	}

	private void EnsureExtractPosition(HeroController hc, Vector2 position)
	{
		if (extractPositionRoutine == null)
		{
			extractPositionRoutine = StartCoroutine(ExtractPositionRoutine(hc, position));
		}
	}

	private IEnumerator ExtractPositionRoutine(HeroController hc, Vector2 position)
	{
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		while (isExtracting)
		{
			hc.Body.MovePosition(position);
			yield return wait;
			if (Mathf.Abs(hc.transform.position.x - position.x) < float.Epsilon)
			{
				break;
			}
		}
		hc.Body.linearVelocity = Vector2.zero;
		extractPositionRoutine = null;
	}
}
