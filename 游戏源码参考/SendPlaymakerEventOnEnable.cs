using UnityEngine;

public class SendPlaymakerEventOnEnable : MonoBehaviour
{
	public string eventName = "";

	private void OnEnable()
	{
		if (eventName != "")
		{
			PlayMakerFSM.BroadcastEvent(eventName);
		}
	}
}
