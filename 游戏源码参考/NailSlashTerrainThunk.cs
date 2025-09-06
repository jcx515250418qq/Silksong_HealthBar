using System;
using System.Collections;
using UnityEngine;

public class NailSlashTerrainThunk : MonoBehaviour
{
	public delegate void TerrainThunkEvent(Vector2 thunkPos, int surfaceDir);

	[SerializeField]
	private bool generateChild;

	[Space]
	[SerializeField]
	private bool doRecoil;

	[SerializeField]
	private Transform overrideTransform;

	[SerializeField]
	private DamageEnemies multiHitter;

	[SerializeField]
	private float cooldown;

	[SerializeField]
	private bool overrideSlashDirection;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideSlashDirection", true, false, false)]
	private TerrainThunkUtils.SlashDirection slashDirectionOverride;

	[SerializeField]
	private bool handleTink;

	private double cooldownTime;

	private readonly ContactPoint2D[] contactsTempStore = new ContactPoint2D[10];

	private readonly ContactPoint2D[] queuedContactsTempStore = new ContactPoint2D[10];

	private int lastThunkFrame;

	private HeroController hc;

	private Rigidbody2D body;

	private Collider2D col;

	private int collisionQueueStepsLeft;

	private GameObject queuedCollisionGameObject;

	private int queuedCollisionCount;

	public static event TerrainThunkEvent AnyThunked;

	public event Action<Vector2> Thunked;

	private void OnValidate()
	{
		if (cooldown < 0f)
		{
			cooldown = 0f;
		}
	}

