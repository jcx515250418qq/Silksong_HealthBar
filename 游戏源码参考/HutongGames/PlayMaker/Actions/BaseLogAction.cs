namespace HutongGames.PlayMaker.Actions
{
	public abstract class BaseLogAction : FsmStateAction
	{
		[Tooltip("Also send to the Unity Log.")]
		public bool sendToUnityLog;

		public override void Reset()
		{
			sendToUnityLog = false;
		}
	}
}
