using UnityEngine;

public class FSMBreakOnContact : MonoBehaviour, IBreakOnContact
{
	[SerializeField]
	private PlayMakerFSM fsm;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequired")]
	private string breakEvent = "BREAK";

	private void OnValidate()
	{
		if (fsm == null)
		{
			fsm = GetComponent<PlayMakerFSM>();
		}
	}

	public void Break()
	{
		if ((bool)fsm)
		{
			fsm.SendEvent(breakEvent);
		}
	}

	private bool? IsFsmEventValidRequired(string eventName)
	{
		return fsm.IsEventValid(eventName, isRequired: true);
	}
}
