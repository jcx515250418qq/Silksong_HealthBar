using System;

public abstract class EventBase : DebugDrawColliderRuntimeAdder
{
	public abstract string InspectorInfo { get; }

	public event Action ReceivedEvent;

	protected void CallReceivedEvent()
	{
		if (this.ReceivedEvent != null)
		{
			this.ReceivedEvent();
		}
	}

	public override void AddDebugDrawComponent()
	{
	}
}
