namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Causes the device to vibrate for half a second.\nNOTE: Unity's built in vibrate function is fairly limited. However there are alternatives available on the Asset Store...")]
	public class DeviceVibrate : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
