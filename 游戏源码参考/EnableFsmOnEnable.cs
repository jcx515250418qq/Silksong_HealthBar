using UnityEngine;

public class EnableFsmOnEnable : MonoBehaviour
{
	[SerializeField]
	private PlayMakerFSM fsm;

	private void Reset()
	{
		fsm = GetComponent<PlayMakerFSM>();
	}

	private void OnEnable()
	{
		fsm.enabled = true;
	}
}
