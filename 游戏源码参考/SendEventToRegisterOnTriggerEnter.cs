using UnityEngine;

public class SendEventToRegisterOnTriggerEnter : MonoBehaviour
{
	[SerializeField]
	private LayerMask allowLayers = -1;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		int layer = collision.gameObject.layer;
		int num = 1 << layer;
		if ((allowLayers.value & num) == num)
		{
			EventRegister.SendEvent(sendEvent);
		}
	}
}
