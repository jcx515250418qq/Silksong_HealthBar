using UnityEngine;

public class TimedTicker : EventBase
{
	[SerializeField]
	private TimedTicker parentTicker;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("parentTicker", false, false, true)]
	private float tickDelay;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("parentTicker", true, false, true)]
	private int tickMisses;

	[SerializeField]
	private bool tickOnStart;

	[Space]
	[SerializeField]
	private string sendEventToRegister;

	private float timeElapsed;

	private int ticksMissed;

	public override string InspectorInfo => $"{TickDelay} seconds";

	public float TickDelay
	{
		get
		{
			if (!parentTicker)
			{
				return tickDelay;
			}
			return parentTicker.TickDelay * (float)(1 + tickMisses);
		}
	}

	private void OnEnable()
	{
		timeElapsed = 0f;
		ticksMissed = 0;
		if ((bool)parentTicker)
		{
			parentTicker.ReceivedEvent += UpdateTickParent;
		}
	}

	private void Start()
	{
		if (tickOnStart)
		{
			Tick();
		}
	}

	private void OnDisable()
	{
		if ((bool)parentTicker)
		{
			parentTicker.ReceivedEvent -= UpdateTickParent;
		}
	}

	private void Update()
	{
		if (!parentTicker)
		{
			UpdateTickTimed();
		}
	}

	private void UpdateTickTimed()
	{
		float num = TickDelay;
		if (timeElapsed > num)
		{
			timeElapsed %= num;
			Tick();
		}
		timeElapsed += Time.deltaTime;
	}

	private void UpdateTickParent()
	{
		if (ticksMissed >= tickMisses)
		{
			ticksMissed = 0;
			Tick();
		}
		else
		{
			ticksMissed++;
		}
	}

	private void Tick()
	{
		if (!string.IsNullOrEmpty(sendEventToRegister))
		{
			EventRegister.SendEvent(sendEventToRegister);
		}
		CallReceivedEvent();
	}
}
