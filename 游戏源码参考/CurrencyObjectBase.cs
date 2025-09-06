using System.Collections;
using System.Collections.Generic;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

public abstract class CurrencyObjectBase : MonoBehaviour, ICurrencyObject, IBurnable, IHitResponder, AntRegion.INotify, IBreakerBreakable
{
	[SerializeField]
	private AudioClip[] landSounds;

	[SerializeField]
	private AudioClip[] pickupSounds;

	[SerializeField]
	private VibrationData pickupVibration;

	[SerializeField]
	private VibrationDataAsset pickupVibrationAsset;

	[SerializeField]
	private ParticleSystem acidEffect;

	[SerializeField]
	private TriggerEnterEvent pickupRange;

	[SerializeField]
	private ToolItem magnetTool;

	[SerializeField]
	private ToolItem magnetBuffTool;

	[SerializeField]
	private float magnetBuffDelayMultiplier;

	[SerializeField]
	private AlertRange magnetToolRange;

	[SerializeField]
	private GameObject magnetEffect;

	[SerializeField]
	private GameObject breakEffect;

	[SerializeField]
	private GameObject pickupEffect;

	[SerializeField]
	private MinMaxFloat magnetStartDelay = new MinMaxFloat(1f, 1.7f);

	[SerializeField]
	private MinMaxFloat magnetStartHeight;

	[SerializeField]
	private float magnetStartMoveDuration;

	[SerializeField]
	private AnimationCurve magnetStartMoveCurve;

	[SerializeField]
	private MinMaxFloat magnetAttractDelay = new MinMaxFloat(0.3f, 0.5f);

	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	protected string firstGetPDBool;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	protected string popupPDBool;

	[SerializeField]
	[LocalisedString.NotRequired]
	protected LocalisedString popupName;

	[SerializeField]
	protected Sprite popupSprite;

	private Coroutine getterRoutine;

	private bool isAttracted;

	private bool isMoving;

	private bool activated;

	private float defaultGravityScale;

	private Vector2 offsetPos;

	private Vector2 attractVelocity;

	private AntRegion insideAntRegion;

	private const float PICKUP_START_DELAY = 0.35f;

	private double pickupStartTime;

	private Transform hero;

	private AudioSource audioSource;

	private Renderer rend;

	private Rigidbody2D body;

	private Collider2D collider;

	private readonly HashSet<Collider2D> landedColliders = new HashSet<Collider2D>();

	private bool hasRenderer;

	private bool hasAcidEffect;

	private bool hasMagnetEffect;

	private bool hasMagnetTool;

	private bool hasMagnetBuff;

	private bool isDisabling;

	private float waitTime;

	private VisibilityGroup visibilityGroup;

	private bool isOnGround;

	private bool hasStarted;

	private static readonly UniqueList<CurrencyObjectBase> _currencyObjects = new UniqueList<CurrencyObjectBase>();

	private static int _lastUpdate = -1;

	private static bool _isHeroDead;

	private bool isBroken;

	private static int lastPickupUpdate = -1;

	private static bool canPickupExist;

	private ToolItemManager.ToolStatus magnetToolStatus;

	private ToolItemManager.ToolStatus magnetBuffStatus;

	private static int lastLandFrame;

	private static int playedCount;

	private const int LAND_SOUND_LIMIT = 5;

	protected bool hasCheckedPopup;

	protected abstract CurrencyType? CurrencyType { get; }

	private static bool IsHeroDead
	{
		get
		{
			int frameCount = Time.frameCount;
			if (_lastUpdate != frameCount)
			{
				_lastUpdate = frameCount;
				HeroController instance = HeroController.instance;
				_isHeroDead = !instance || instance.cState.dead;
			}
			return _isHeroDead;
		}
	}

	private static bool CanPickupExist
	{
		get
		{
			if (lastPickupUpdate != Time.frameCount)
			{
				lastPickupUpdate = Time.frameCount;
				canPickupExist = GameManager.instance.CanPickupsExist();
			}
			return canPickupExist;
		}
	}

	public BreakableBreaker.BreakableTypes BreakableType => BreakableBreaker.BreakableTypes.Basic;

