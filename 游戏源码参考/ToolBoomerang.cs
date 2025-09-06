using System;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ToolBoomerang : MonoBehaviour, IHitResponder, ITinkResponder
{
	private enum Direction
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3
	}

	private enum States
	{
		Translating = 0,
		Body = 1,
		Stuck = 2,
		Broken = 3
	}

	private struct DirectionPos
	{
		public Direction Direction;

		public Vector2 Position;

		public static DirectionPos GetDefault(Transform transform)
		{
			DirectionPos result = default(DirectionPos);
			result.Direction = Direction.Right;
			result.Position = transform.position;
			return result;
		}
	}

	[SerializeField]
	private float flySpinSpeed = 10f;

	[SerializeField]
	private MinMaxFloat startRotation;

	[SerializeField]
	private MinMaxFloat stickRotation;

	[Space]
	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("CheckAnim")]
	private string flyAnim;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("CheckAnim")]
	private string stickAnim;

	[Space]
	[SerializeField]
	private Vector2 flyOutPosition = new Vector2(10f, 0f);

	[SerializeField]
	private AnimationCurve flyOutCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve flyOutCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float flyOutCurveDuration = 1f;

	[SerializeField]
	private float stuckIdleDuration = 3f;

	[SerializeField]
	[Range(0f, 1f)]
	private float flyOutReturnPoint = 0.5f;

	[Space]
	[SerializeField]
	private DamageEnemies damager;

	[SerializeField]
	private CircleCollider2D damagerCollider;

	[SerializeField]
	private float hitDamageMult = 2f;

	[SerializeField]
	private AnimationCurve damageVelocityLerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float damageVelocityLerpTime = 0.05f;

	[Space]
	[SerializeField]
	private GameObject[] childStickEffect;

	[SerializeField]
	private GameObject[] childBreakEffects;

	[SerializeField]
	private float recycleDelay;

	[SerializeField]
	private MeshRenderer renderer;

	[Space]
	[SerializeField]
	private float knockbackSpeed = 30f;

	[SerializeField]
	private Vector2 horizontalKnockbackDirection = new Vector2(2f, 1f);

	[SerializeField]
	private float horizontalKnockbackFlattenSpeed = 10f;

	[SerializeField]
	private Vector2 downKnockbackDirection = new Vector2(1f, -1f);

	[SerializeField]
	private float weakKnockbackAmount = 1f;

	[SerializeField]
	private AudioEvent nailDeflectClip;

	[SerializeField]
	private GameObject nailHitEffectPrefab;

	[SerializeField]
	private GameObject wallHitPrefab;

	[Header("Poison")]
	[SerializeField]
	private ToolItem getTintFrom;

	public ParticleSystem ptPoisonIdle;

	public ParticleSystem ptBreak;

	private Vector3 initialScale;

	private Vector2 initialPosition;

	private Vector2 targetPosition;

	private Color poisonTint;

	private float elapsedTime;

	private float currentDamageVelocityLerpTime;

	private Vector2 bodyVelocity;

	private bool doFlattenVelocity;

	private bool wasNailHit;

	private int initialDamage;

	private float initialDamageMult;

	private Vector2 previousPosition;

	private bool doSetup;

	private double recycleTime;

	private float bounceCoolDown;

	private States currentState;

	private Collider2D stuckCollider;

	private Rigidbody2D body;

	private Collider2D collider;

	private AudioSource audioSource;

	private const int STUCK_FRAME_COUNT = 3;

	private const float STUCK_RADIUS = 0.15f;

	private const float STUCK_RADIUS_SQR = 0.0225f;

	private readonly Vector2[] positionHistory = new Vector2[3];

	private int positionIndex;

	private int positionCount;

	public ITinkResponder.TinkFlags ResponderType => ITinkResponder.TinkFlags.Projectile;

	[UsedImplicitly]
	private bool? CheckAnim(string animName)
	{
		if (!animator)
		{
			return null;
		}
		return animator.GetClipByName(animName) != null;
	}

	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
		}
		Vector2 from = (Application.isPlaying ? initialPosition : Vector2.zero);
		Vector2 to = (Application.isPlaying ? targetPosition : flyOutPosition);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(flyOutPosition, 0.2f);
		Gizmos.color = Color.yellow;
		for (float num = 0.05f; num <= 1f; num += 0.05f)
		{
			Gizmos.DrawWireSphere(SamplePosAtTime(from, to, num), 0.1f);
		}
		Gizmos.color = Color.red;
		for (float num2 = 1f; num2 <= 2f; num2 += 0.05f)
		{
			Gizmos.DrawWireSphere(SamplePosAtTime(from, to, num2), 0.1f);
		}
		Vector2 vector = SamplePosAtTime(from, to, flyOutReturnPoint);
		Gizmos.color = new Color(1f, 0.5f, 0f);
		Gizmos.DrawWireSphere(vector, 0.2f);
	}

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
		audioSource = GetComponent<AudioSource>();
		poisonTint = Gameplay.PoisonPouchTintColour;
		damager.WillDamageEnemyOptions += OnWillDamageEnemy;
		damager.DamagedEnemy += OnDamagedEnemy;
	}

	private void OnEnable()
	{
		doSetup = true;
		ResetStuckDetection();
		body.isKinematic = true;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		doFlattenVelocity = false;
		audioSource.Play();
		initialDamageMult = damager.magnitudeMult;
		initialDamage = damager.damageDealt;
		childStickEffect.SetAllActive(value: false);
		childBreakEffects.SetAllActive(value: false);
		if ((bool)renderer)
		{
			renderer.enabled = true;
		}
		currentState = States.Translating;
		stuckCollider = null;
		initialScale = base.transform.localScale;
		damagerCollider.enabled = true;
		CheckPoison();
	}

	private void Start()
	{
		CheckPoison();
	}

	private void CheckPoison()
	{
		tk2dSprite component = GetComponent<tk2dSprite>();
		if (Gameplay.PoisonPouchTool.IsEquipped)
		{
			if ((bool)getTintFrom)
			{
				component.EnableKeyword("CAN_HUESHIFT");
				component.SetFloat(PoisonTintBase.HueShiftPropId, getTintFrom.PoisonHueShift);
			}
			else
			{
				component.EnableKeyword("RECOLOUR");
				component.color = poisonTint;
			}
			ParticleSystem.MainModule main = ptBreak.main;
			main.startColor = poisonTint;
			ptPoisonIdle.Play();
		}
		else
		{
			component.DisableKeyword("CAN_HUESHIFT");
			component.DisableKeyword("RECOLOUR");
			component.color = Color.white;
			ParticleSystem.MainModule main2 = ptBreak.main;
			main2.startColor = Color.white;
		}
	}

	private void OnDisable()
	{
		base.transform.localScale = initialScale;
		wasNailHit = false;
		damager.magnitudeMult = initialDamageMult;
		damager.damageDealt = initialDamage;
	}

	private void FixedUpdate()
	{
		if (bounceCoolDown > 0f)
		{
			bounceCoolDown -= Time.fixedDeltaTime;
			if (bounceCoolDown <= 0f && (bool)damager)
			{
				damager.damageDealt = initialDamage;
			}
		}
		if (doSetup)
		{
			doSetup = false;
			bool flag = base.transform.localScale.x > 0f;
			float randomValue = startRotation.GetRandomValue();
			base.transform.SetRotation2D(flag ? randomValue : (0f - randomValue));
			damager.direction = ((!flag) ? 180 : 0);
			damager.enabled = true;
			if ((bool)animator)
			{
				animator.Play(flyAnim);
			}
			initialPosition = base.transform.position;
			previousPosition = initialPosition;
			targetPosition = base.transform.TransformPoint(flyOutPosition);
			elapsedTime = 0f;
			currentDamageVelocityLerpTime = damageVelocityLerpTime;
			currentState = States.Translating;
		}
		switch (currentState)
		{
		case States.Broken:
			UpdateBroken();
			break;
		case States.Stuck:
			UpdateStuck();
			break;
		case States.Translating:
			UpdateTranslating();
			break;
		case States.Body:
			UpdateBody();
			break;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer != 8)
		{
			return;
		}
		switch (currentState)
		{
		case States.Body:
			stuckCollider = collision.collider;
			StickInWall(collision);
			break;
		case States.Translating:
			if (bounceCoolDown <= 0f)
			{
				if (elapsedTime / flyOutCurveDuration >= flyOutReturnPoint)
				{
					stuckCollider = collision.collider;
					StickInWall(collision);
				}
				else
				{
					HitWall();
					BounceOffWall();
				}
			}
			break;
		}
	}

	private DirectionPos GetClosestWall()
	{
		CircleCollider2D circle = (CircleCollider2D)collider;
		if ((bool)circle)
		{
			Vector2 pos = base.transform.TransformPoint(circle.offset);
			float radius = base.transform.TransformRadius(circle.radius);
			return RunCastForDirections((Vector2 dir) => Physics2D.CircleCast(pos - dir * circle.radius, radius, dir, radius));
		}
		Debug.LogError("Collider type not implemented!", this);
		return DirectionPos.GetDefault(base.transform);
	}

	private DirectionPos RunCastForDirections(Func<Vector2, RaycastHit2D> func)
	{
		RaycastHit2D raycastHit2D = func(Vector2.right);
		if (raycastHit2D.collider != null)
		{
			DirectionPos result = default(DirectionPos);
			result.Direction = Direction.Right;
			result.Position = raycastHit2D.point;
			return result;
		}
		raycastHit2D = func(Vector2.left);
		if (raycastHit2D.collider != null)
		{
			DirectionPos result = default(DirectionPos);
			result.Direction = Direction.Left;
			result.Position = raycastHit2D.point;
			return result;
		}
		raycastHit2D = func(Vector2.up);
		if (raycastHit2D.collider != null)
		{
			DirectionPos result = default(DirectionPos);
			result.Direction = Direction.Up;
			result.Position = raycastHit2D.point;
			return result;
		}
		raycastHit2D = func(Vector2.down);
		if (raycastHit2D.collider != null)
		{
			DirectionPos result = default(DirectionPos);
			result.Direction = Direction.Down;
			result.Position = raycastHit2D.point;
			return result;
		}
		return DirectionPos.GetDefault(base.transform);
	}

	private Direction HitWall()
	{
		DirectionPos closestWall = GetClosestWall();
		if ((bool)wallHitPrefab)
		{
			wallHitPrefab.Spawn(closestWall.Position);
		}
		return closestWall.Direction;
	}

	private void StickInWall(Collision2D collision)
	{
		Direction direction = HitWall();
		bool flag = base.transform.localScale.x > 0f;
		damager.enabled = false;
		damagerCollider.enabled = false;
		float randomValue = stickRotation.GetRandomValue();
		base.transform.SetRotation2D(flag ? randomValue : (0f - randomValue));
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z + 0.006f);
		if ((bool)animator)
		{
			animator.Play(stickAnim);
		}
		childStickEffect.SetAllActive(value: true);
		if ((flag && collision.relativeVelocity.x > 0f) || (!flag && collision.relativeVelocity.x < 0f))
		{
			base.transform.FlipLocalScale(x: true);
		}
		if ((direction == Direction.Up && base.transform.localScale.y > 0f) || (direction == Direction.Down && base.transform.localScale.y < 0f))
		{
			base.transform.FlipLocalScale(x: false, y: true);
		}
		currentState = States.Stuck;
		elapsedTime = 0f;
		body.isKinematic = true;
		audioSource.Stop();
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
	}

	private void Stuck()
	{
		Direction direction = HitWall();
		bool flag = base.transform.localScale.x > 0f;
		damager.enabled = false;
		damagerCollider.enabled = false;
		float randomValue = stickRotation.GetRandomValue();
		base.transform.SetRotation2D(flag ? randomValue : (0f - randomValue));
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z + 0.006f);
		if ((bool)animator)
		{
			animator.Play(stickAnim);
		}
		childStickEffect.SetAllActive(value: true);
		if ((direction == Direction.Up && base.transform.localScale.y > 0f) || (direction == Direction.Down && base.transform.localScale.y < 0f))
		{
			base.transform.FlipLocalScale(x: false, y: true);
		}
		currentState = States.Stuck;
		elapsedTime = 0f;
		body.isKinematic = true;
		audioSource.Stop();
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
	}

	private void BounceOffWall()
	{
		Vector2 vector = SamplePosAtTime(initialPosition, targetPosition, flyOutReturnPoint);
		Vector2 vector2 = (Vector2)base.transform.position - vector;
		initialPosition += vector2;
		targetPosition += vector2;
		elapsedTime = flyOutReturnPoint * flyOutCurveDuration;
		if ((bool)damager)
		{
			damager.damageDealt = 0;
		}
		bounceCoolDown = 0.1f;
	}

	private void UpdateTranslating()
	{
		float t = elapsedTime / flyOutCurveDuration;
		Vector2 vector = SamplePosAtTime(initialPosition, targetPosition, t);
		if (previousPosition != vector)
		{
			Vector2 vector2 = vector - previousPosition;
			if (flyOutPosition.x > flyOutPosition.y)
			{
				damager.direction = ((!(vector2.x > 0f)) ? 180 : 0);
			}
			else
			{
				damager.direction = ((vector2.y > 0f) ? 90 : 270);
			}
		}
		if (elapsedTime >= flyOutCurveDuration)
		{
			StartBodyControl();
			return;
		}
		previousPosition = body.position;
		body.MovePosition(vector);
		float rotation = body.rotation;
		rotation += ((base.transform.localScale.x > 0f) ? flySpinSpeed : (0f - flySpinSpeed)) * Time.deltaTime;
		body.MoveRotation(rotation);
		elapsedTime += Time.deltaTime * GetSpeedMult();
	}

	private void StartBodyControl()
	{
		States num = currentState;
		currentState = States.Body;
		body.isKinematic = false;
		if (num == States.Translating)
		{
			float num2 = Mathf.Clamp01(elapsedTime / flyOutCurveDuration);
			if (num2 > 0.01f)
			{
				float t = num2 - 0.01f;
				float num3 = 0.01f * flyOutCurveDuration;
				Vector2 vector = SamplePosAtTime(initialPosition, targetPosition, num2);
				Vector2 vector2 = SamplePosAtTime(initialPosition, targetPosition, t);
				Vector2 vector3 = (vector - vector2) / num3;
				bodyVelocity = vector3;
			}
		}
		body.angularVelocity = ((base.transform.localScale.x > 0f) ? flySpinSpeed : (0f - flySpinSpeed));
	}

	private void UpdateStuck()
	{
		if (elapsedTime >= stuckIdleDuration || ((bool)stuckCollider && !stuckCollider.isActiveAndEnabled))
		{
			Break();
		}
		elapsedTime += Time.deltaTime;
	}

	private void UpdateBroken()
	{
		if (Time.timeAsDouble >= recycleTime)
		{
			base.gameObject.Recycle();
		}
	}

	private void UpdateBody()
	{
		if (doFlattenVelocity)
		{
			bodyVelocity.y = Mathf.Lerp(bodyVelocity.y, 0f, horizontalKnockbackFlattenSpeed * Time.deltaTime);
		}
		body.linearVelocity = bodyVelocity * GetSpeedMult();
		if (CheckStuck())
		{
			Stuck();
		}
	}

	private bool CheckStuck()
	{
		Vector2 currentPosition = body.position;
		if (positionCount < 3)
		{
			positionCount++;
			AddCurrentPosition();
			return false;
		}
		if ((positionHistory[positionIndex] - currentPosition).sqrMagnitude > 0.0225f)
		{
			AddCurrentPosition();
			return false;
		}
		return true;
		void AddCurrentPosition()
		{
			positionHistory[positionIndex] = currentPosition;
			positionIndex = (positionIndex + 1) % 3;
		}
	}

	public void ResetStuckDetection()
	{
		positionIndex = 0;
		positionCount = 0;
	}

	private float GetSpeedMult()
	{
		currentDamageVelocityLerpTime += Time.deltaTime;
		return damageVelocityLerpCurve.Evaluate(Mathf.Clamp01(currentDamageVelocityLerpTime / damageVelocityLerpTime));
	}

	private void OnDamagedEnemy()
	{
		currentDamageVelocityLerpTime = 0f;
		if (wasNailHit)
		{
			Break();
		}
	}

	public void Break()
	{
		body.isKinematic = true;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		childBreakEffects.SetAllActive(value: true);
		if (recycleDelay <= 0f)
		{
			base.gameObject.Recycle();
		}
		else
		{
			recycleTime = Time.timeAsDouble + (double)recycleDelay;
			if ((bool)renderer)
			{
				renderer.enabled = false;
			}
		}
		damager.enabled = false;
		audioSource.Stop();
		currentState = States.Broken;
	}

	private Vector2 SamplePosAtTime(Vector2 from, Vector2 to, float t)
	{
		Vector2 zero = Vector2.zero;
		zero.x = Mathf.LerpUnclamped(from.x, to.x, flyOutCurveX.Evaluate(t));
		zero.y = Mathf.LerpUnclamped(from.y, to.y, flyOutCurveY.Evaluate(t));
		return zero;
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (currentState == States.Broken)
		{
			return IHitResponder.Response.None;
		}
		if ((bool)damageInstance.Source && (bool)damageInstance.Source.GetComponentInParent<ToolBoomerang>(includeInactive: true))
		{
			return IHitResponder.Response.None;
		}
		if ((bool)nailHitEffectPrefab)
		{
			nailHitEffectPrefab.Spawn(base.transform.position);
		}
		if (currentState == States.Stuck)
		{
			return IHitResponder.Response.None;
		}
		if (damageInstance.IsNailDamage)
		{
			bool flag = base.transform.localScale.x > 0f;
			switch (damageInstance.GetHitDirection(HitInstance.TargetType.Regular))
			{
			case HitInstance.HitDirection.Left:
				doSetup = true;
				if (flag)
				{
					base.transform.FlipLocalScale(x: true);
				}
				break;
			case HitInstance.HitDirection.Right:
				doSetup = true;
				if (!flag)
				{
					base.transform.FlipLocalScale(x: true);
				}
				return IHitResponder.Response.GenericHit;
			}
			if (currentState == States.Translating)
			{
				StartBodyControl();
			}
			NailKnockBack(damageInstance.GetHitDirection(HitInstance.TargetType.Regular));
			nailDeflectClip.SpawnAndPlayOneShot(base.transform.position);
		}
		else
		{
			WeakKnockBack(damageInstance.GetHitDirection(HitInstance.TargetType.Regular));
		}
		return IHitResponder.Response.GenericHit;
	}

	private void NailKnockBack(HitInstance.HitDirection hitDir)
	{
		switch (hitDir)
		{
		case HitInstance.HitDirection.Left:
			bodyVelocity = new Vector2(0f - horizontalKnockbackDirection.x, horizontalKnockbackDirection.y).normalized * knockbackSpeed;
			damager.direction = 180f;
			doFlattenVelocity = true;
			break;
		case HitInstance.HitDirection.Right:
			bodyVelocity = horizontalKnockbackDirection.normalized * knockbackSpeed;
			damager.direction = 0f;
			doFlattenVelocity = true;
			break;
		case HitInstance.HitDirection.Up:
			bodyVelocity = new Vector2(0f, knockbackSpeed);
			damager.direction = 90f;
			break;
		case HitInstance.HitDirection.Down:
			if (HeroController.instance.cState.facingRight)
			{
				bodyVelocity = downKnockbackDirection;
				damager.direction = 0f;
			}
			else
			{
				bodyVelocity = new Vector2(0f - downKnockbackDirection.x, downKnockbackDirection.y);
				damager.direction = 180f;
			}
			bodyVelocity = bodyVelocity.normalized * knockbackSpeed;
			break;
		}
		body.linearVelocity = bodyVelocity * GetSpeedMult();
		damager.damageDealt = Mathf.RoundToInt((float)damager.damageDealt * hitDamageMult);
		damager.magnitudeMult *= hitDamageMult;
		wasNailHit = true;
	}

	private void WeakKnockBack(HitInstance.HitDirection hitDir)
	{
		Vector2 zero = Vector2.zero;
		switch (hitDir)
		{
		case HitInstance.HitDirection.Left:
		case HitInstance.HitDirection.Right:
		case HitInstance.HitDirection.Up:
			zero.y += weakKnockbackAmount;
			break;
		case HitInstance.HitDirection.Down:
			zero.y -= weakKnockbackAmount;
			break;
		}
		initialPosition += zero;
		targetPosition += zero;
		damager.damageDealt = Mathf.RoundToInt((float)damager.damageDealt * hitDamageMult);
		damager.magnitudeMult *= hitDamageMult;
	}

	public void Tinked()
	{
		if (currentState == States.Translating)
		{
			BounceOffWall();
		}
	}

	private void OnWillDamageEnemy(HealthManager enemyHealthManager, HitInstance damageInstance)
	{
		if (enemyHealthManager.IsBlockingByDirection(DirectionUtils.GetCardinalDirection(damageInstance.Direction), damageInstance.AttackType))
		{
			Tinked();
		}
	}
}
