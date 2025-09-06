using UnityEngine;
using UnityEngine.Events;

public class TripWire : MonoBehaviour
{
	[SerializeField]
	private TriggerEnterEvent trigger;

	[SerializeField]
	private CameraShakeTarget tripShake;

	[SerializeField]
	private GameObject wire;

	[SerializeField]
	private GameObject wireFlash;

	[SerializeField]
	private GameObject[] activateEffects;

	[SerializeField]
	private CurveRotationAnimation wireCapRotator;

	[Space]
	public UnityEvent OnTriggered;

	private bool isTriggered;

	private void Awake()
	{
		if ((bool)wireFlash)
		{
			wireFlash.SetActive(value: false);
		}
		activateEffects.SetAllActive(value: false);
		if ((bool)trigger)
		{
			trigger.OnTriggerEntered += OnTriggerEntered;
		}
	}

	private void OnTriggerEntered(Collider2D collider, GameObject sender)
	{
		if (isTriggered || (collider.gameObject.layer != 9 && collider.gameObject.layer != 17 && collider.gameObject.layer != 20))
		{
			return;
		}
		isTriggered = true;
		tripShake.DoShake(this);
		if ((bool)wireFlash)
		{
			if ((bool)wire)
			{
				wireFlash.transform.SetPosition2D(wire.transform.position);
				wireFlash.transform.SetRotation2D(wire.transform.eulerAngles.z);
			}
			wireFlash.SetActive(value: true);
		}
		if ((bool)wire)
		{
			wire.SetActive(value: false);
		}
		if ((bool)wireCapRotator)
		{
			wireCapRotator.StartAnimation();
		}
		activateEffects.SetAllActive(value: true);
		OnTriggered.Invoke();
	}
}
