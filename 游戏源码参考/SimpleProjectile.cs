using GlobalSettings;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleProjectile : MonoBehaviour
{
	[SerializeField]
	private GameObject spawnFlingOnBreak;

	[SerializeField]
	[Range(0f, 1f)]
	private float spawnChance;

	[SerializeField]
	private FlingUtils.ObjectFlingParams spawnFling;

	[SerializeField]
	private GameObject terrainImpactSpawn;

	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnim")]
	private string shootAnim;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnim")]
	private string[] brokenAnims;

	[SerializeField]
	[Range(0f, 1f)]
	private float shatterChance;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private DamageEnemies damager;

	[SerializeField]
	private ParticleSystem breakEffect;

	[SerializeField]
	private ParticleSystem breakEffectPoison;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private MeshRenderer meshRenderer;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Collider2D terrainDetector;

	[SerializeField]
	[Range(0f, 1f)]
	private float brokenSolidChance = 1f;

	private bool queuedUpdate;

	private bool hasHit;

	private bool breakOnLand;

	private bool hasBrokenOnLand;

	private float recycleTimer;

	private bool isPoison;

	private ParticleSystem breakEffectCurrent;

	private Rigidbody2D body;

	private Collider2D damagerCollider;

	private PoisonTintBase tinter;

	private bool? ValidateAnim(string animName)
	{
		if (!animator || string.IsNullOrEmpty(animName))
		{
			return null;
		}
		return animator.GetClipByName(animName) != null;
	}

	private void Awake()
	{
		damager.DamagedEnemy += Break;
		damagerCollider = damager.GetComponent<Collider2D>();
		body = GetComponent<Rigidbody2D>();
		tinter = GetComponent<PoisonTintBase>();
	}

	private void OnEnable()
	{
		damagerCollider.enabled = true;
		hasHit = false;
		breakOnLand = false;
		hasBrokenOnLand = false;
		body.linearVelocity = Vector2.zero;
		body.gravityScale = 0f;
		body.isKinematic = false;
		meshRenderer.enabled = true;
		terrainDetector.enabled = true;
		isPoison = Gameplay.PoisonPouchTool.IsEquipped;
		queuedUpdate = true;
	}

	private void Update()
	{
		if (recycleTimer > 0f)
		{
			recycleTimer -= Time.deltaTime;
			if (recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
			}
		}
		if (hasBrokenOnLand && (bool)breakEffectCurrent && !breakEffectCurrent.IsAlive(withChildren: true))
		{
			base.gameObject.Recycle();
		}
		if (!hasHit && queuedUpdate)
		{
			queuedUpdate = false;
			if ((bool)animator && !string.IsNullOrEmpty(shootAnim))
			{
				animator.Play(shootAnim);
			}
			Vector2 normalized = body.linearVelocity.normalized;
			float num = Vector2.Angle(Vector2.right, normalized);
			if ((bool)damager)
			{
				damager.direction = num;
			}
			base.transform.SetRotation2D(num);
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (breakOnLand)
		{
			DoBreakEffect();
			meshRenderer.enabled = false;
			body.isKinematic = true;
			body.linearVelocity = Vector2.zero;
			hasBrokenOnLand = true;
		}
		else if (!hasHit)
		{
			Vector2 point = other.GetSafeContact().Point;
			if ((bool)terrainImpactSpawn)
			{
				terrainImpactSpawn.Spawn(point.ToVector3(terrainImpactSpawn.transform.position.z), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
			}
			DidHit(other);
		}
	}

	private void DoBreakEffect()
	{
		breakEffectCurrent = (isPoison ? breakEffectPoison : breakEffect);
		if ((bool)breakEffectCurrent)
		{
			breakEffectCurrent.Play(withChildren: true);
		}
	}

	public void Break()
	{
		if (!hasHit)
		{
			DidHit(null);
		}
	}

	private void DidHit(Collision2D other)
	{
		if (Random.Range(0f, 1f) <= spawnChance)
		{
			BreakSpawn(other);
			base.gameObject.Recycle();
			return;
		}
		if ((bool)tinter)
		{
			tinter.Clear();
		}
		if (Random.Range(0f, 1f) <= shatterChance)
		{
			DoBreakEffect();
			meshRenderer.enabled = false;
			body.isKinematic = true;
			body.linearVelocity = Vector2.zero;
		}
		else
		{
			FlingBack(base.gameObject, other);
			body.gravityScale = 1f;
			if (brokenAnims.Length != 0)
			{
				animator.Play(brokenAnims.GetRandomElement());
			}
			if (Random.Range(0f, 1f) <= brokenSolidChance)
			{
				breakOnLand = true;
			}
			else
			{
				terrainDetector.enabled = false;
			}
		}
		hasHit = true;
		damagerCollider.enabled = false;
		recycleTimer = 5f;
	}

	private void BreakSpawn(Collision2D other)
	{
		if ((bool)spawnFlingOnBreak)
		{
			GameObject obj = spawnFlingOnBreak.Spawn();
			FlingBack(obj, other);
		}
	}

	private void FlingBack(GameObject obj, Collision2D other)
	{
		FlingUtils.SelfConfig selfConfig = spawnFling.GetSelfConfig(obj);
		bool flag;
		if (other != null)
		{
			Collision2DUtils.Collision2DSafeContact safeContact = other.GetSafeContact();
			flag = ((!(Mathf.Abs(Vector2.Dot(safeContact.Normal, Vector2.right)) > 0.5f)) ? (body.linearVelocity.x > 0f) : (safeContact.Point.x < base.transform.position.x));
		}
		else
		{
			float num;
			for (num = base.transform.eulerAngles.z; num < 0f; num += 360f)
			{
			}
			while (num >= 360f)
			{
				num -= 360f;
			}
			flag = num > 90f && num < 270f;
		}
		if (flag)
		{
			selfConfig.AngleMin = Helper.GetReflectedAngle(selfConfig.AngleMin, reflectHorizontal: true, reflectVertical: false);
			selfConfig.AngleMax = Helper.GetReflectedAngle(selfConfig.AngleMax, reflectHorizontal: true, reflectVertical: false);
		}
		FlingUtils.FlingObject(selfConfig, base.transform, Vector3.zero);
	}
}
