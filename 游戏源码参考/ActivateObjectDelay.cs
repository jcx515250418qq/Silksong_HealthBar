using UnityEngine;

public class ActivateObjectDelay : MonoBehaviour
{
	[SerializeField]
	private GameObject objectToActivate;

	[SerializeField]
	private float time;

	private float timer;

	private bool didActivation;

	private void OnEnable()
	{
		timer = time;
		didActivation = false;
		objectToActivate.SetActive(value: false);
	}

	private void Update()
	{
		if (!didActivation)
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
				return;
			}
			objectToActivate.SetActive(value: true);
			didActivation = true;
		}
	}

	public void Cancel()
	{
		didActivation = true;
	}
}
