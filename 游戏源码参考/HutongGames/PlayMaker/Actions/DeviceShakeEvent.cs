namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends an Event when the mobile device is shaken.")]
	public class DeviceShakeEvent : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Amount of acceleration required to trigger the event. Higher numbers require a harder shake.")]
		public FsmFloat shakeThreshold;

		[RequiredField]
		[Tooltip("Event to send when Shake Threshold is exceeded.")]
		public FsmEvent sendEvent;

		public override void Reset()
		{
			shakeThreshold = 3f;
			sendEvent = null;
		}

		public override void OnUpdate()
		{
			if (ActionHelpers.GetDeviceAcceleration().sqrMagnitude > shakeThreshold.Value * shakeThreshold.Value)
			{
				base.Fsm.Event(sendEvent);
			}
		}
	}
}
