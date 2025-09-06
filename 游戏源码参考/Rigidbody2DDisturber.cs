using System.Collections;
using UnityEngine;

public class Rigidbody2DDisturber : Rigidbody2DDisturberBase
{
	[SerializeField]
	private Vector2 force;

	[SerializeField]
	private float frequency = 60f;

	private Coroutine rumbleRoutine;

	public void StartRumble()
	{
		StopRumble();
		rumbleRoutine = StartCoroutine(Rumble());
	}

	public void StopRumble()
	{
		if (rumbleRoutine != null)
		{
			StopCoroutine(rumbleRoutine);
			rumbleRoutine = null;
		}
	}

	private IEnumerator Rumble()
	{
		WaitForSeconds wait = new WaitForSeconds(1f / frequency);
		while (true)
		{
			Rigidbody2D[] array = bodies;
			foreach (Rigidbody2D obj in array)
			{
				Vector2 vector = new Vector2(Random.Range(0f - force.x, force.x), Random.Range(0f - force.y, force.y));
				obj.AddForce(vector, ForceMode2D.Impulse);
			}
			yield return wait;
		}
	}
}
