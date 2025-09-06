using UnityEngine;

public class DeactivatePlayerDataTest : MonoBehaviour
{
	[SerializeField]
	private PlayerDataTest test;

	private bool hasStarted;

	private void Start()
	{
		hasStarted = true;
		OnEnable();
	}

	private void OnEnable()
	{
		if (hasStarted && test.IsFulfilled)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
