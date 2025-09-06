using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public class NailSlashTravel : MonoBehaviour
{
	[SerializeField]
	private NailSlash slash;

	[SerializeField]
	private DamageEnemies damager;

	[SerializeField]
	private Transform distanceFiller;

	[SerializeField]
	private GameObject terrainThunker;

	[Space]
	[SerializeField]
	private float groundedYOffset;

	[SerializeField]
	private Vector2 travelDistance;

	[SerializeField]
	private float travelDuration;

	[SerializeField]
	private AnimationCurve travelCurve;

	[SerializeField]
	[Range(0f, 1f)]
	private float recoilDistance;

	[Space]
	[SerializeField]
	private GameObject impactPrefab;

	[SerializeField]
	private Vector2 impactScale = Vector2.one;

	[SerializeField]
	private Vector2 impactOffset;

	[SerializeField]
	private OverrideFloat maxXOffset;

	[SerializeField]
	private OverrideFloat maxYOffset;

	[Space]
	[SerializeField]
	private ParticleSystem particles;

	[Space]
	[SerializeField]
	private GameObject bouncePrefab;

	[SerializeField]
	private bool onlyThunkCancelIfColliderEnabled;

	private float elapsedT;

	private Action setPosition;

	private bool hasStarted;

	private HeroController hc;

	private bool isSlashActive;

	private Coroutine travelRoutine;

	private Vector3 initialLocalPos;

	private Vector3 initialLocalScale;

	private bool hasCollider;

	private Collider2D collider2D;

	private bool wasColliderActive;

	private bool queuedThunkerStateChanged;

	private bool targetThunkerState;

	private bool currentThunkerState;

	private bool hasSlash;

	private HashSet<GameObject> impactTargets = new HashSet<GameObject>();

	private HashSet<GameObject> bounceTargets = new HashSet<GameObject>();

	private void Reset()
	{
		slash = GetComponent<NailSlash>();
		damager = GetComponent<DamageEnemies>();
	}

	private void Awake()
	{
		hasSlash = slash;
		if (hasSlash)
		{
			slash.AttackStarting += OnSlashStarting;
			slash.EndedDamage += OnDamageEnded;
			slash.SetLongNeedleHandled();
		}
		if ((bool)damager)
		{
			damager.HitResponded += OnDamaged;
		}
		collider2D = GetComponent<Collider2D>();
		hasCollider = collider2D != null;
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			ResetTransform();
			SetInitialPos();
		}
	}

	private void Start()
	{
		if (!hasStarted)
		{
			Transform transform = base.transform;
			initialLocalPos = transform.localPosition;
			initialLocalScale = transform.localScale;
			NailSlashTerrainThunk componentInChildren = GetComponentInChildren<NailSlashTerrainThunk>(includeInactive: true);
			if ((bool)componentInChildren)
			{
				componentInChildren.Thunked += OnThunked;
			}
			hc = GetComponentInParent<HeroController>();
			hc.FlippedSprite += OnHeroFlipped;
			hasStarted = true;
			SetInitialPos();
		}
	}

	private void OnDestroy()
	{
		if ((bool)hc)
		{
			hc.FlippedSprite -= OnHeroFlipped;
		}
	}

	private void FixedUpdate()
	{
		if (!hasCollider || !isSlashActive)
		{
			return;
		}
		if (collider2D.enabled)
		{
			wasColliderActive = true;
			if (queuedThunkerStateChanged)
			{
				SetThunkerActive(targetThunkerState);
			}
		}
		else if (wasColliderActive && !collider2D.enabled)
		{
			if (hasSlash)
			{
				slash.SetCollidersActive(value: true);
			}
			SetThunkerActive(active: true);
		}
	}

	private void SetInitialPos()
	{
		if (hc.cState.onGround)
		{
			Vector3 localPosition = initialLocalPos;
			localPosition.y += groundedYOffset;
			base.transform.localPosition = localPosition;
			if ((bool)distanceFiller)
			{
				distanceFiller.gameObject.SetActive(value: false);
			}
			SetThunkerActive(active: false);
		}
	}

	private void OnDisable()
	{
		ResetTransform();
		ClearTargets();
	}

	private void ResetTransform()
	{
		if (!hasStarted)
		{
			Start();
		}
		Transform obj = base.transform;
		obj.localPosition = initialLocalPos;
		obj.localScale = initialLocalScale;
	}

	public void OnSlashStarting()
	{
		isSlashActive = true;
		wasColliderActive = false;
		if (travelRoutine != null)
		{
			StopCoroutine(travelRoutine);
		}
		ResetTransform();
		if ((bool)particles)
		{
			particles.Play(withChildren: true);
		}
		hc.AllowRecoil();
		travelRoutine = StartCoroutine(Travel());
	}

	private void OnDamageEnded(bool didHit)
	{
		isSlashActive = false;
		wasColliderActive = false;
		if ((bool)distanceFiller)
		{
			distanceFiller.gameObject.SetActive(value: false);
		}
		SetThunkerActive(active: false);
		ClearTargets();
	}

	private void ClearTargets()
	{
		impactTargets.Clear();
		bounceTargets.Clear();
	}

	private void OnDamaged(DamageEnemies.HitResponse hitResponse)
	{
		if (hasSlash && (!slash.IsDamagerActive || !slash.CanDamageEnemies))
		{
			return;
		}
		PhysLayers layerOnHit = hitResponse.LayerOnHit;
		if (layerOnHit != PhysLayers.TERRAIN && layerOnHit != PhysLayers.ENEMIES && !(hitResponse.HealthManager != null))
		{
			return;
		}
		IHitResponder responder = hitResponse.Responder;
		if ((responder is IBreakerBreakable breakerBreakable && !breakerBreakable.gameObject.CompareTag("Recoiler")) || responder is HitRigidbody2D || responder is ChainAttackForce || responder is ChainInteraction)
		{
			return;
		}
		Vector3 vector = hitResponse.Target.transform.position;
		if (hasCollider)
		{
			vector = collider2D.ClosestPoint(vector);
		}
		Vector3 position = base.transform.position;
		Vector3 vector2 = vector - position;
		if (maxXOffset.IsEnabled)
		{
			if (vector2.x > maxXOffset.Value)
			{
				vector2.x -= maxXOffset.Value;
				vector.x -= vector2.x;
			}
			if (vector2.x < 0f - maxXOffset.Value)
			{
				vector2.x += maxXOffset.Value;
				vector.x += vector2.x;
			}
		}
		if (maxYOffset.IsEnabled)
		{
			if (vector2.y > maxYOffset.Value)
			{
				vector2.y -= maxYOffset.Value;
				vector.y -= vector2.y;
			}
			if (vector2.y < 0f - maxYOffset.Value)
			{
				vector2.y += maxYOffset.Value;
				vector.y += vector2.y;
			}
		}
		if (impactTargets.Add(hitResponse.Target))
		{
			StopTravelImpact(vector);
		}
		if (!bounceTargets.Contains(hitResponse.Target) && !(hitResponse.Responder is BouncePod))
		{
			NonBouncer component = hitResponse.Target.GetComponent<NonBouncer>();
			if ((!(component != null) || !component.active) && bounceTargets.Add(hitResponse.Target))
			{
				DoBounceEffect(vector);
			}
		}
	}

	private void OnThunked(Vector2 hitPoint)
	{
		if (!onlyThunkCancelIfColliderEnabled || !hasCollider || collider2D.enabled)
		{
			StopTravelImpact(hitPoint);
		}
	}

	private void StopTravelImpact(Vector3 pos)
	{
		if (travelRoutine != null)
		{
			StopCoroutine(travelRoutine);
			travelRoutine = null;
		}
		SpawnImpactEffect(pos);
		if ((bool)slash)
		{
			slash.CancelAttack();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void SpawnImpactEffect(Vector3 pos)
	{
		Transform transform = base.transform;
		Vector3 position = transform.InverseTransformPoint(pos);
		position.x += impactOffset.x;
		position.y += impactOffset.y;
		Vector3 position2 = transform.TransformPoint(position);
		GameObject obj = impactPrefab.Spawn(position2, transform.rotation);
		Vector3 lossyScale = transform.lossyScale;
		Vector2 vector = new Vector2(0f - Mathf.Sign(lossyScale.x), Mathf.Sign(lossyScale.y));
		obj.transform.localScale = impactScale * vector * impactPrefab.transform.localScale;
	}

	private void SetThunkerActive(bool active)
	{
		targetThunkerState = active;
		if (active && !wasColliderActive)
		{
			queuedThunkerStateChanged = true;
			return;
		}
		queuedThunkerStateChanged = false;
		if (targetThunkerState != currentThunkerState && (bool)terrainThunker)
		{
			currentThunkerState = active;
			terrainThunker.SetActive(active);
		}
	}

	private IEnumerator Travel()
	{
		yield return null;
		Transform trans = base.transform;
		Vector2 worldPos = trans.position;
		hc.AllowRecoil();
		bool disabledRecoil = false;
		if (hc.cState.onGround)
		{
			worldPos.y += groundedYOffset;
		}
		if (isSlashActive)
		{
			if ((bool)distanceFiller)
			{
				distanceFiller.gameObject.SetActive(value: true);
			}
			SetThunkerActive(active: true);
		}
		Vector2 heroVelocity = hc.Body.linearVelocity;
		Vector2 distanceMultiplier = (Gameplay.LongNeedleTool.IsEquipped ? Gameplay.LongNeedleMultiplier : Vector2.one);
		setPosition = delegate
		{
			float x = Mathf.Sign(trans.lossyScale.x);
			Vector2 self = travelDistance.MultiplyElements(new Vector2(x, 1f));
			Vector2 vector = self.MultiplyElements(distanceMultiplier);
			if ((bool)damager)
			{
				Vector2 normalized = self.normalized;
				heroVelocity = ((Vector2.Dot(heroVelocity.normalized, normalized) > 0f) ? heroVelocity.MultiplyElements(normalized.Abs()) : Vector2.zero);
				vector += heroVelocity * (0.5f * travelDuration);
			}
			float num = travelCurve.Evaluate(elapsedT);
			trans.SetPosition2D(worldPos + vector * num);
			if ((bool)distanceFiller)
			{
				float num2 = Mathf.Abs(vector.x) * num;
				distanceFiller.SetScaleX(num2);
				distanceFiller.SetLocalPositionX(num2 * 0.5f);
			}
		};
		for (float elapsed = 0f; elapsed < travelDuration; elapsed += Time.deltaTime)
		{
			elapsedT = elapsed / travelDuration;
			setPosition();
			if (!disabledRecoil && elapsedT >= recoilDistance)
			{
				disabledRecoil = true;
				hc.PreventRecoil(1f - recoilDistance * travelDuration);
			}
			yield return null;
		}
		travelRoutine = null;
	}

	private void OnHeroFlipped()
	{
		if (travelRoutine != null)
		{
			if (hasSlash)
			{
				slash.SetCollidersActive(value: false);
			}
			if ((bool)distanceFiller)
			{
				distanceFiller.gameObject.SetActive(value: false);
			}
			SetThunkerActive(active: false);
			setPosition?.Invoke();
			float z = base.transform.localEulerAngles.z;
			bool flag = z > -45f && z < 45f;
			base.transform.FlipLocalScale(flag, !flag);
		}
	}

	public void DoBounceEffect(Vector2 fromPos)
	{
		if ((bool)bouncePrefab)
		{
			Vector2 vector = hc.transform.position;
			float z = (fromPos - vector).normalized.DirectionToAngle();
			bouncePrefab.Spawn(fromPos, Quaternion.Euler(0f, 0f, z));
		}
	}
}
