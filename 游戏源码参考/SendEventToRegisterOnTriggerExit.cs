using UnityEngine;

public class SendEventToRegisterOnTriggerExit : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;

	private void OnTriggerExit2D(Collider2D collision)
	{
		EventRegister.SendEvent(sendEvent);
	}
}