	GameObject IBreakerBreakable.gameObject => base.gameObject;

	public static void ProcessHeroDeath()
	{
		_currencyObjects.ReserveListUsage();
		foreach (CurrencyObjectBase item in _currencyObjects.List)
		{
			item.OnHeroDeath();
		}
		_currencyObjects.ReleaseListUsage();
	}

	protected virtual void Awake()
	{
		if ((bool)pickupRange)
		{
			pickupRange.OnTriggerEntered += HandleHeroEnter;
		}
		audioSource = GetComponent<AudioSource>();
		rend = GetComponent<Renderer>();
		body = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
		hasRenderer = rend;
		hasAcidEffect = acidEffect;
		hasMagnetEffect = magnetEffect;
		hasMagnetTool = magnetTool;
		hasMagnetBuff = magnetBuffTool;
		if (hasMagnetTool)
		{
			magnetToolStatus = magnetTool.Status;
		}
		if (hasMagnetBuff)
		{
			magnetBuffStatus = magnetBuffTool.Status;
		}
		defaultGravityScale = body.gravityScale;
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out bool value)
			{
				value = activated;
			};
			persistent.OnSetSaveState += delegate(bool value)
			{
				activated = value;
				if (activated)
				{
					base.gameObject.SetActive(value: false);
				}
			};
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += OnToolEquipsUpdated;
		if ((bool)pickupVibrationAsset)
		{
			pickupVibration = pickupVibrationAsset.VibrationData;
		}
		visibilityGroup = base.gameObject.AddComponent<VisibilityGroup>();
	}

	protected virtual void OnDestroy()
	{
		if ((bool)pickupRange)
		{
			pickupRange.OnTriggerEntered -= HandleHeroEnter;
		}
	}

	protected virtual void OnEnable()
	{
		activated = false;
		body.bodyType = RigidbodyType2D.Dynamic;
		body.gravityScale = defaultGravityScale;
		collider.enabled = true;
		collider.isTrigger = false;
		isAttracted = false;
		attractVelocity = Vector2.zero;
		isMoving = false;
		if (isBroken)
		{
			if ((bool)breakEffect)
			{
				breakEffect.SetActive(value: false);
			}
			isBroken = false;
		}
		SetVisible(visible: true);
		if (hasAcidEffect)
		{
			acidEffect.gameObject.SetActive(value: false);
		}
		if (hasMagnetEffect)
		{
			magnetEffect.SetActive(value: false);
		}
		pickupStartTime = Time.timeAsDouble + 0.3499999940395355;
		OnToolEquipsUpdated();
		_currencyObjects.Add(this);
		if (hasStarted)
		{
			OnStartOrEnable();
			ComponentSingleton<CurrencyObjectBaseCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
		}
	}

	protected virtual void Start()
	{
		OnStartOrEnable();
		hasStarted = true;
		ComponentSingleton<CurrencyObjectBaseCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
	}

	protected virtual void OnStartOrEnable()
	{
		if (!CanPickupExist)
		{
			base.gameObject.Recycle();
		}
	}

	protected virtual void OnDisable()
	{
		ComponentSingleton<CurrencyObjectBaseCallbackHooks>.Instance.OnFixedUpdate -= OnFixedUpdate;
		if (isBroken)
		{
			if ((bool)breakEffect)
			{
				breakEffect.SetActive(value: false);
			}
			isBroken = false;
		}
		if (getterRoutine != null)
		{
			StopCoroutine(getterRoutine);
			getterRoutine = null;
		}
		body.bodyType = RigidbodyType2D.Dynamic;
		_currencyObjects.Remove(this);
		isDisabling = false;
		landedColliders.Clear();
	}

	private void OnHeroDeath()
	{
		activated = true;
		collider.enabled = true;
		collider.isTrigger = false;
		isAttracted = false;
		isMoving = false;
		body.bodyType = RigidbodyType2D.Dynamic;
		body.gravityScale = defaultGravityScale;
		Vector2 linearVelocity = body.linearVelocity;
		linearVelocity.x = 0f;
		if (linearVelocity.y > 0f)
		{
			linearVelocity.y = 0f;
		}
		body.linearVelocity = linearVelocity;
		if (hasMagnetEffect)
		{
			magnetEffect.SetActive(value: false);
		}
		if (getterRoutine != null)
		{
			StopCoroutine(getterRoutine);
			getterRoutine = null;
		}
	}

	private void OnToolEquipsUpdated()
	{
		if (!base.isActiveAndEnabled || activated || isDisabling)
		{
			return;
		}
		if (MagnetToolIsEquipped())
		{
			if (getterRoutine == null)
			{
				getterRoutine = StartCoroutine(Getter());
			}
		}
		else if (getterRoutine != null)
		{
			StopCoroutine(getterRoutine);
			getterRoutine = null;
		}
	}

	private bool MagnetToolIsEquipped()
	{
		if (hasMagnetTool)
		{
			return magnetToolStatus.IsEquipped;
		}
		return false;
	}

	private bool MagnetBuffEquipped()
	{
		if (hasMagnetBuff)
		{
			return magnetBuffStatus.IsEquipped;
		}
		return false;
	}

	private void OnFixedUpdate()
	{
		if (activated)
		{
			return;
		}
		if (isAttracted)
		{
			if (!hero)
			{
				hero = HeroController.instance.transform;
			}
			Vector3 position = hero.transform.position;
			Vector3 position2 = base.transform.position;
			Vector2 vector = new Vector2(position.x - position2.x, position.y - 0.5f - position2.y);
			vector = Vector2.ClampMagnitude(vector, 1f);
			vector *= 200f;
			attractVelocity += vector * Time.fixedDeltaTime;
			attractVelocity = Vector2.ClampMagnitude(attractVelocity, 20f);
		}
		else if (!isMoving)
		{
			return;
		}
		Vector2 vector2 = attractVelocity * Time.fixedDeltaTime;
		body.linearVelocity = Vector2.zero;
		body.position += offsetPos + vector2;
		offsetPos = Vector2.zero;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((bool)pickupRange)
		{
			return;
		}
		if (collision.CompareTag("Acid"))
		{
			float num = 0f;
			if (hasAcidEffect)
			{
				acidEffect.gameObject.SetActive(value: true);
				ParticleSystem.MainModule main = acidEffect.main;
				num = Mathf.Max(num, main.duration + main.startLifetime.constant);
			}
			Disable(num);
		}
		else
		{
			HandleHeroEnter(collision, base.gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (landedColliders.Add(other.collider))
		{
			Land();
		}
	}

	protected virtual void Land()
	{
		int frameCount = Time.frameCount;
		if (lastLandFrame != frameCount)
		{
			lastLandFrame = frameCount;
			playedCount = 0;
		}
		if (playedCount < 5)
		{
			playedCount++;
			PlaySound(landSounds);
		}
		isOnGround = true;
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		if (isOnGround && landedColliders.Remove(other.collider) && landedColliders.Count == 0)
		{
			LeftGround();
		}
	}

	protected virtual void LeftGround()
	{
		isOnGround = false;
	}

	protected virtual void HandleHeroEnter(Collider2D collision, GameObject sender)
	{
		if (collision.CompareTag("HeroBox"))
		{
			DoCollect();
		}
	}

	public void DoCollect()
	{
		if (!activated && !(Time.timeAsDouble < pickupStartTime) && !IsHeroDead)
		{
			if ((bool)pickupEffect)
			{
				Vector3 position = base.transform.position;
				position.z = 0.001f;
				pickupEffect.Spawn(position);
			}
			VibrationManager.PlayVibrationClipOneShot(pickupVibration, null);
			float recycleTime = PlaySound(pickupSounds);
			if (Collect())
			{
				Disable(recycleTime);
			}
		}
	}

	private float PlaySound(IReadOnlyList<AudioClip> sounds)
	{
		if (!audioSource || sounds.Count <= 0)
		{
			return 0f;
		}
		AudioClip audioClip = sounds[Random.Range(0, sounds.Count)];
		if ((bool)audioClip)
		{
			audioSource.PlayOneShot(audioClip);
			return audioClip.length;
		}
		return 0f;
	}

	private void SetVisible(bool visible)
	{
		if (hasRenderer)
		{
			rend.enabled = visible;
		}
		SetRendererActive(visible);
	}

	protected virtual void SetRendererActive(bool active)
	{
	}

	public void Disable(float recycleTime)
	{
		activated = true;
		SetVisible(visible: false);
		if (hasMagnetEffect)
		{
			magnetEffect.SetActive(value: false);
		}
		if ((bool)persistent)
		{
			persistent.SaveState();
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (isDisabling)
		{
			waitTime = recycleTime;
			return;
		}
		collider.enabled = false;
		body.bodyType = RigidbodyType2D.Kinematic;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		isDisabling = true;
		if (getterRoutine != null)
		{
			StopCoroutine(getterRoutine);
			getterRoutine = null;
		}
		StartCoroutine(DisableAfterTime(recycleTime));
	}

	private IEnumerator DisableAfterTime(float recycleTime)
	{
		waitTime = 0f;
		while (recycleTime > 0f)
		{
			yield return null;
			if (waitTime > recycleTime)
			{
				recycleTime = waitTime;
				waitTime = 0f;
			}
			recycleTime -= Time.deltaTime;
		}
		if ((bool)breakEffect)
		{
			breakEffect.SetActive(value: false);
			isBroken = false;
		}
		isDisabling = false;
		base.gameObject.Recycle();
	}

	private IEnumerator Getter()
	{
		if ((bool)magnetToolRange)
		{
			while (!magnetToolRange.IsHeroInRange())
			{
				yield return null;
			}
		}
		float num = magnetStartDelay.GetRandomValue();
		if (MagnetBuffEquipped())
		{
			num *= magnetBuffDelayMultiplier;
		}
		yield return new WaitForSeconds(num);
		if (hasMagnetEffect)
		{
			magnetEffect.SetActive(value: true);
		}
		collider.isTrigger = true;
		body.gravityScale = 0f;
		body.linearVelocity = Vector2.zero;
		float magnetAttractDelayLeft = magnetAttractDelay.GetRandomValue();
		if (magnetStartMoveDuration > 0f)
		{
			isMoving = true;
			Vector2 startPos = body.position;
			Vector2 targetPos = startPos + new Vector2(0f, magnetStartHeight.GetRandomValue());
			Vector2 previousPos = startPos;
			float elapsed = 0f;
			while (elapsed <= magnetStartMoveDuration)
			{
				float time = elapsed / magnetStartMoveDuration;
				time = magnetStartMoveCurve.Evaluate(time);
				Vector2 vector = Vector2.LerpUnclamped(startPos, targetPos, time);
				offsetPos += vector - previousPos;
				previousPos = vector;
				yield return null;
				elapsed += Time.deltaTime;
				if (!isAttracted)
				{
					magnetAttractDelayLeft -= Time.deltaTime;
					if (magnetAttractDelayLeft <= 0f)
					{
						isAttracted = true;
					}
				}
			}
		}
		if (!isAttracted && magnetAttractDelayLeft > 0f)
		{
			yield return new WaitForSeconds(magnetAttractDelayLeft);
			isAttracted = true;
		}
	}

	private bool Collect()
	{
		bool num = Collected();
		if (num && !hasCheckedPopup)
		{
			hasCheckedPopup = true;
			CollectPopup();
		}
		return num;
	}

	public virtual void CollectPopup()
	{
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(firstGetPDBool) && !instance.GetVariable<bool>(firstGetPDBool))
		{
			instance.SetVariable(firstGetPDBool, value: true);
		}
		if (!string.IsNullOrEmpty(popupPDBool) && !instance.GetVariable<bool>(popupPDBool))
		{
			instance.SetVariable(popupPDBool, value: true);
			if (!popupName.IsEmpty)
			{
				UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
				uIMsgDisplay.Name = popupName;
				uIMsgDisplay.Icon = popupSprite;
				uIMsgDisplay.IconScale = 1f;
				CollectableUIMsg.Spawn(uIMsgDisplay);
			}
		}
	}

	protected abstract bool Collected();

	public virtual void BurnUp()
	{
		Disable(3f);
	}

	public void Break()
	{
		body.bodyType = RigidbodyType2D.Kinematic;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		if ((bool)breakEffect)
		{
			isBroken = true;
			breakEffect.SetActive(value: true);
		}
		Disable(1f);
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (activated || Time.timeAsDouble < pickupStartTime)
		{
			return IHitResponder.Response.None;
		}
		AttackTypes attackType = damageInstance.AttackType;
		if (attackType == AttackTypes.Acid || attackType == AttackTypes.Splatter || attackType == AttackTypes.RuinsWater || attackType == AttackTypes.Coal || attackType == AttackTypes.Fire || attackType == AttackTypes.Spikes)
		{
			return IHitResponder.Response.None;
		}
		if (BreakOnHazard.IsCogDamager(damageInstance.Source))
		{
			return IHitResponder.Response.None;
		}
		float magnitudeMultForType = damageInstance.GetMagnitudeMultForType(HitInstance.TargetType.Currency);
		Vector2 vector;
		if (damageInstance.CircleDirection)
		{
			vector = damageInstance.GetActualDirection(base.transform, HitInstance.TargetType.Currency).AngleToDirection();
			vector.x *= 5f;
			if (isOnGround)
			{
				vector.y = Random.Range(5f, 15f);
			}
			else
			{
				vector.y *= ((vector.y > 0f) ? 10 : 3);
			}
		}
		else
		{
			HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.Regular);
			Vector2 vector2 = new Vector2(Random.Range(5f, 1.5f), Random.Range(8f, 2f));
			if (Random.Range(0f, 1f) <= 0.25f)
			{
				hitDirection = hitDirection switch
				{
					HitInstance.HitDirection.Left => HitInstance.HitDirection.Right, 
					HitInstance.HitDirection.Right => HitInstance.HitDirection.Left, 
					_ => hitDirection, 
				};
			}
			vector = hitDirection switch
			{
				HitInstance.HitDirection.Left => new Vector2(0f - vector2.x, vector2.y), 
				HitInstance.HitDirection.Right => vector2, 
				_ => new Vector2(Random.Range(-3f, 3f), Random.Range(5f, 10f)), 
			};
		}
		if ((bool)insideAntRegion)
		{
			insideAntRegion.ResetTracker(base.gameObject);
		}
		body.AddForce(vector * magnitudeMultForType, ForceMode2D.Impulse);
		if ((body.constraints & RigidbodyConstraints2D.FreezeRotation) == 0)
		{
			float num = 3f;
			float torque = Random.Range(0f - num, num) * magnitudeMultForType;
			body.AddTorque(torque, ForceMode2D.Impulse);
		}
		return new IHitResponder.HitResponse(IHitResponder.Response.GenericHit, consumeCharges: false);
	}

	public static void SendOnCameraShakedWorldForce(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		foreach (CurrencyObjectBase item in _currencyObjects.List)
		{
			item.OnCameraShakedWorldForce(intensity);
		}
	}

	protected virtual bool CanReactCameraShake()
	{
		if (isOnGround)
		{
			return body.linearVelocity.y <= 0f;
		}
		return false;
	}

	private void OnCameraShakedWorldForce(CameraShakeWorldForceIntensities intensity)
	{
		if (intensity >= CameraShakeWorldForceIntensities.Intense && CanReactCameraShake() && visibilityGroup.IsVisible)
		{
			RandomJostle();
		}
	}

	private void RandomJostle()
	{
		Vector2 force = new Vector2(Random.Range(-3f, 3f), Random.Range(10f, 20f));
		body.AddForce(force, ForceMode2D.Impulse);
	}

	public void EnteredAntRegion(AntRegion antRegion)
	{
		insideAntRegion = antRegion;
	}

	public void ExitedAntRegion(AntRegion antRegion)
	{
		if (insideAntRegion == antRegion)
		{
			insideAntRegion = null;
		}
	}

	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		RandomJostle();
	}

	public void HitFromBreaker(BreakableBreaker breaker)
	{
		RandomJostle();
	}
}
