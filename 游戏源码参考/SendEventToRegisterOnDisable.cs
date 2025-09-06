using UnityEngine;

public class SendEventToRegisterOnDisable : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;

	private void OnDisable()
	{
		if (!string.IsNullOrEmpty(sendEvent))
		{
			EventRegister.SendEvent(sendEvent);
		}
	}
}
