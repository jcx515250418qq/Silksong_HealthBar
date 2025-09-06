using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public abstract class CustomPlayMakerPhysicsEvent<T> : MonoBehaviour
{
	public sealed class EventResponder
	{
		public FsmStateAction stateAction;

		public Action<T> callback;

		public bool pendingRemoval;

		public bool IsValid
		{
			get
			{
				if (stateAction != null && stateAction.Active)
				{
					Fsm fsm = stateAction.Fsm;
					if (fsm != null && !fsm.Finished && fsm.ActiveState != null)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool SendEvent(T other)
		{
			if (pendingRemoval)
			{
				return false;
			}
			if (!IsValid)
			{
				return false;
			}
			callback?.Invoke(other);
			return !pendingRemoval;
		}

		public EventResponder(FsmStateAction stateAction, Action<T> callback)
		{
			this.stateAction = stateAction;
			this.callback = callback;
		}

		public override bool Equals(object obj)
		{
			if (obj is EventResponder eventResponder)
			{
				return stateAction == eventResponder.stateAction;
			}
			return false;
		}

		public bool Equals(FsmStateAction stateAction)
		{
			if (stateAction == null)
			{
				return false;
			}
			return this.stateAction.Equals(stateAction);
		}

		public override int GetHashCode()
		{
			return stateAction?.GetHashCode() ?? 0;
		}
	}

	private HashSet<EventResponder> eventResponders = new HashSet<EventResponder>();

	private List<EventResponder> runList = new List<EventResponder>();

	private bool listDirty;

	private bool sendingEvents;

	private EventResponder removalHelper = new EventResponder(null, null);

	protected void SendEvent(T other)
	{
		if (eventResponders.Count <= 0)
		{
			return;
		}
		if (listDirty)
		{
			runList.Clear();
			if (runList.Capacity < eventResponders.Count)
			{
				runList.Capacity = eventResponders.Count;
			}
			runList.AddRange(eventResponders);
			listDirty = false;
		}
		sendingEvents = true;
		for (int num = runList.Count - 1; num >= 0; num--)
		{
			EventResponder eventResponder = runList[num];
			if (!eventResponder.SendEvent(other))
			{
				eventResponders.Remove(eventResponder);
			}
		}
		if (eventResponders.Count == 0)
		{
			runList.Clear();
		}
		sendingEvents = false;
	}

	public EventResponder Add(FsmStateAction fsmStateAction, Action<T> collisionCallback)
	{
		if (fsmStateAction == null)
		{
			return null;
		}
		if (collisionCallback == null)
		{
			return null;
		}
		EventResponder eventResponder = new EventResponder(fsmStateAction, collisionCallback);
		if (!eventResponders.Add(eventResponder))
		{
			if (eventResponders.Remove(eventResponder))
			{
				eventResponders.Add(eventResponder);
				listDirty = true;
			}
			else
			{
				eventResponder = null;
			}
		}
		else
		{
			listDirty = true;
		}
		return eventResponder;
	}

	public void Remove(EventResponder eventResponder)
	{
		if (eventResponder == null)
		{
			return;
		}
		eventResponder.pendingRemoval = true;
		if (!eventResponders.Remove(eventResponder))
		{
			return;
		}
		if (eventResponders.Count == 0)
		{
			if (!sendingEvents)
			{
				runList.Clear();
			}
		}
		else
		{
			listDirty = true;
		}
	}

	public void Remove(FsmStateAction fsmStateAction)
	{
		removalHelper.stateAction = fsmStateAction;
		if (!eventResponders.Remove(removalHelper))
		{
			return;
		}
		if (eventResponders.Count == 0)
		{
			if (!sendingEvents)
			{
				runList.Clear();
			}
		}
		else
		{
			listDirty = true;
		}
	}
}
