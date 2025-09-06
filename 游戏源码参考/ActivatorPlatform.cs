using System;
using UnityEngine;
using UnityEngine.Events;

public class ActivatorPlatform : AnimatorActivatingStates, IHitResponder
{
	[Space]
	[SerializeField]
	private PlayMakerFSM tiltPlatFsm;

	[SerializeField]
	private TiltPlat tiltPlatScript;

	[Space]
	[SerializeField]
	private int hitsToBreak;

	[SerializeField]
	private string breakAnim;

	[SerializeField]
	private GameObject hitEffectPrefab;

	[SerializeField]
	private CameraShakeTarget hitCamShake;

	[SerializeField]
	private CameraShakeTarget breakCamShake;

	[Space]
	public UnityEvent OnHit;

	public UnityEvent OnBreak;

	private int hits;

	private bool isBroken;

	private bool hasCollider;

	private Collider2D collider;

	private AnimatorHashCache breakAnimHashCache;

	protected override void Awake()
	{
		base.Awake();
		breakAnimHashCache = new AnimatorHashCache(breakAnim);
		collider = GetComponent<Collider2D>();
		hasCollider = collider != null;
		if (hasCollider)
		{
			collider.enabled = false;
		}
	}

	protected override void Start()
	{
		base.Start();
		if (GetUpDirection() != 1)
		{
			if ((bool)tiltPlatFsm)
			{
				tiltPlatFsm.enabled = false;
			}
			if ((bool)tiltPlatScript)
			{
				tiltPlatScript.enabled = false;
			}
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		breakAnimHashCache = new AnimatorHashCache(breakAnim);
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (isBroken)
		{
			return IHitResponder.Response.None;
		}
		HitInstance.HitDirection actualHitDirection = damageInstance.GetActualHitDirection(base.transform, HitInstance.TargetType.Regular);
		if (GetHitDirBitmask().IsBitSet((int)actualHitDirection))
		{
			return IHitResponder.Response.None;
		}
		hitCamShake.DoShake(this);
		OnHit.Invoke();
		if ((bool)hitEffectPrefab)
		{
			float overriddenDirection = damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			hitEffectPrefab.Spawn(base.transform.position).transform.SetRotation2D(Helper.GetReflectedAngle(overriddenDirection, reflectHorizontal: true, reflectVertical: false) + 180f);
		}
		hits++;
		if (hitsToBreak <= 0 || hits < hitsToBreak)
		{
			return IHitResponder.Response.GenericHit;
		}
		isBroken = true;
		SetActive(value: false, isInstant: true);
		PlayAnim(breakAnimHashCache.Hash, fromEnd: false);
		breakCamShake.DoShake(this);
		OnBreak.Invoke();
		return IHitResponder.Response.GenericHit;
	}

	protected override void OnActivate()
	{
		hits = 0;
		isBroken = false;
		if (hasCollider)
		{
			collider.enabled = true;
		}
	}

	protected override void OnDeactivate()
	{
		if (hasCollider)
		{
			collider.enabled = false;
		}
	}

	private int GetUpDirection()
	{
		Vector3 vector = base.transform.TransformVector(Vector3.up);
		float num;
		for (num = Vector2.SignedAngle(Vector2.right, vector); num < 0f; num += 360f)
		{
		}
		if (num < 45f)
		{
			return 0;
		}
		if (num < 135f)
		{
			return 1;
		}
		if (num < 225f)
		{
			return 2;
		}
		return 3;
	}

	private int GetHitDirBitmask()
	{
		return GetUpDirection() switch
		{
			0 => 0.SetBitAtIndex(0), 
			1 => 0.SetBitAtIndex(3), 
			2 => 0.SetBitAtIndex(1), 
			3 => 0.SetBitAtIndex(2), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
