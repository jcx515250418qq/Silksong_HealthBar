using UnityEngine;

public class CorpseBitEnd : MonoBehaviour
{
	public float timer;

	private bool stopped;

	private void Update()
	{
		if (timer <= 0f && !stopped)
		{
			Rigidbody2D component = GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				component.isKinematic = true;
			}
			component.linearVelocity = new Vector2(0f, 0f);
			component.angularVelocity = 0f;
			GetComponent<ObjectBounce>().StopBounce();
			GetComponent<PolygonCollider2D>().enabled = false;
			stopped = true;
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}
}
