using System;
using System.Collections;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;

public class CogMultiHitter : MonoBehaviour, ISceneLintUpgrader
{
	[SerializeField]
	private Transform heroGrindEffect;

	[SerializeField]
	private bool useSelfForAngle;

	[SerializeField]
	private GameObject hitEffectPrefab;

	[SerializeField]
	private CameraShakeTarget multiHitRumble;

	[SerializeField]
	private JitterSelf jitter;

	[SerializeField]
	private AudioEvent heroDamageAudio;

	[SerializeField]
	private VectorCurveAnimator cogAnimator;

	private double canDamageTime;

	private Coroutine delayRoutine;

	private HeroController damagingHc;

	private void Awake()
	{
		OnSceneLintUpgrade(doUpgrade: true);
		EventRegister.GetRegisterGuaranteed(base.gameObject, "COG DAMAGE").ReceivedEvent += delegate
		{
			CancelDelay();
			canDamageTime = Time.timeAsDouble + 1.0;
		};
		heroGrindEffect.SetParent(null, worldPositionStays: true);
		Vector3 localScale = heroGrindEffect.localScale;
		localScale.x = Mathf.Abs(localScale.x);
		heroGrindEffect.localScale = localScale;
	}

	private void OnDisable()
	{
		CancelDelay();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (Time.timeAsDouble < canDamageTime || other.gameObject.layer != 20)
		{
			return;
		}
		HeroController componentInParent = other.GetComponentInParent<HeroController>();
		if (!componentInParent.isHeroInPosition || componentInParent.playerData.isInvincible)
		{
			return;
		}
		CancelDelay();
		damagingHc = componentInParent;
		damagingHc.OnHazardRespawn += OnHeroHazardRespawn;
		Vector3 position = componentInParent.transform.position;
		float angleToTarget;
		if (useSelfForAngle)
		{
			angleToTarget = GetAngleToTarget(position, base.transform.position);
		}
		else
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Cog Grind Marker");
			Transform transform = null;
			float num = float.PositiveInfinity;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				if (!(gameObject == base.gameObject))
				{
					Transform transform2 = gameObject.transform;
					float sqrMagnitude = (position - transform2.position).sqrMagnitude;
					if (!(sqrMagnitude > num))
					{
						num = sqrMagnitude;
						transform = transform2;
					}
				}
			}
			angleToTarget = GetAngleToTarget(position, transform ? transform.position : base.transform.position);
		}
		EventRegister.SendEvent(EventRegisterEvents.CogDamage, base.gameObject);
		componentInParent.TakeQuickDamage(1, playEffects: true);
		hitEffectPrefab.Spawn(position);
		EventRegister.SendEvent(EventRegisterEvents.HeroDamaged);
		StaticVariableList.SetValue("Wound Sender Override", base.gameObject);
		FSMUtility.SendEventToGameObject(componentInParent.gameObject, "WOUND START");
		multiHitRumble.DoShake(this, shouldFreeze: false);
		if ((bool)jitter)
		{
			jitter.StartJitter();
		}
		if ((bool)cogAnimator)
		{
			cogAnimator.FreezePosition = true;
		}
		heroGrindEffect.SetPosition2D(position);
		heroGrindEffect.SetRotation2D(angleToTarget);
		heroGrindEffect.gameObject.SetActive(value: true);
		heroDamageAudio.SpawnAndPlayOneShot(position);
		delayRoutine = StartCoroutine(DelayEnd(componentInParent));
	}

	private void OnHeroHazardRespawn()
	{
		if ((bool)cogAnimator)
		{
			cogAnimator.FreezePosition = false;
		}
		CancelDelay();
	}

	private void CancelDelay()
	{
		if ((bool)damagingHc)
		{
			damagingHc.OnHazardRespawn -= OnHeroHazardRespawn;
			damagingHc = null;
		}
		if (delayRoutine != null)
		{
			StopCoroutine(delayRoutine);
			delayRoutine = null;
			multiHitRumble.CancelShake();
			if ((bool)jitter)
			{
				jitter.StopJitter();
			}
			heroGrindEffect.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator DelayEnd(HeroController hc)
	{
		yield return new WaitForSeconds(0.5f);
		multiHitRumble.CancelShake();
		if ((bool)jitter)
		{
			jitter.StopJitter();
		}
		DamageHeroDirectly(hc);
		yield return new WaitForSeconds(0.2f);
		heroGrindEffect.gameObject.SetActive(value: false);
	}

	private float GetAngleToTarget(Vector2 targetPos, Vector2 sourcePos)
	{
		Vector2 vector = targetPos - sourcePos;
		float num;
		for (num = Mathf.Atan2(vector.y, vector.x) * (180f / MathF.PI); num < 0f; num += 360f)
		{
		}
		return num;
	}

	private void DamageHeroDirectly(HeroController hc)
	{
		hc.CancelDownspikeInvulnerability();
		hc.playerData.isInvincible = false;
		hc.cState.parrying = false;
		hc.TakeDamage(base.gameObject, (!(base.transform.position.x > hc.transform.position.x)) ? CollisionSide.left : CollisionSide.right, 1, HazardType.SPIKES);
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Multihitter");
		if (!playMakerFSM)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return "cog_multihitter FSM needs upgrading to CogMultiHitter script";
		}
		FsmBool fsmBool = playMakerFSM.FsmVariables.FindFsmBool("Use Self For Angle");
		if (fsmBool != null)
		{
			useSelfForAngle = fsmBool.Value;
		}
		UnityEngine.Object.DestroyImmediate(playMakerFSM);
		return "cog_multihitter FSM was upgraded to CogMultiHitter script";
	}
}
