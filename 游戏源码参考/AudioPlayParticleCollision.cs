using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public class AudioPlayParticleCollision : MonoBehaviour
{
	[SerializeField]
	private RandomAudioClipTable table;

	[SerializeField]
	private AudioEventRandom clips;

	[SerializeField]
	private AudioSource sourcePrefabOverride;

	[Space]
	[SerializeField]
	private float alwaysPlayTime;

	[SerializeField]
	private float frequencyLimit;

	[SerializeField]
	[Range(0f, 1f)]
	private float playChance = 1f;

	[SerializeField]
	private MinMaxFloat collisionSpeedRange = new MinMaxFloat(2f, 10f);

	[SerializeField]
	private AnimationCurve speedVolumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private double alwaysPlayTimer;

	private double limitTimer;

	private ParticleSystem system;

	private readonly List<ParticleCollisionEvent> particleCollisions = new List<ParticleCollisionEvent>();

	private void Awake()
	{
		system = GetComponent<ParticleSystem>();
		if ((bool)system)
		{
			ParticleSystem.CollisionModule collision = system.collision;
			collision.sendCollisionMessages = true;
		}
	}

	public void OnParticleCollision(GameObject other)
	{
		particleCollisions.Clear();
		system.GetCollisionEvents(other, particleCollisions);
		if (particleCollisions.Count == 0)
		{
			return;
		}
		float num = 0f;
		ParticleCollisionEvent particleCollisionEvent = particleCollisions[0];
		bool flag = false;
		foreach (ParticleCollisionEvent particleCollision in particleCollisions)
		{
			float sqrMagnitude = particleCollision.velocity.sqrMagnitude;
			if (!float.IsNaN(sqrMagnitude) && !float.IsNaN(particleCollision.intersection.sqrMagnitude) && !(sqrMagnitude <= num))
			{
				num = sqrMagnitude;
				particleCollisionEvent = particleCollision;
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		num = Mathf.Sqrt(num);
		float num2 = ((Math.Abs(collisionSpeedRange.Start - collisionSpeedRange.End) <= Mathf.Epsilon) ? 1f : collisionSpeedRange.GetTBetween(num));
		if (num2 <= Mathf.Epsilon)
		{
			return;
		}
		float num3 = speedVolumeCurve.Evaluate(num2);
		if (!(num3 <= Mathf.Epsilon))
		{
			double timeAsDouble = Time.timeAsDouble;
			if (!(timeAsDouble < alwaysPlayTimer) || (!(timeAsDouble < limitTimer) && !(UnityEngine.Random.Range(0f, 1f) > playChance)))
			{
				alwaysPlayTimer = timeAsDouble + (double)alwaysPlayTime;
				limitTimer = timeAsDouble + (double)frequencyLimit;
				Vector3 intersection = particleCollisionEvent.intersection;
				float? z = base.transform.position.z;
				Vector3 position = intersection.Where(null, null, z);
				AudioSource prefab = (sourcePrefabOverride ? sourcePrefabOverride : Audio.DefaultAudioSourcePrefab);
				clips.SpawnAndPlayOneShot(prefab, position, num3);
				table.SpawnAndPlayOneShot(prefab, position, forcePlay: false, num3);
			}
		}
	}

	public Vector3 FixNaN(Vector3 v)
	{
		bool num = float.IsNaN(v.x);
		bool flag = float.IsNaN(v.y);
		bool flag2 = float.IsNaN(v.z);
		_ = num || flag || flag2;
		return new Vector3(num ? 0f : v.x, flag ? 0f : v.y, flag2 ? 0f : v.z);
	}
}