	private void Awake()
	{
		OnValidate();
		col = GetComponent<Collider2D>();
		if (!col)
		{
			return;
		}
		if (!col.isTrigger)
		{
			col.forceSendLayers = 0;
			col.forceReceiveLayers = 0;
		}
		if (generateChild)
		{
			GameObject obj = new GameObject("Terrain Thunker")
			{
				layer = 16
			};
			obj.transform.SetParentReset(base.transform);
			((Collider2D)obj.AddComponent(col.GetType())).CopyFrom(col);
			NailSlashTerrainThunk nailSlashTerrainThunk = obj.AddComponent<NailSlashTerrainThunk>();
			nailSlashTerrainThunk.doRecoil = doRecoil;
			nailSlashTerrainThunk.overrideTransform = overrideTransform;
			nailSlashTerrainThunk.multiHitter = multiHitter;
			nailSlashTerrainThunk.cooldown = cooldown;
			UnityEngine.Object.Destroy(this);
			return;
		}
		hc = base.transform.GetComponentInParent<HeroController>();
		body = GetComponent<Rigidbody2D>();
		if (!body && (bool)col)
		{
			col.isTrigger = false;
			col.forceSendLayers = 0;
			col.forceReceiveLayers = 0;
			body = base.gameObject.AddComponent<Rigidbody2D>();
			body.bodyType = RigidbodyType2D.Kinematic;
			body.simulated = true;
			body.useFullKinematicContacts = true;
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnEnable()
	{
		collisionQueueStepsLeft = 2;
	}

	private void OnDisable()
	{
		queuedCollisionGameObject = null;
	}

	private void Start()
	{
		if ((bool)multiHitter && multiHitter.multiHitter)
		{
			multiHitter.MultiHitEvaluated += OnMultiHitEvaluated;
		}
	}

	private void FixedUpdate()
	{
		if (collisionQueueStepsLeft > 0)
		{
			collisionQueueStepsLeft--;
			if (collisionQueueStepsLeft <= 0 && !(queuedCollisionGameObject == null))
			{
				HandleQueuedCollision();
				queuedCollisionGameObject = null;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (handleTink)
		{
			TinkEffect component = collision.collider.GetComponent<TinkEffect>();
			if ((bool)component)
			{
				Vector2 tinkPosition = Vector2.zero;
				if ((bool)multiHitter)
				{
					component.TryDoTinkReactionNoDamager(multiHitter.gameObject, doCamShake: true, doSound: true, isNailAttack: true, out tinkPosition);
				}
				else
				{
					component.TryDoTinkReactionNoDamager(collision.otherCollider.gameObject, doCamShake: true, doSound: true, isNailAttack: true, out tinkPosition);
				}
				this.Thunked?.Invoke(tinkPosition);
				return;
			}
		}
		HandleCollision(collision);
		if ((bool)multiHitter && !multiHitter.multiHitter)
		{
			LagHitOptions lagHits = multiHitter.LagHits;
			if (lagHits != null && lagHits.ShouldDoLagHits())
			{
				StartCoroutine(RepeatCollision(collision, lagHits.HitCount, lagHits.StartDelay, lagHits.HitDelay));
			}
		}
	}

	private void HandleCollision(Collision2D collision)
	{
		if (lastThunkFrame == Time.frameCount || Time.timeAsDouble < cooldownTime)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		if (!gameObject)
		{
			return;
		}
		ForceThunker component = gameObject.GetComponent<ForceThunker>();
		if (gameObject.layer != 8 && !component)
		{
			return;
		}
		if (collisionQueueStepsLeft > 0)
		{
			queuedCollisionGameObject = gameObject;
			queuedCollisionCount = collision.GetContacts(queuedContactsTempStore);
			return;
		}
		lastThunkFrame = Time.frameCount;
		TerrainThunkUtils.SlashDirection slashDirection;
		if (overrideSlashDirection)
		{
			slashDirection = slashDirectionOverride;
			if (base.transform.lossyScale.x < 0f)
			{
				slashDirection = slashDirection switch
				{
					TerrainThunkUtils.SlashDirection.Left => TerrainThunkUtils.SlashDirection.Right, 
					TerrainThunkUtils.SlashDirection.Right => TerrainThunkUtils.SlashDirection.Left, 
					_ => slashDirection, 
				};
			}
		}
		else if (hc.cState.upAttacking)
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Up;
		}
		else if (!hc.cState.downAttacking)
		{
			slashDirection = ((!hc.cState.facingRight) ? TerrainThunkUtils.SlashDirection.Left : TerrainThunkUtils.SlashDirection.Right);
		}
		else
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Down;
			hc.crestAttacksFSM.SendEvent("THUNKED GROUND");
		}
		cooldownTime = Time.timeAsDouble + (double)cooldown;
		int recoilDir;
		int surfaceDir;
		Vector2 vector = TerrainThunkUtils.GenerateTerrainThunk(collision, contactsTempStore, slashDirection, overrideTransform ? overrideTransform.position : hc.transform.position, out recoilDir, out surfaceDir);
		if (!doRecoil || recoilDir < 0)
		{
			return;
		}
		if (hc.cState.downAttacking)
		{
			if ((bool)component && component.PreventDownBounce)
			{
				return;
			}
			hc.crestAttacksFSM.SendEvent("THUNKED DOWN");
		}
		else if (hc.cState.upAttacking)
		{
			if (surfaceDir == 1 || surfaceDir == 3)
			{
				hc.RecoilDown();
			}
		}
		else if (surfaceDir == 2 || surfaceDir == 0)
		{
			hc.SetAllowRecoilWhileRelinquished(value: true);
			if (hc.cState.facingRight)
			{
				hc.RecoilLeft();
			}
			else
			{
				hc.RecoilRight();
			}
			hc.SetAllowRecoilWhileRelinquished(value: false);
		}
		this.Thunked?.Invoke(vector);
		NailSlashTerrainThunk.AnyThunked?.Invoke(vector, surfaceDir);
	}

	private void HandleQueuedCollision()
	{
		if (lastThunkFrame == Time.frameCount || Time.timeAsDouble < cooldownTime || queuedCollisionGameObject == null)
		{
			return;
		}
		ForceThunker component = queuedCollisionGameObject.GetComponent<ForceThunker>();
		lastThunkFrame = Time.frameCount;
		TerrainThunkUtils.SlashDirection slashDirection;
		if (overrideSlashDirection)
		{
			slashDirection = slashDirectionOverride;
			if (base.transform.lossyScale.x < 0f)
			{
				slashDirection = slashDirection switch
				{
					TerrainThunkUtils.SlashDirection.Left => TerrainThunkUtils.SlashDirection.Right, 
					TerrainThunkUtils.SlashDirection.Right => TerrainThunkUtils.SlashDirection.Left, 
					_ => slashDirection, 
				};
			}
		}
		else if (hc.cState.upAttacking)
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Up;
		}
		else if (!hc.cState.downAttacking)
		{
			slashDirection = ((!hc.cState.facingRight) ? TerrainThunkUtils.SlashDirection.Left : TerrainThunkUtils.SlashDirection.Right);
		}
		else
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Down;
			hc.crestAttacksFSM.SendEvent("THUNKED GROUND");
		}
		cooldownTime = Time.timeAsDouble + (double)cooldown;
		int recoilDir;
		int surfaceDir;
		Vector2 vector = TerrainThunkUtils.GenerateTerrainThunk(queuedCollisionCount, queuedContactsTempStore, slashDirection, overrideTransform ? overrideTransform.position : hc.transform.position, out recoilDir, out surfaceDir);
		if (!doRecoil || recoilDir < 0)
		{
			return;
		}
		if (hc.cState.downAttacking)
		{
			if ((bool)component && component.PreventDownBounce)
			{
				return;
			}
			hc.crestAttacksFSM.SendEvent("THUNKED DOWN");
		}
		else if (hc.cState.upAttacking)
		{
			if (surfaceDir == 1 || surfaceDir == 3)
			{
				hc.RecoilDown();
			}
		}
		else if (surfaceDir == 2 || surfaceDir == 0)
		{
			hc.SetAllowRecoilWhileRelinquished(value: true);
			if (hc.cState.facingRight)
			{
				hc.RecoilLeft();
			}
			else
			{
				hc.RecoilRight();
			}
			hc.SetAllowRecoilWhileRelinquished(value: false);
		}
		this.Thunked?.Invoke(vector);
		NailSlashTerrainThunk.AnyThunked?.Invoke(vector, surfaceDir);
	}

	private IEnumerator RepeatCollision(Collision2D collision, int repeatCount, float startDelay, float repeatDelay)
	{
		yield return new WaitForSeconds(startDelay);
		WaitForSeconds wait = new WaitForSeconds(repeatDelay);
		for (int i = 0; i < repeatCount; i++)
		{
			HandleCollision(collision);
			yield return wait;
		}
	}

	private void OnMultiHitEvaluated()
	{
		if ((bool)col && col.enabled)
		{
			col.enabled = false;
			col.enabled = true;
		}
	}

	public bool WillHandleTink(Collider2D otherCol)
	{
		if (!handleTink)
		{
			return false;
		}
		int layerCollisionMask = Physics2D.GetLayerCollisionMask(col.gameObject.layer);
		int num = 1 << otherCol.gameObject.layer;
		return (layerCollisionMask & num) != 0;
	}

	public static void ReportDownspikeHitGround(Vector2 hitPoint)
	{
		NailSlashTerrainThunk.AnyThunked?.Invoke(hitPoint, 1);
	}
}
