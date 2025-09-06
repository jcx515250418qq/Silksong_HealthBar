using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Recoil : MonoBehaviour
{
	public interface IRecoilMultiplier
	{
		float GetRecoilMultiplier();
	}

	private enum States
	{
		Ready = 0,
		Frozen = 1,
		Recoiling = 2
	}

	public delegate void FreezeEvent();

	public delegate void CancelRecoilEvent();

	[FormerlySerializedAs("freezeInPlace")]
	public bool FreezeInPlace;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("FreezeInPlace", false, false, false)]
	private float recoilSpeedBase;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("FreezeInPlace", false, false, false)]
	private float recoilDuration;

	[Space]
	public bool IsUpBlocked;

	public bool IsDownBlocked;

	public bool IsLeftBlocked;

	public bool IsRightBlocked;

	private bool skipFreezingByController;

	private States state;

	private float recoilTimeRemaining;

	private float recoilSpeed;

	private Sweep recoilSweep;

	private bool isRecoilSweeping;

	private float previousRecoilAngle;

	private List<IRecoilMultiplier> recoilMultipliers;

	private Rigidbody2D body;

	private Collider2D bodyCollider;

	private const int SweepLayerMask = 256;

	public float RecoilSpeedBase => recoilSpeedBase;

	public bool SkipFreezingByController
	{
		get
		{
			return skipFreezingByController;
		}
		set
		{
			skipFreezingByController = value;
		}
	}

	public bool IsRecoiling
	{
		get
		{
			if (state != States.Recoiling)
			{
				return state == States.Frozen;
			}
			return true;
		}
	}

	private float RecoilMultiplier
	{
		get
		{
			if (recoilMultipliers == null)
			{
				return 1f;
			}
			float num = 1f;
			foreach (IRecoilMultiplier recoilMultiplier in recoilMultipliers)
			{
				num *= recoilMultiplier.GetRecoilMultiplier();
			}
			return num;
		}
	}

	public event FreezeEvent OnHandleFreeze;

	public event CancelRecoilEvent OnCancelRecoil;

	protected void Reset()
	{
		FreezeInPlace = false;
		recoilDuration = 0.5f;
		recoilSpeedBase = 15f;
	}

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		bodyCollider = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		CancelRecoil();
	}

	public void RecoilByHealthManagerFSMParameters()
	{
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "health_manager_enemy");
		int cardinalDirection = DirectionUtils.GetCardinalDirection(playMakerFSM.FsmVariables.GetFsmFloat("Attack Direction").Value);
		_ = playMakerFSM.FsmVariables.GetFsmInt("Attack Type").Value;
		float value = playMakerFSM.FsmVariables.GetFsmFloat("Attack Magnitude").Value;
		RecoilByDirection(cardinalDirection, value);
	}

	public void RecoilByDamage(HitInstance damageInstance)
	{
		int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.Direction);
		RecoilByDirection(cardinalDirection, damageInstance.MagnitudeMultiplier);
	}

	public void RecoilDirectly(int attackDirection)
	{
		RecoilByDirection(attackDirection, 1f);
	}

	public void RecoilByDirection(int attackDirection, float attackMagnitude)
	{
		float num = recoilSpeedBase * attackMagnitude * RecoilMultiplier;
		if (state != 0)
		{
			float num2 = recoilTimeRemaining / recoilDuration;
			float num3 = recoilSpeed * num2;
			if (num < num3)
			{
				return;
			}
		}
		if (FreezeInPlace)
		{
			Freeze();
		}
		else if ((attackDirection != 1 || !IsUpBlocked) && (attackDirection != 3 || !IsDownBlocked) && (attackDirection != 2 || !IsLeftBlocked) && (attackDirection != 0 || !IsRightBlocked))
		{
			switch (attackDirection)
			{
			default:
				return;
			case 2:
				FSMUtility.SendEventToGameObject(base.gameObject, "RECOIL HORIZONTAL");
				FSMUtility.SendEventToGameObject(base.gameObject, "HIT LEFT");
				previousRecoilAngle = 180f;
				break;
			case 0:
				FSMUtility.SendEventToGameObject(base.gameObject, "RECOIL HORIZONTAL");
				FSMUtility.SendEventToGameObject(base.gameObject, "HIT RIGHT");
				previousRecoilAngle = 0f;
				break;
			case 3:
				FSMUtility.SendEventToGameObject(base.gameObject, "HIT DOWN");
				previousRecoilAngle = 270f;
				break;
			case 1:
				FSMUtility.SendEventToGameObject(base.gameObject, "HIT UP");
				previousRecoilAngle = 90f;
				break;
			}
			FSMUtility.SendEventToGameObject(base.gameObject, "RECOIL");
			if (bodyCollider == null)
			{
				bodyCollider = GetComponent<Collider2D>();
			}
			state = States.Recoiling;
			recoilSpeed = num;
			recoilSweep = new Sweep(bodyCollider, attackDirection, 3);
			isRecoilSweeping = true;
			recoilTimeRemaining = recoilDuration;
			UpdatePhysics(0f);
		}
	}

	public void CancelRecoil()
	{
		if (state != 0)
		{
			state = States.Ready;
			if (this.OnCancelRecoil != null)
			{
				this.OnCancelRecoil();
			}
			FSMUtility.SendEventToGameObject(base.gameObject, "RECOIL END");
		}
	}

	private void Freeze()
	{
		if (skipFreezingByController)
		{
			if (this.OnHandleFreeze != null)
			{
				this.OnHandleFreeze();
			}
			state = States.Ready;
			return;
		}
		state = States.Frozen;
		if (body != null)
		{
			body.linearVelocity = Vector2.zero;
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "FREEZE IN PLACE");
		recoilTimeRemaining = recoilDuration;
		UpdatePhysics(0f);
	}

	protected void FixedUpdate()
	{
		UpdatePhysics(Time.fixedDeltaTime);
	}

	private void UpdatePhysics(float deltaTime)
	{
		if (state == States.Frozen)
		{
			if (body != null)
			{
				body.linearVelocity = Vector2.zero;
			}
			recoilTimeRemaining -= deltaTime;
			if (recoilTimeRemaining <= 0f)
			{
				CancelRecoil();
			}
		}
		else
		{
			if (state != States.Recoiling)
			{
				return;
			}
			if (isRecoilSweeping)
			{
				if (recoilSweep.Check(recoilSpeed * deltaTime, 256, out var clippedDistance))
				{
					isRecoilSweeping = false;
				}
				if (clippedDistance > Mathf.Epsilon)
				{
					Vector2 vector = recoilSweep.Direction * clippedDistance;
					if ((bool)body)
					{
						body.position += vector;
					}
					else
					{
						base.transform.Translate(vector, Space.World);
					}
				}
			}
			recoilTimeRemaining -= deltaTime;
			if (recoilTimeRemaining <= 0f)
			{
				CancelRecoil();
			}
		}
	}

	public void SetRecoilSpeed(float newSpeed)
	{
		recoilSpeedBase = newSpeed;
	}

	public bool GetIsRecoiling()
	{
		return state == States.Recoiling;
	}

	public float GetPreviousRecoilAngle()
	{
		return previousRecoilAngle;
	}

	public void AddRecoilMultiplier(IRecoilMultiplier multiplier)
	{
		if (recoilMultipliers == null)
		{
			recoilMultipliers = new List<IRecoilMultiplier>();
		}
		recoilMultipliers.Add(multiplier);
	}

	public void RemoveRecoilMultiplier(IRecoilMultiplier multiplier)
	{
		recoilMultipliers?.Remove(multiplier);
	}
}
