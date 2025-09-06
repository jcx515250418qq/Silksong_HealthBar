using UnityEngine;

public class SendFSMEventOnEntry : MonoBehaviour
{
	public PlayMakerFSM fsm;

	public string fsmEvent;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		fsm.SendEvent(fsmEvent);
	}
}
