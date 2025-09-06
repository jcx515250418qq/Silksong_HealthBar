using UnityEngine;
using UnityEngine.Events;

public class TrackTriggerResponder : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects trigger;

	[SerializeField]
	private FadeUpWhileIntersecting intersectingTracker;

	[Space]
	public UnityEvent OnEntered;

	public UnityEvent OnExited;

	private bool wasInside;

	private void OnEnable()
	{
		wasInside = trigger.IsInside;
		trigger.InsideStateChanged += OnInsideStateChanged;
	}

	private void OnDisable()
	{
		if ((bool)trigger)
		{
			trigger.InsideStateChanged -= OnInsideStateChanged;
		}
	}

	private void OnInsideStateChanged(bool isInside)
	{
		if (isInside == wasInside)
		{
			return;
		}
		wasInside = isInside;
		if (!intersectingTracker || intersectingTracker.IsTargetIntersecting)
		{
			if (isInside)
			{
				OnEntered.Invoke();
			}
			else
			{
				OnExited.Invoke();
			}
		}
	}
}
