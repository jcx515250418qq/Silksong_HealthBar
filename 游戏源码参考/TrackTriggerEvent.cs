using UnityEngine;
using UnityEngine.Events;

public class TrackTriggerEvent : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects triggerObjects;

	public UnityEvent OnInside;

	public UnityEvent OnOutside;

	private bool isInside;

	private void Awake()
	{
		if ((bool)triggerObjects)
		{
			triggerObjects.InsideStateChanged += OnInsideStateChanged;
		}
	}

	private void Start()
	{
		bool flag = true;
		if ((bool)triggerObjects && triggerObjects.IsInside)
		{
			SetInside(isInside: true);
			flag = false;
		}
		if (flag)
		{
			OnOutside.Invoke();
		}
	}

	private void OnInsideStateChanged(bool insideState)
	{
		SetInside(insideState);
	}

	private void SetInside(bool isInside)
	{
		if (this.isInside != isInside)
		{
			this.isInside = isInside;
			if (isInside)
			{
				OnInside.Invoke();
			}
			else
			{
				OnOutside.Invoke();
			}
		}
	}
}
