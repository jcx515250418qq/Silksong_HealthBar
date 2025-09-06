using UnityEngine;

public class ConveyorSpeedZone : MonoBehaviour
{
	public float speed;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		HeroController component = collision.GetComponent<HeroController>();
		if ((bool)component)
		{
			component.SetConveyorSpeed(speed);
		}
	}
}
