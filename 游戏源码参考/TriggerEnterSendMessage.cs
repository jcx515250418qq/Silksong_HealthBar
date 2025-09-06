using UnityEngine;

public class TriggerEnterSendMessage : MonoBehaviour
{
	public string message = "Acid";

	public SendMessageOptions options = SendMessageOptions.DontRequireReceiver;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("Acid trigger entered");
		collision.gameObject.SendMessage(message, options);
	}
}
