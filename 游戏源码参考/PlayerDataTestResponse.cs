using UnityEngine;
using UnityEngine.Events;

public class PlayerDataTestResponse : MonoBehaviour
{
	private enum RunOnTypes
	{
		StartThenOnEnable = 0,
		JustStart = 1,
		OnEnable = 2,
		Awake = 3
	}

	[SerializeField]
	private PlayerDataTest test;

	[Space]
	[SerializeField]
	private RunOnTypes runOn;

	[Space]
	public UnityEvent IsFullfilled;

	[Space]
	public UnityEvent IsNotFulfilled;

	private bool hasStarted;

	private void Awake()
	{
		if (runOn == RunOnTypes.Awake)
		{
			Evaluate();
		}
	}

	private void Start()
	{
		if (runOn == RunOnTypes.JustStart || runOn == RunOnTypes.StartThenOnEnable)
		{
			hasStarted = true;
			Evaluate();
		}
	}

	private void OnEnable()
	{
		if (runOn == RunOnTypes.StartThenOnEnable)
		{
			if (!hasStarted)
			{
				return;
			}
		}
		else if (runOn != RunOnTypes.OnEnable)
		{
			return;
		}
		Evaluate();
	}

	private void Evaluate()
	{
		if (test.IsFulfilled)
		{
			IsFullfilled?.Invoke();
		}
		else
		{
			IsNotFulfilled?.Invoke();
		}
	}
}
