using System;
using GlobalSettings;
using UnityEngine;

public static class BloodSpawner
{
	[Serializable]
	public struct Config
	{
		public Vector3 Position;

		public short MinCount;

		public short MaxCount;

		public float MinSpeed;

		public float MaxSpeed;

		public float AngleMin;

		public float AngleMax;

		public float SizeMultiplier;
	}

	[Serializable]
	public struct GeneralConfig
	{
		public Color Colour;

		public short MinCount;

		public short MaxCount;

		public float MinSpeed;

		public float MaxSpeed;

		public float AngleMin;

		public float AngleMax;

		public float SizeMultiplier;
	}

	public static GameObject SpawnBlood(Config config, Transform spawnPoint, Color? colorOverride = null)
	{
		return SpawnBlood(config.Position + spawnPoint.position, config.MinCount, config.MaxCount, config.MinSpeed, config.MaxSpeed, config.AngleMin, config.AngleMax, colorOverride, config.SizeMultiplier);
	}

	public static GameObject SpawnBlood(GeneralConfig config, Vector3 position)
	{
		return SpawnBlood(position, config.MinCount, config.MaxCount, config.MinSpeed, config.MaxSpeed, config.AngleMin, config.AngleMax, config.Colour, config.SizeMultiplier);
	}

	public static GameObject SpawnBlood(Vector3 position, short minCount, short maxCount, float minSpeed, float maxSpeed, float angleMin = 0f, float angleMax = 360f, Color? colorOverride = null, float sizeMultiplier = 0f)
	{
		GameObject bloodParticlePrefab = Effects.BloodParticlePrefab;
		if ((bool)bloodParticlePrefab && maxCount > 0)
		{
			GameObject gameObject = bloodParticlePrefab.Spawn();
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			if ((bool)component)
			{
				component.Stop();
				component.emission.SetBursts(new ParticleSystem.Burst[1]
				{
					new ParticleSystem.Burst(0f, minCount, maxCount)
				});
				ParticleSystem.MainModule main = component.main;
				main.maxParticles = Mathf.RoundToInt(maxCount);
				main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed, maxSpeed);
				if (colorOverride.HasValue)
				{
					ParticleSystem.MinMaxGradient initialColor = main.startColor;
					main.startColor = new ParticleSystem.MinMaxGradient(colorOverride.Value);
					RecycleResetHandler.Add(component.gameObject, (Action)delegate
					{
						main.startColor = initialColor;
					});
				}
				if (sizeMultiplier != 0f)
				{
					float initialMultiplier = main.startSizeMultiplier;
					main.startSizeMultiplier *= sizeMultiplier;
					RecycleResetHandler.Add(component.gameObject, (Action)delegate
					{
						main.startSizeMultiplier = initialMultiplier;
					});
				}
				ParticleSystem.ShapeModule shape = component.shape;
				float num2 = (shape.arc = angleMax - angleMin);
				component.transform.SetRotation2D(angleMin);
				component.transform.position = position;
				component.Play();
			}
			return gameObject;
		}
		return null;
	}
}
