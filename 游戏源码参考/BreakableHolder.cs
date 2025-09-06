using System;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class BreakableHolder : DebugDrawColliderRuntimeAdder, IHitResponder, IBreakerBreakable
{
	private enum HitDirection
	{
		Left = 0,
		Right = 1,
		Down = 2
	}

	[Serializable]
	private struct ObjectFling
	{
		public GameObject Object;

		public MinMaxFloat LeftAngleRange;

		public MinMaxFloat RightAngleRange;

		public MinMaxFloat FlingSpeedRange;
	}

	[SerializeField]
	private PersistentIntItem persistent;

	[Space]
	[SerializeField]
	private int finalPayout;

	[SerializeField]
	private int payoutPerHit;

	[SerializeField]
	private int totalHits;

	private int hitsLeft;

	[SerializeField]
	private float hitCooldown = 0.15f;

	private double lastHitTime;

	private bool isBroken;

	[SerializeField]
	private bool resetHitsOnBreak;

	[SerializeField]
	private float noiseRadius = 3f;

	[SerializeField]
	private Probability.ProbabilityGameObject[] holdingGameObjects;

	[SerializeField]
	private ObjectFling[] debrisParts;

	[SerializeField]
	private Vector3 originOffset;

	[SerializeField]
	private MinMaxFloat rightAngleRange;

	[SerializeField]
	private MinMaxFloat leftAngleRange;

	[SerializeField]
	private float angleOffset;

	[SerializeField]
	private MinMaxFloat flingSpeedRange;

	[Space]
	[SerializeField]
	private bool canBreakFromBreaker = true;

	[SerializeField]
	private Breakable forwardToBreakable;

	[Space]
	[SerializeField]
	private GameObject strikePrefab;

	[SerializeField]
	private GameObject breakPrefab;

	[SerializeField]
	private GameObject hitFlingPrefab;

	[SerializeField]
	private GameObject hitDustPrefab;

	[SerializeField]
	private CameraShakeTarget hitCameraShake;

	[SerializeField]
	private CameraShakeTarget breakCameraShake;

	[SerializeField]
	public bool noHitShake;

	[SerializeField]
	private AudioSource audioPlayerPrefab;

	[SerializeField]
	private AudioEventRandom breakSound;

	[SerializeField]
	private AudioEventRandom hitSound;

	[SerializeField]
	private RandomAudioClipTable hitSoundTable;

	[Space]
	public UnityEvent Break;

	public UnityEvent Broken;

	public UnityEvent HitStarted;

	public UnityEvent HitEnded;

	private GameObject breakEffects;

	public BreakableBreaker.BreakableTypes BreakableType => BreakableBreaker.BreakableTypes.Basic;

	GameObject IBreakerBreakable.gameObject => base.gameObject;

	private void OnEnable()
	{
		ResetHits();
	}

	private void Start()
	{
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out int value)
			{
				value = hitsLeft;
				if ((bool)forwardToBreakable)
				{
					forwardToBreakable.SetHitsToBreak(hitsLeft);
				}
			};
			persistent.OnSetSaveState += delegate(int value)
			{
				hitsLeft = value;
				if (hitsLeft <= 0)
				{
					SetBroken();
					if ((bool)forwardToBreakable)
					{
						forwardToBreakable.SetAlreadyBroken();
					}
				}
			};
		}
		if ((bool)forwardToBreakable)
		{
			forwardToBreakable.SetHitsToBreak(hitsLeft);
			float num = forwardToBreakable.GetHitCoolDown();
			if (num > 0f)
			{
				hitCooldown = ((hitCooldown > 0f) ? Mathf.Min(hitCooldown, num) : num);
				num = Mathf.Min(hitCooldown, num);
			}
			forwardToBreakable.SetHitCoolDownDuration(num);
		}
		if (breakPrefab != null)
		{
			Transform transform = base.transform;
			breakEffects = UnityEngine.Object.Instantiate(breakPrefab, transform.position, transform.rotation);
			breakEffects.SetActive(value: false);
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		return DoHit(damageInstance.AttackType, damageInstance.Direction, damageInstance.Source) ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	private bool DoHit(AttackTypes attackType, float direction, GameObject source)
	{
		if (hitsLeft <= 0)
		{
			return false;
		}
		bool flag;
		if (attackType == AttackTypes.Heavy)
		{
			hitsLeft = 0;
			flag = true;
		}
		else
		{
			if (lastHitTime > Time.timeAsDouble)
			{
				return false;
			}
			lastHitTime = Time.timeAsDouble + (double)hitCooldown;
			hitsLeft--;
			flag = hitsLeft <= 0;
		}
		DoHitWithPayout(flag, direction, source.transform.position.x > base.transform.position.x);
		if (!flag)
		{
			return true;
		}
		SetBroken();
		if ((bool)forwardToBreakable)
		{
			forwardToBreakable.BreakSelf();
		}
		return true;
	}

	private void DoHitWithPayout(bool doBreak, float direction, bool isFromRight)
	{
		if ((bool)strikePrefab)
		{
			strikePrefab.Spawn(base.transform.position);
		}
		FlingHolding(payoutPerHit, isFromRight);
		DoHit(doBreak, direction, isFromRight);
	}

	private void DoHit(bool doBreak, float direction, bool isFromRight)
	{
		if (doBreak)
		{
			breakCameraShake.DoShake(this);
			breakSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			FlingHolding(finalPayout, isFromRight);
			if ((bool)breakEffects)
			{
				breakEffects.transform.position = base.transform.position;
				breakEffects.SetActive(value: true);
			}
			Break.Invoke();
			ObjectFling[] array = debrisParts;
			for (int i = 0; i < array.Length; i++)
			{
				ObjectFling objectFling = array[i];
				if ((bool)objectFling.Object)
				{
					objectFling.Object.SetActive(value: true);
					MinMaxFloat minMaxFloat = (isFromRight ? objectFling.RightAngleRange : objectFling.LeftAngleRange);
					FlingUtils.SelfConfig config = default(FlingUtils.SelfConfig);
					config.Object = objectFling.Object;
					config.SpeedMin = objectFling.FlingSpeedRange.Start;
					config.SpeedMax = objectFling.FlingSpeedRange.End;
					config.AngleMin = minMaxFloat.Start;
					config.AngleMax = minMaxFloat.End;
					FlingUtils.FlingObject(config, base.transform, Vector3.zero);
				}
			}
			return;
		}
		hitCameraShake.DoShake(this);
		if ((bool)hitSoundTable)
		{
			hitSoundTable.SpawnAndPlayOneShot(base.transform.position);
		}
		else
		{
			hitSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		}
		HitDirection hitDirection = HitDirection.Down;
		if (direction < 45f)
		{
			hitDirection = HitDirection.Right;
		}
		else if (direction < 135f)
		{
			hitDirection = HitDirection.Down;
		}
		else if (direction < 225f)
		{
			hitDirection = HitDirection.Left;
		}
		switch (hitDirection)
		{
		case HitDirection.Right:
			if (!base.transform.eulerAngles.z.IsWithinTolerance(10f, 270f))
			{
				DoHitEffects(20f, 40f, new Vector3(0f, 90f, 270f));
			}
			break;
		case HitDirection.Left:
			if (!base.transform.eulerAngles.x.IsWithinTolerance(10f, 90f))
			{
				DoHitEffects(100f, 140f, new Vector3(180f, 90f, 270f));
			}
			break;
		case HitDirection.Down:
			if (!base.transform.eulerAngles.z.IsWithinTolerance(10f, 180f))
			{
				DoHitEffects(70f, 110f, new Vector3(-90f, -180f, -180f));
			}
			break;
		}
		HitStarted.Invoke();
		Vector3 initialPosition = base.transform.position;
		if (!noHitShake)
		{
			this.StartTimerRoutine(0f, 0.2f, delegate(float time)
			{
				Vector3 vector = new Vector3(UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f));
				base.transform.position = Vector3.Lerp(initialPosition + vector, initialPosition, time);
			}, null, delegate
			{
				HitEnded.Invoke();
			});
		}
	}

	private void DoHitEffects(float pAngleMin, float pAngleMax, Vector3 dustRotation)
	{
		if ((bool)hitFlingPrefab)
		{
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = hitFlingPrefab;
			config.AmountMin = 3;
			config.AmountMax = 5;
			config.SpeedMin = 15f;
			config.SpeedMax = 20f;
			config.AngleMin = pAngleMin;
			config.AngleMax = pAngleMax;
			config.OriginVariationX = 0.25f;
			config.OriginVariationY = 0.25f;
			FlingUtils.SpawnAndFling(config, base.transform, Vector3.zero);
		}
		if ((bool)hitDustPrefab)
		{
			hitDustPrefab.Spawn(base.transform.position + new Vector3(0f, 0f, 0.1f), Quaternion.Euler(dustRotation));
		}
	}

	private void SetBroken()
	{
		if (!isBroken)
		{
			isBroken = true;
			Broken.Invoke();
			NoiseMaker.CreateNoise(base.transform.position, noiseRadius, NoiseMaker.Intensities.Normal);
			Collider2D component = GetComponent<Collider2D>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			if (resetHitsOnBreak)
			{
				ResetHits();
			}
		}
	}

	private void ResetHits()
	{
		isBroken = false;
		hitsLeft = totalHits;
	}

	private void FlingHolding(int amount, bool isDirectionRight)
	{
		GameObject randomGameObjectByProbability = Probability.GetRandomGameObjectByProbability(holdingGameObjects);
		if ((bool)randomGameObjectByProbability)
		{
			Vector3 lossyScale = base.transform.lossyScale;
			if (lossyScale.x < 0f)
			{
				isDirectionRight = !isDirectionRight;
			}
			if (lossyScale.y < 0f)
			{
				isDirectionRight = !isDirectionRight;
			}
			MinMaxFloat relativeAngleRange = GetRelativeAngleRange(isDirectionRight ? rightAngleRange : leftAngleRange);
			if (Gameplay.IsShellShardPrefab(randomGameObjectByProbability))
			{
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = randomGameObjectByProbability;
				config.AmountMin = amount;
				config.AmountMax = amount;
				config.SpeedMin = flingSpeedRange.Start;
				config.SpeedMax = flingSpeedRange.End;
				config.AngleMin = relativeAngleRange.Start;
				config.AngleMax = relativeAngleRange.End;
				config.OriginVariationX = 0.25f;
				config.OriginVariationY = 0.25f;
				FlingUtils.SpawnAndFlingShellShards(config, base.transform, originOffset);
			}
			else
			{
				FlingUtils.Config config = default(FlingUtils.Config);
				config.Prefab = randomGameObjectByProbability;
				config.AmountMin = amount;
				config.AmountMax = amount;
				config.SpeedMin = flingSpeedRange.Start;
				config.SpeedMax = flingSpeedRange.End;
				config.AngleMin = relativeAngleRange.Start;
				config.AngleMax = relativeAngleRange.End;
				config.OriginVariationX = 0.25f;
				config.OriginVariationY = 0.25f;
				FlingUtils.SpawnAndFling(config, base.transform, originOffset);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.TransformPoint(originOffset);
		HandleHelper.Draw2DAngle(position, GetRelativeAngleRange(leftAngleRange).Start, GetRelativeAngleRange(leftAngleRange).End, 1f);
		HandleHelper.Draw2DAngle(position, GetRelativeAngleRange(rightAngleRange).Start, GetRelativeAngleRange(rightAngleRange).End, 1f);
	}

	private MinMaxFloat GetRelativeAngleRange(MinMaxFloat angleRange)
	{
		float num = angleOffset * Mathf.Sign(base.transform.localScale.x);
		float num2 = base.transform.eulerAngles.z + num;
		return new MinMaxFloat(angleRange.Start + num2, angleRange.End + num2);
	}

	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		if (canBreakFromBreaker && !isBroken)
		{
			bool flag = breaker.transform.position.x > base.transform.position.x;
			float direction = ((!flag) ? 1 : (-1));
			if ((bool)strikePrefab)
			{
				strikePrefab.Spawn(base.transform.position);
			}
			while (hitsLeft > 0)
			{
				hitsLeft--;
				FlingHolding(payoutPerHit, flag);
			}
			DoHit(doBreak: true, direction, flag);
			SetBroken();
			if ((bool)forwardToBreakable)
			{
				forwardToBreakable.BreakSelf();
			}
		}
	}

	public void HitFromBreaker(BreakableBreaker breaker)
	{
		if (canBreakFromBreaker)
		{
			float direction = ((breaker.transform.position.x > base.transform.position.x) ? 180 : 0);
			DoHit(AttackTypes.Generic, direction, breaker.gameObject);
		}
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject);
	}
}
