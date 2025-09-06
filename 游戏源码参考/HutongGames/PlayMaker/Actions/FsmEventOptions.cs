namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets how subsequent events sent in this state are handled.")]
	public class FsmEventOptions : FsmStateAction
	{
		public PlayMakerFSM sendToFsmComponent;

		public FsmGameObject sendToGameObject;

		public FsmString fsmName;

		public FsmBool sendToChildren;

		public FsmBool broadcastToAll;

		public override void Reset()
		{
			sendToFsmComponent = null;
			sendToGameObject = null;
			fsmName = "";
			sendToChildren = false;
			broadcastToAll = false;
		}

		public override void OnUpdate()
		{
		}
	}
}
