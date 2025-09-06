using System;

namespace HutongGames.PlayMaker.Ecosystem.Utils
{
	[Serializable]
	public class PlayMakerEvent
	{
		public string eventName;

		public bool allowLocalEvents;

		public string defaultEventName;

		public PlayMakerEvent()
		{
		}

		public PlayMakerEvent(string defaultEventName)
		{
			this.defaultEventName = defaultEventName;
			eventName = defaultEventName;
		}

		public bool SendEvent(PlayMakerFSM fromFsm, PlayMakerEventTarget eventTarget)
		{
			if (eventTarget.eventTarget == ProxyEventTarget.BroadCastAll)
			{
				PlayMakerFSM.BroadcastEvent(eventName);
			}
			else if (eventTarget.eventTarget == ProxyEventTarget.Owner || eventTarget.eventTarget == ProxyEventTarget.GameObject)
			{
				PlayMakerUtils.SendEventToGameObject(fromFsm, eventTarget.gameObject, eventName, eventTarget.includeChildren);
			}
			else if (eventTarget.eventTarget == ProxyEventTarget.FsmComponent)
			{
				eventTarget.fsmComponent.SendEvent(eventName);
			}
			return true;
		}
	}
}
