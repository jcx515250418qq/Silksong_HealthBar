using UnityEngine;

public class EnemyWakeRegion : MonoBehaviour
{
	public PlayMakerFSM fsm;

	public string enterEvent = "WAKE";

	public string exitEvent = "SLEEP";

	private void OnTriggerEnter2D(Collider2D collision)
	{
		fsm.SendEvent(enterEvent);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		fsm.SendEvent(exitEvent);
	}
}
