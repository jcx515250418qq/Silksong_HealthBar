using InControl;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("InControl")]
	[Tooltip("Gets the pressed state of the specified InControl Button for a given Device and stores it in a Bool Variable.")]
	public class GetInControlDeviceInputButton : FsmStateAction
	{
		[Tooltip("The index of the device.")]
		public FsmInt deviceIndex;

		public InputControlType axis;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private InputDevice _inputDevice;

		public override void Reset()
		{
			deviceIndex = null;
			axis = InputControlType.Action1;
			storeResult = null;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			if (deviceIndex.Value == -1)
			{
				_inputDevice = InputManager.ActiveDevice;
			}
			else
			{
				_inputDevice = InputManager.Devices[deviceIndex.Value];
			}
			DoGetButton();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetButton();
		}

		private void DoGetButton()
		{
			storeResult.Value = _inputDevice.GetControl(axis).IsPressed;
		}
	}
}
