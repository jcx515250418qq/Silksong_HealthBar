using UnityEngine;

public class SendEventToRegisterMethod : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;

	public void DoSend()
	{
		EventRegister.SendEvent(sendEvent);
	}
}
