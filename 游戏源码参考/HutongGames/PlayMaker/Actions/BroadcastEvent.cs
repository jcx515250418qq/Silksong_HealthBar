using System;

namespace HutongGames.PlayMaker.Actions
{
	[Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event to all FSMs in the scene or to all FSMs on a Game Object. NOTE: This action won't work on the very first frame of the game...")]
	public class BroadcastEvent : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The event to broadcast.")]
		public FsmString broadcastEvent;

		[Tooltip("By default, the event is broadcast to all FSMs in the scene. Optionally you can specify a game object to target. The event will then be broadcast to all FSMs on that game object.")]
		public FsmGameObject gameObject;

		[Tooltip("Broadcast the event to all the Game Object's children too.")]
		public FsmBool sendToChildren;

		[Tooltip("Don't send the event to self.")]
		public FsmBool excludeSelf;

		public override void Reset()
		{
			broadcastEvent = null;
			gameObject = null;
			sendToChildren = false;
			excludeSelf = false;
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(broadcastEvent.Value))
			{
				if (gameObject.Value != null)
				{
					base.Fsm.BroadcastEventToGameObject(gameObject.Value, broadcastEvent.Value, sendToChildren.Value, excludeSelf.Value);
				}
				else
				{
					base.Fsm.BroadcastEvent(broadcastEvent.Value, excludeSelf.Value);
				}
			}
			Finish();
		}
	}
}
