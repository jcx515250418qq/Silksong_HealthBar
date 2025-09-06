using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class FlockFlyer : MonoBehaviour
{
	[Serializable]
	private struct Appearance
	{
		public tk2dSpriteAnimation AnimLib;

		public GameObject CorpsePrefab;

		public Vector2 Offset;
	}

	[Serializable]
	private class ProbabilityAppearance : Probability.ProbabilityBase<Appearance>
	{
		public Appearance Appearance;

		public override Appearance Item => Appearance;
	}

	[SerializeField]
	private ProbabilityAppearance[] appearances;

	[SerializeField]
	[Range(0f, 1f)]
	private float activeProbability;

	[Space]
	[SerializeField]
	private TriggerEnterEvent heroTrigger;

	[SerializeField]
	private TriggerEnterEvent enemyTrigger;

	[SerializeField]
	private TrackTriggerObjects wakeOthersCollider;

	[SerializeField]
	private ParticleSystem takeOffParticles;

	[Space]
	[SerializeField]
	private AudioEventRandom alertSound;

	[SerializeField]
	private AudioEventRandom flyAwayStartSound;

	[Space]
	[SerializeField]
	private Vector3 minScale = Vector3.one;

	[SerializeField]
	private Vector3 maxScale = Vector3.one;

	[SerializeField]
	private MinMaxFloat fleeRiseForce;

	[SerializeField]
	private MinMaxFloat fleeSideForce;

	[SerializeField]
	private float fleeDisableTime;

	[SerializeField]
	private MinMaxFloat fleeReactionTime;

	private readonly List<FlockFlyer> wakeOthers = new List<FlockFlyer>();

	private bool isSinging;

	private bool isFleeing;

	private Vector2 force;

	private tk2dSpriteAnimator animator;

	private HeroPerformanceSingReaction singReaction;

	private Rigidbody2D body;

	private EnemyDeathEffects deathEffects;

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		body = GetComponent<Rigidbody2D>();
		Appearance randomItemByProbability = Probability.GetRandomItemByProbability<ProbabilityAppearance, Appearance>(appearances);
		if ((bool)randomItemByProbability.AnimLib)
		{
			animator.Library = randomItemByProbability.AnimLib;
		}
		base.transform.Translate(randomItemByProbability.Offset, Space.Self);
		deathEffects = GetComponent<EnemyDeathEffects>();
		if ((bool)deathEffects)
		{
			deathEffects.CorpsePrefab = randomItemByProbability.CorpsePrefab;
			deathEffects.PreInstantiate();
		}
		if ((bool)heroTrigger)
		{
			heroTrigger.OnTriggerEntered += OnHeroTriggerEntered;
		}
		if ((bool)enemyTrigger)
		{
			enemyTrigger.OnTriggerEntered += OnEnemyTriggerEntered;
		}
		singReaction = GetComponent<HeroPerformanceSingReaction>();
		if ((bool)singReaction)
		{
			singReaction.OnSingStarted.AddListener(StartSing);
			singReaction.OnSingEnded.AddListener(EndSing);
			singReaction.OnStartleEnded.AddListener(Flee);
		}
	}

	private void OnEnable()
	{
		if (UnityEngine.Random.Range(0f, 1f) > activeProbability)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		float num = ((UnityEngine.Random.Range(0f, 1f) > 0.5f) ? 1 : (-1));
		Vector3 localScale = Vector3.Lerp(minScale, maxScale, UnityEngine.Random.Range(0f, 1f));
		localScale.x = Mathf.Abs(localScale.x) * num;
		base.transform.localScale = localScale;
		if ((bool)wakeOthersCollider)
		{
			foreach (GameObject insideGameObject in wakeOthersCollider.InsideGameObjects)
			{
				FlockFlyer component = insideGameObject.GetComponent<FlockFlyer>();
				if ((bool)component && component != this)
				{
					wakeOthers.Add(component);
				}
			}
		}
		body.linearVelocity = Vector2.zero;
		force = Vector2.zero;
		animator.Play("Idle");
		Collider2D component2 = GetComponent<Collider2D>();
		if ((bool)component2)
		{
			component2.enabled = base.transform.IsOnHeroPlane();
		}
	}

	private void OnDisable()
	{
		wakeOthers.Clear();
		isFleeing = false;
		StopAllCoroutines();
	}

	private void FixedUpdate()
	{
		if (isFleeing)
		{
			body.AddForce(force);
		}
	}

	public void StartSing()
	{
		isSinging = true;
		if ((bool)singReaction && singReaction.IsForcedAny)
		{
			return;
		}
		foreach (FlockFlyer wakeOther in wakeOthers)
		{
			if ((bool)wakeOther)
			{
				wakeOther.StartSingForced();
			}
		}
	}

	public void EndSing()
	{
		isSinging = false;
		if ((bool)singReaction && singReaction.IsForcedAny)
		{
			return;
		}
		Flee();
		foreach (FlockFlyer wakeOther in wakeOthers)
		{
			if ((bool)wakeOther)
			{
				wakeOther.EndSingForced();
			}
		}
	}

	private void StartSingForced()
	{
		if ((bool)singReaction)
		{
			singReaction.IsForcedSoft = true;
		}
	}

	private void EndSingForced()
	{
		if ((bool)singReaction)
		{
			singReaction.IsForcedSoft = false;
		}
	}

	private void OnHeroTriggerEntered(Collider2D col, GameObject sender)
	{
		Flee();
	}

	private void OnEnemyTriggerEntered(Collider2D col, GameObject sender)
	{
		if (!col.GetComponent<FlockFlyer>())
		{
			Flee();
		}
	}

	private void Flee()
	{
		if (!isFleeing && base.isActiveAndEnabled)
		{
			isFleeing = true;
			StartCoroutine(FlyAway());
		}
	}

	private IEnumerator FlyAway()
	{
		if ((bool)singReaction)
		{
			while (isSinging)
			{
				yield return null;
			}
			if (singReaction.enabled)
			{
				singReaction.enabled = false;
			}
		}
		yield return new WaitForSeconds(fleeReactionTime.GetRandomValue());
		foreach (FlockFlyer wakeOther in wakeOthers)
		{
			if ((bool)wakeOther)
			{
				wakeOther.Flee();
			}
		}
		Transform transform = base.transform;
		Vector3 pos = transform.position;
		HeroController instance = HeroController.instance;
		Vector3 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x) * Mathf.Sign(pos.x - instance.transform.position.x);
		transform.localScale = scale;
		alertSound.SpawnAndPlayOneShot(pos);
		yield return StartCoroutine(animator.PlayAnimWait("Fly Antic"));
		flyAwayStartSound.SpawnAndPlayOneShot(pos);
		takeOffParticles.Play();
		force = new Vector2(fleeSideForce.GetRandomValue(), fleeRiseForce.GetRandomValue());
		force.x *= Mathf.Sign(scale.x);
		animator.Play("Fly");
		yield return new WaitForSeconds(fleeDisableTime);
		base.gameObject.SetActive(value: false);
	}
}
