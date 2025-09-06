using System;
using UnityEngine;

[Serializable]
public class FlingGameObject
{
	public GameObject prefab;

	[Space]
	public int spawnMin = 5;

	public int spawnMax = 10;

	[Space]
	public float speedMin = 10f;

	public float speedMax = 20f;

	[Space]
	public float angleMin = -45f;

	public float angleMax = 45f;

	public void Fling(Vector3 spawnPosition, float velocityMultiplier)
	{
		Fling(spawnPosition, Quaternion.identity, velocityMultiplier);
	}

	public void Fling(Vector3 spawnPosition, Quaternion spawnRotation, float velocityMultiplier)
	{
		int num = UnityEngine.Random.Range(spawnMin, spawnMax + 1);
		Vector2 linearVelocity = default(Vector2);
		for (int i = 1; i <= num; i++)
		{
			GameObject gameObject = prefab.Spawn(spawnPosition, spawnRotation);
			float num2 = UnityEngine.Random.Range(speedMin, speedMax);
			float num3 = UnityEngine.Random.Range(angleMin, angleMax);
			linearVelocity.x = num2 * Mathf.Cos(num3 * (MathF.PI / 180f));
			linearVelocity.y = num2 * Mathf.Sin(num3 * (MathF.PI / 180f));
			linearVelocity *= Mathf.Abs(velocityMultiplier);
			Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				component.linearVelocity = linearVelocity;
			}
		}
	}
}
