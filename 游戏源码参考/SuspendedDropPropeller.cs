using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SuspendedDropPropeller : SuspendedPlatformBase
{
	[SerializeField]
	private float gravity;

	[SerializeField]
	private float maxSpeed;

	[SerializeField]
	private Transform platDrop;

	[SerializeField]
	private Transform dropTarget;

	[SerializeField]
	private PlayMakerFSM updraft;

	[SerializeField]
	private float propellerStartDelay;

	[SerializeField]
	private GameObject inactivePropeller;

	[SerializeField]
	private GameObject activePropeller;

	[SerializeField]
	private CameraShakeTarget impactShake;

	[Space]
	public UnityEvent OnDropStart;

	public UnityEvent OnDropImpact;

	public UnityEvent OnStartedActivated;

	private Coroutine dropRoutine;

	protected override void Awake()
	{
		base.Awake();
		if ((bool)inactivePropeller)
		{
			inactivePropeller.SetActive(value: true);
		}
		if ((bool)activePropeller)
		{
			activePropeller.SetActive(value: false);
		}
		dropTarget.gameObject.SetActive(value: false);
	}

	public override void CutDown()
	{
		base.CutDown();
		StopDrop();
		dropRoutine = StartCoroutine(DropDown());
	}

	protected override void OnStartActivated()
	{
		base.OnStartActivated();
		StopDrop();
		platDrop.SetPosition2D(dropTarget.position);
		if ((bool)inactivePropeller)
		{
			inactivePropeller.SetActive(value: false);
		}
		if ((bool)activePropeller)
		{
			activePropeller.SetActive(value: true);
		}
		if ((bool)updraft)
		{
			updraft.SendEvent("ACTIVATE");
		}
		OnStartedActivated?.Invoke();
	}

	private void StopDrop()
	{
		if (dropRoutine != null)
		{
			StopCoroutine(dropRoutine);
			dropRoutine = null;
		}
	}

	private IEnumerator DropDown()
	{
		OnDropStart?.Invoke();
		Vector2 endPos = dropTarget.position;
		float speed = 0f;
		while (platDrop.position.y > endPos.y)
		{
			speed += gravity * Time.deltaTime;
			if (speed > maxSpeed)
			{
				speed = maxSpeed;
			}
			Vector3 position = platDrop.position;
			position.y -= speed * Time.deltaTime;
			platDrop.SetPosition2D(position);
			yield return null;
		}
		platDrop.SetPosition2D(endPos);
		foreach (Transform item in dropTarget)
		{
			item.SetParent(null, worldPositionStays: true);
			item.gameObject.SetActive(value: true);
		}
		impactShake.DoShake(this);
		OnDropImpact?.Invoke();
		yield return new WaitForSeconds(propellerStartDelay);
		if ((bool)inactivePropeller)
		{
			inactivePropeller.SetActive(value: false);
		}
		if ((bool)activePropeller)
		{
			activePropeller.SetActive(value: true);
		}
		if ((bool)updraft)
		{
			updraft.SendEvent("ACTIVATE");
		}
	}
}
