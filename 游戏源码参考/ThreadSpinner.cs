using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ThreadSpinner : MonoBehaviour, IHitResponder
{
	[Serializable]
	private struct ScaleAppearance
	{
		public float Scale;

		public string Animation;
	}

	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}

	private readonly int idleAnim = Animator.StringToHash("Idle");

	private readonly int spinAnim = Animator.StringToHash("Spin");

	private readonly int disperseAnim = Animator.StringToHash("Disperse");

	[SerializeField]
	private PersistentIntItem persistent;

	[SerializeField]
	private Animator threadSpool;

	[SerializeField]
	private GameObject threadPrefab;

	[SerializeField]
	private Transform threadSpawn;

	[SerializeField]
	private int amountPerHit;

	[SerializeField]
	private float addSilkDelay;

	[SerializeField]
	private int addSilkPerHit;

	[SerializeField]
	private ScaleAppearance[] hitStageScales;

	[SerializeField]
	private float spinDownTime;

	[SerializeField]
	private GameObject[] spawnEffectPrefabs;

	[SerializeField]
	private AudioEventRandom hitSound;

	[SerializeField]
	private AudioEventRandom dislodgeSound;

	[SerializeField]
	private AudioEventRandom flingSound;

	[Space]
	public UnityBoolEvent HitEventDirectional;

	public UnityEvent HitEvent;

	[Space]
	[SerializeField]
	private float lastHitRotation;

	[SerializeField]
	private AnimationCurve lastHitCurve;

	[SerializeField]
	private float lastHitDuration;

	[SerializeField]
	private GameObject[] inertHitEffectPrefabs;

	[SerializeField]
	private FlingUtils.ChildrenConfig flingConfig;

	[Space]
	public UnityEvent OnDislodge;

	public UnityEvent OnFling;

	private Coroutine animRoutine;

	private float startLerp;

	private float endLerp;

	private int hits;

	private float initialXScale;

	private void Awake()
	{
		if ((bool)threadSpool)
		{
			initialXScale = threadSpool.transform.localScale.x;
		}
		if (!persistent)
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out int value)
		{
			value = hits;
		};
		persistent.OnSetSaveState += delegate(int value)
		{
			hits = value;
			if ((bool)threadSpool)
			{
				if (hits <= hitStageScales.Length && hitStageScales.Length != 0)
				{
					if (hits > 0)
					{
						ScaleAppearance scaleAppearance = hitStageScales[hits - 1];
						threadSpool.transform.SetScaleX(scaleAppearance.Scale);
					}
					threadSpool.gameObject.SetActive(value: true);
				}
				else if (hits <= 0)
				{
					threadSpool.transform.SetScaleX(initialXScale);
					threadSpool.gameObject.SetActive(value: true);
				}
				else
				{
					threadSpool.gameObject.SetActive(value: false);
					hits = hitStageScales.Length + 1;
				}
			}
		};
		ResetDynamicHierarchy resetter = base.gameObject.AddComponent<ResetDynamicHierarchy>();
		persistent.SemiPersistentReset += delegate
		{
			hits = 0;
			resetter.DoReset(alsoRoot: true);
			if ((bool)threadSpool)
			{
				threadSpool.gameObject.SetActive(value: true);
				threadSpool.transform.SetScaleX(initialXScale);
				threadSpool.Play(idleAnim);
			}
		};
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!damageInstance.IsNailDamage)
		{
			return IHitResponder.Response.None;
		}
		bool flag = false;
		bool flag2 = false;
		if (hits > hitStageScales.Length)
		{
			if (hits == hitStageScales.Length + 1)
			{
				flag = true;
			}
			else
			{
				if (hits != hitStageScales.Length + 2)
				{
					return IHitResponder.Response.None;
				}
				flag2 = true;
			}
		}
		if (animRoutine != null)
		{
			StopCoroutine(animRoutine);
		}
		bool flag3 = false;
		bool flag4 = false;
		Vector3 position = (threadSpawn ? threadSpawn.position : base.transform.position);
		bool flag5 = damageInstance.GetHitDirection(HitInstance.TargetType.Regular) switch
		{
			HitInstance.HitDirection.Left => true, 
			HitInstance.HitDirection.Right => false, 
			_ => UnityEngine.Random.Range(0, 2) == 0, 
		};
		if (hits < hitStageScales.Length)
		{
			flag3 = true;
			flag4 = true;
			if ((bool)threadSpool)
			{
				ScaleAppearance scaleAppearance = hitStageScales[hits];
				Action onTimerEnd;
				if (!string.IsNullOrEmpty(scaleAppearance.Animation))
				{
					threadSpool.Play(scaleAppearance.Animation);
					onTimerEnd = null;
				}
				else
				{
					threadSpool.Play(spinAnim);
					onTimerEnd = delegate
					{
						threadSpool.Play(idleAnim);
					};
				}
				startLerp = threadSpool.transform.localScale.x;
				endLerp = scaleAppearance.Scale;
				animRoutine = this.StartTimerRoutine(0f, spinDownTime, delegate(float time)
				{
					threadSpool.transform.SetScaleX(Mathf.LerpUnclamped(startLerp, endLerp, time));
				}, null, onTimerEnd);
			}
		}
		else if (hits == hitStageScales.Length)
		{
			flag3 = true;
			flag4 = true;
			if ((bool)threadSpool)
			{
				threadSpool.transform.SetScaleX(1f);
				threadSpool.Play(disperseAnim);
			}
		}
		else if (flag)
		{
			flag3 = true;
			dislodgeSound.SpawnAndPlayOneShot(position);
			OnDislodge.Invoke();
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			startLerp = localEulerAngles.z;
			endLerp = localEulerAngles.z + (flag5 ? (0f - lastHitRotation) : lastHitRotation);
			animRoutine = this.StartTimerRoutine(0f, lastHitDuration, delegate(float time)
			{
				time = lastHitCurve.Evaluate(time);
				base.transform.SetLocalRotation2D(Mathf.LerpUnclamped(startLerp, endLerp, time));
			});
		}
		else if (flag2)
		{
			flag3 = true;
			flingSound.SpawnAndPlayOneShot(position);
			Collider2D component = GetComponent<Collider2D>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			FlingUtils.ChildrenConfig config = flingConfig;
			if (flag5)
			{
				config.AngleMin = Helper.GetReflectedAngle(config.AngleMin, reflectHorizontal: true, reflectVertical: false);
				config.AngleMax = Helper.GetReflectedAngle(config.AngleMax, reflectHorizontal: true, reflectVertical: false);
			}
			if ((bool)config.Parent)
			{
				config.Parent.transform.SetParent(null, worldPositionStays: true);
			}
			FlingUtils.FlingChildren(config, config.Parent ? config.Parent.transform : null, Vector3.zero, null);
			OnFling.Invoke();
			base.transform.SetPosition2D(-2000f, -2000f);
		}
		if (flag3)
		{
			hits++;
			if (flag4)
			{
				if ((bool)threadPrefab)
				{
					int num = amountPerHit;
					for (int i = 0; i < num; i++)
					{
						threadPrefab.Spawn(position);
					}
				}
				spawnEffectPrefabs.SpawnAll(position);
				hitSound.SpawnAndPlayOneShot(position);
				StartCoroutine(AddSilkDelayed());
				if (HitEvent != null)
				{
					HitEvent.Invoke();
				}
				if (HitEventDirectional != null)
				{
					HitEventDirectional.Invoke(flag5);
				}
			}
			else
			{
				inertHitEffectPrefabs.SpawnAll(position);
			}
		}
		return flag3 ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	private IEnumerator AddSilkDelayed()
	{
		yield return new WaitForSeconds(addSilkDelay);
		HeroController.instance.AddSilk(addSilkPerHit, heroEffect: true);
	}
}
