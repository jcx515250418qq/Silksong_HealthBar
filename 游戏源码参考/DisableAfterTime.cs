using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
	[SerializeField]
	private float waitTime = 5f;

	[SerializeField]
	private bool isRealtime;

	[SerializeField]
	private string sendEvent;

	private float timeLeft;

	private void OnEnable()
	{
		timeLeft = waitTime;
	}

	private void OnDisable()
	{
		GameObject gameObject = base.gameObject;
		if ((object)gameObject != null && !gameObject.activeInHierarchy && gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		timeLeft -= (isRealtime ? Time.unscaledDeltaTime : Time.deltaTime);
		if (!(timeLeft > 0f))
		{
			if (!string.IsNullOrEmpty(sendEvent))
			{
				FSMUtility.SendEventToGameObject(base.gameObject, sendEvent);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
