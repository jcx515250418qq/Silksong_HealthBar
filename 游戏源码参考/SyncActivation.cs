using UnityEngine;

public class SyncActivation : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject target;

	[SerializeField]
	private bool inverted;

	private void OnEnable()
	{
		target.SetActive(!inverted);
	}

	private void OnDisable()
	{
		target.SetActive(inverted);
	}
}
