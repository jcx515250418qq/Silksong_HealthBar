using UnityEngine;

public class ActivateObjectRandomDelay : MonoBehaviour
{
	public GameObject objectToActivate;

	public float timeMin;

	public float timeMax;

	private float time;

	private float timer;

	private bool didActivation;

	private void OnEnable()
	{
		time = Random.Range(timeMin, timeMax);
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
}
