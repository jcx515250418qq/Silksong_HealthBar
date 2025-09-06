using UnityEngine;

public class RestBench : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D otherObject)
	{
		HeroController component = otherObject.GetComponent<HeroController>();
		if ((bool)component)
		{
			component.NearBench(isNearBench: true);
		}
	}

	private void OnTriggerExit2D(Collider2D otherObject)
	{
		HeroController component = otherObject.GetComponent<HeroController>();
		if ((bool)component)
		{
			component.NearBench(isNearBench: false);
			ToolItemManager.ShowQueuedReminder();
		}
	}
}
