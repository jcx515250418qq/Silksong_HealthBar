using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Corpse : MonoBehaviour, IInitialisable, BlackThreadState.IBlackThreadStateReceiver
{
	private enum States
	{
		NotStarted = 0,
		InAir = 1,
		DeathAnimation = 2,
		Complete = 3,
		PendingLandEffects = 4
	}

	[SerializeField]
	[FormerlySerializedAs("landEffects")]
	protected GameObject activateOnLand;

	[SerializeField]
	protected RandomAudioClipTable splatAudioClipTable;

	[SerializeField]
	private int smashBounces;

	[SerializeField]
	private bool breaker;

	[SerializeField]
	private bool bigBreaker;

	[SerializeField]
	private bool massless;

	[SerializeField]
	private bool resetRotation;

	[SerializeField]
	private Vector2 rotateAroundLocal;

	[SerializeField]
	protected AudioSource audioPlayerPrefab;

	[SerializeField]
	[HideInInspector]
	private AudioEvent startAudio;

	[SerializeField]
	private AudioEventRandom startAudios;

	[SerializeField]
	private PlayMakerFSM landEventTarget;

	[SerializeField]
	private bool doLandOnSplash;

	[SerializeField]
	protected float splashLandDelay;

	protected bool hitAcid;

	private bool isRecyclable;

	protected bool splashed;

	protected Color? bloodColorOverride;

	private Action<Transform> onCorpseBegin;

	private States state;

	private bool bouncedThisFrame;

	private int bounceCount;

	private float landEffectsDelayRemaining;

	private float landTimer;

	private Coroutine dropRoutine;

	private bool hasStarted;

	private Color initialSpriteColor;

	private bool hasPlayedStartAudio;

	protected MeshRenderer meshRenderer;

	protected tk2dSprite sprite;

	protected tk2dSpriteAnimator spriteAnimator;

	protected SpriteFlash spriteFlash;

	protected Rigidbody2D body;

	protected Collider2D bodyCollider;

	private CorpseItems corpseItems;

	private CorpseLandTint landTint;

	private static Dictionary<GameObject, Corpse> corpses = new Dictionary<GameObject, Corpse>();

	private bool hasAwaken;

	private bool startCalled;

	private bool destroyed;

	private bool isBlackThreaded;

	protected virtual bool DoLandEffectsInstantly => false;

	protected virtual bool DesaturateOnLand => false;

	GameObject IInitialisable.gameObject => base.gameObject;

	public static bool TryGetCorpse(GameObject gameObject, out Corpse corpse)
	{
		return corpses.TryGetValue(gameObject, out corpse);
	}

	private void OnDrawGizmosSelected()
	{
		if (resetRotation)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireSphere(rotateAroundLocal, 0.2f);
		}
	}

	public virtual bool OnAwake()
	{
		if (destroyed)
		{
			return false;
		}
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		OnValidate();
		meshRenderer = GetComponent<MeshRenderer>();
		sprite = GetComponent<tk2dSprite>();
		spriteAnimator = GetComponent<tk2dSpriteAnimator>();
		spriteFlash = GetComponent<SpriteFlash>();
		body = GetComponent<Rigidbody2D>();
		bodyCollider = GetComponent<Collider2D>();
		corpseItems = base.gameObject.GetComponent<CorpseItems>();
		landTint = base.gameObject.AddComponent<CorpseLandTint>();
		return true;
	}

	public virtual bool OnStart()
	{
		if (destroyed)
		{
			return false;
		}
		OnAwake();
		if (startCalled)
		{
			return false;
		}
		startCalled = true;
		return true;
	}

	protected virtual void Awake()
	{
		OnAwake();
		corpses[base.gameObject] = this;
	}

	private void OnDestroy()
	{
		corpses.Remove(base.gameObject);
		destroyed = true;
	}

	private void OnValidate()
	{
		if ((bool)startAudio.Clip)
		{
			startAudios = new AudioEventRandom
			{
				Clips = new AudioClip[1] { startAudio.Clip },
				PitchMin = startAudio.PitchMin,
				PitchMax = startAudio.PitchMax
			};
			startAudio.Clip = null;
		}
	}

	public void Setup(Color? bloodColorOverride, Action<Transform> onCorpseBegin, bool isCorpseRecyclable)
	{
		this.bloodColorOverride = bloodColorOverride;
		this.onCorpseBegin = onCorpseBegin;
		isRecyclable = isCorpseRecyclable;
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			bounceCount = 0;
			if ((bool)sprite)
			{
				sprite.color = initialSpriteColor;
			}
			Begin();
		}
		else if ((bool)sprite)
		{
			initialSpriteColor = sprite.color;
		}
		landTimer = 0f;
	}

	protected virtual void OnDisable()
	{
		splashed = false;
		hitAcid = false;
	}

	protected virtual void Start()
	{
		if (!hasStarted)
		{
			OnStart();
			Begin();
			hasStarted = true;
		}
	}

	protected void PlayStartAudio()
	{
		if (!hasPlayedStartAudio)
		{
			hasPlayedStartAudio = true;
			startAudios.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		}
	}

	protected virtual void Begin()
	{
		Transform transform = base.transform;
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = true;
		}
		if (onCorpseBegin != null)
		{
			onCorpseBegin(transform);
			onCorpseBegin = null;
		}
		PlayStartAudio();
		if (resetRotation)
		{
			Vector3 point = transform.TransformPoint(rotateAroundLocal);
			transform.RotateAround(point, Vector3.forward, 0f - transform.eulerAngles.z);
			transform.SetScaleY(Mathf.Abs(transform.localScale.y));
		}
		if (spriteFlash != null)
		{
			spriteFlash.flashWhiteQuick();
		}
		if (massless)
		{
			state = States.DeathAnimation;
		}
		else
		{
			state = States.InAir;
			if (spriteAnimator != null)
			{
				spriteAnimator.TryPlay("Death Air");
			}
		}
		if (DoLandEffectsInstantly)
		{
			Land();
		}
		StartCoroutine(DisableFlame());
	}

	protected void Update()
	{
		if (state == States.DeathAnimation)
		{
			if (spriteAnimator == null || !spriteAnimator.Playing)
			{
				Complete(detachChildren: true, destroyMe: true);
			}
		}
		else if (state == States.InAir)
		{
			bouncedThisFrame = false;
			if (base.transform.position.y < -10f)
			{
				Complete(detachChildren: true, destroyMe: true);
			}
			float y = body.linearVelocity.y;
			if (y < 0.1f && y > -0.1f)
			{
				landTimer += Time.deltaTime;
				if (landTimer > 0.1f)
				{
					Land();
				}
			}
		}
		else if (state == States.PendingLandEffects)
		{
			landEffectsDelayRemaining -= Time.deltaTime;
			if (landEffectsDelayRemaining <= 0f)
			{
				Complete(detachChildren: false, destroyMe: false);
			}
		}
	}

	private void Complete(bool detachChildren, bool destroyMe)
	{
		state = States.Complete;
		base.enabled = false;
		if (detachChildren)
		{
			base.transform.DetachChildren();
		}
		if (destroyMe)
		{
			if (isRecyclable)
			{
				base.gameObject.Recycle();
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnCollisionEnter2D()
	{
		OnCollision();
	}

	private void OnCollisionStay2D()
	{
		OnCollision();
	}

	private void OnCollision()
	{
		if (state == States.InAir && new Sweep(bodyCollider, 3, 3).Check(0.1f, 256, out var _))
		{
			Land();
		}
	}

	private void Land()
	{
		if (breaker)
		{
			if (bouncedThisFrame)
			{
				return;
			}
			bounceCount++;
			bouncedThisFrame = true;
			if (bounceCount >= smashBounces)
			{
				Smash();
			}
		}
		else
		{
			if (spriteAnimator != null && (!hitAcid || splashed))
			{
				spriteAnimator.TryPlay("Death Land");
			}
			landEffectsDelayRemaining = 1f;
			if (activateOnLand != null)
			{
				activateOnLand.SetActive(value: true);
			}
			state = States.PendingLandEffects;
			if (!hitAcid || splashed)
			{
				LandEffects();
			}
		}
		if ((bool)landEventTarget)
		{
			landEventTarget.SendEvent("LAND");
		}
		if ((bool)corpseItems)
		{
			corpseItems.ActivatePickup();
		}
		splashed = false;
	}

	protected virtual void LandEffects()
	{
		landTint.Landed(DesaturateOnLand);
	}

	protected virtual void Smash()
	{
		if (!hitAcid)
		{
			BloodSpawner.SpawnBlood(base.transform.position, 6, 8, 10f, 20f, 75f, 105f, null);
		}
		splatAudioClipTable.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		if (spriteAnimator != null)
		{
			spriteAnimator.Play("Death Land");
		}
		body.linearVelocity = Vector2.zero;
		state = States.DeathAnimation;
		if (bigBreaker)
		{
			if (!hitAcid)
			{
				BloodSpawner.SpawnBlood(base.transform.position, 30, 30, 20f, 30f, 80f, 100f, null);
			}
			GameCameras instance = GameCameras.instance;
			if ((bool)instance)
			{
				instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			}
		}
	}

	public void Acid()
	{
		hitAcid = true;
		Splashed();
		Land();
	}

	public void Splashed()
	{
		if (doLandOnSplash)
		{
			splashed = true;
		}
	}

	public void DropThroughFloor(bool waitToDrop)
	{
		if (dropRoutine != null)
		{
			StopCoroutine(dropRoutine);
		}
		dropRoutine = StartCoroutine(DropThroughFloorRoutine(waitToDrop));
	}

	private IEnumerator DropThroughFloorRoutine(bool waitToDrop)
	{
		if (waitToDrop)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 6f));
		}
		Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		if ((bool)body)
		{
			body.isKinematic = false;
		}
		yield return new WaitForSeconds(1f);
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator DisableFlame()
	{
		yield return new WaitForSeconds(5f);
	}

	public float GetBlackThreadAmount()
	{
		return isBlackThreaded ? 1 : 0;
	}

	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			isBlackThreaded = true;
			BlackThreadEffectRendererGroup component = GetComponent<BlackThreadEffectRendererGroup>();
			if (component != null)
			{
				component.SetBlackThreadAmount(1f);
			}
		}
		else
		{
			isBlackThreaded = false;
			BlackThreadEffectRendererGroup component2 = GetComponent<BlackThreadEffectRendererGroup>();
			if (component2 != null)
			{
				component2.OnRecycled();
			}
		}
	}
}
