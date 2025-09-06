using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dripper : MonoBehaviour
{
	private const float FLICK_AWAY_SPEED = 20f;

	[SerializeField]
	private GameObject spatterPrefab;

	[SerializeField]
	private ParticleSystem flickOffParticle;

	private bool flickingAway;

	private bool skipFlickAway;

	private List<GameObject> spawnedFlingTracker = new List<GameObject>();

	private Coroutine routine;

	private Rigidbody2D rb;

	public event Action<List<GameObject>> OnSpawned;

	public void StartDripper(Transform target)
	{
		if ((bool)target && (bool)spatterPrefab)
		{
			flickingAway = false;
			base.transform.SetParent(target);
			base.transform.localPosition = new Vector3(0f, -0.5f, 0.001f);
			rb = target.GetComponent<Rigidbody2D>();
			AreaEffectTint component = GetComponent<AreaEffectTint>();
			if ((bool)component)
			{
				component.DoTint();
			}
			routine = StartCoroutine(Behaviour());
		}
	}

	public void StartDripper(GameObject target)
	{
		StartDripper(target.transform);
	}

	private void OnDisable()
	{
		if (routine != null)
		{
			StopCoroutine(routine);
		}
	}

	private void OnEnable()
	{
		skipFlickAway = false;
	}

	private void Update()
	{
		if (rb != null && !flickingAway && Math.Abs(rb.linearVelocity.x) >= 20f)
		{
			if (routine != null)
			{
				StopCoroutine(routine);
			}
			routine = StartCoroutine(FlickAway());
			flickingAway = true;
		}
	}

	private IEnumerator Behaviour()
	{
		yield return new WaitForSeconds(0.04f);
		WaitForSeconds frequency = new WaitForSeconds(0.025f);
		float elapsed = 0f;
		while (elapsed <= 0.7f)
		{
			yield return frequency;
			elapsed += 0.025f;
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = spatterPrefab;
			config.SpeedMin = 0f;
			config.SpeedMax = 1f;
			config.AmountMin = 1;
			config.AmountMax = 1;
			config.AngleMin = 90f;
			config.AngleMax = 90f;
			config.OriginVariationX = 0.5f;
			config.OriginVariationY = 0.8f;
			FlingUtils.SpawnAndFling(config, base.transform, Vector3.zero, spawnedFlingTracker);
			if (this.OnSpawned != null)
			{
				this.OnSpawned(spawnedFlingTracker);
			}
			spawnedFlingTracker.Clear();
		}
		routine = null;
		base.gameObject.Recycle();
	}

	private IEnumerator FlickAway()
	{
		if (!skipFlickAway)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			flickOffParticle.Play();
			float angleMin;
			float angleMax;
			if (base.transform.lossyScale.x > 0f)
			{
				angleMin = 20f;
				angleMax = 60f;
			}
			else
			{
				angleMin = 120f;
				angleMax = 160f;
			}
			FlingUtils.Config config = default(FlingUtils.Config);
			config.Prefab = spatterPrefab;
			config.SpeedMin = 10f;
			config.SpeedMax = 20f;
			config.AmountMin = 4;
			config.AmountMax = 5;
			config.AngleMin = angleMin;
			config.AngleMax = angleMax;
			config.OriginVariationX = 0.5f;
			config.OriginVariationY = 0.8f;
			FlingUtils.SpawnAndFling(config, base.transform, Vector3.zero, spawnedFlingTracker);
			if (this.OnSpawned != null)
			{
				this.OnSpawned(spawnedFlingTracker);
			}
			spawnedFlingTracker.Clear();
		}
		yield return new WaitForSeconds(1f);
		base.gameObject.Recycle();
	}

	public void SkipFlickaway()
	{
		skipFlickAway = true;
	}
}
