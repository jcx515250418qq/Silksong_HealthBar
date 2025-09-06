using UnityEngine;

public class WaterDetector : MonoBehaviour
{
	private WaterPhysics water;

	private BoxCollider2D collider;

	private void Awake()
	{
		collider = GetComponent<BoxCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Rigidbody2D component = collision.GetComponent<Rigidbody2D>();
		if ((bool)component)
		{
			Splash(component.linearVelocity.y * component.mass / 40f);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Rigidbody2D component = collision.GetComponent<Rigidbody2D>();
		if ((bool)component)
		{
			Splash(component.linearVelocity.y * component.mass / 40f);
		}
	}

	public void Splash(float force)
	{
		if (!water)
		{
			water = base.transform.parent.GetComponent<WaterPhysics>();
		}
		water.Splash(base.transform.position.x, force);
	}
}
