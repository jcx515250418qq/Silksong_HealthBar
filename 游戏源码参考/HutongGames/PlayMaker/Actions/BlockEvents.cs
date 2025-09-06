using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Block events while this action is active.")]
	public class BlockEvents : FsmStateAction
	{
		public enum Options
		{
			Timeout = 0,
			WhileTrue = 1,
			WhileFalse = 2,
			UntilTrue = 3,
			UntilFalse = 4,
			UntilEvent = 5
		}

		[Tooltip("When to block events.")]
		public Options condition;

		[Tooltip("Context sensitive parameter. Depends on Condition.")]
		public FsmFloat floatParam;

		[Tooltip("Context sensitive parameter. Depends on Condition.")]
		public FsmBool boolParam;

		[EventNotSent]
		[Tooltip("Context sensitive parameter. Depends on Condition.")]
		public FsmEvent eventParam;

		[ActionSection("Debug")]
		[Tooltip("Log any events blocked by this action. Helpful for debugging.")]
		public FsmBool logBlockedEvents;

		private bool firstTime = true;

		public override void Reset()
		{
			condition = Options.Timeout;
			floatParam = null;
			boolParam = null;
			eventParam = null;
			logBlockedEvents = false;
		}

		public override void Awake()
		{
			base.HandlesOnEvent = true;
		}

		public override void OnUpdate()
		{
			switch (condition)
			{
			case Options.Timeout:
				if (boolParam.Value)
				{
					if (FsmTime.RealtimeSinceStartup - base.State.RealStartTime > floatParam.Value)
					{
						Finish();
					}
				}
				else if (base.State.StateTime > floatParam.Value)
				{
					Finish();
				}
				break;
			case Options.UntilFalse:
				if (!boolParam.Value)
				{
					Finish();
				}
				break;
			case Options.UntilTrue:
				if (boolParam.Value)
				{
					Finish();
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case Options.WhileTrue:
			case Options.WhileFalse:
			case Options.UntilEvent:
				break;
			}
		}

		public override bool Event(FsmEvent fsmEvent)
		{
			if (firstTime)
			{
				if (!Validate())
				{
					Finish();
				}
				firstTime = false;
			}
			if (base.Finished)
			{
				return false;
			}
			if (Fsm.EventData.SentByState == base.State || fsmEvent == FsmEvent.Finished)
			{
				return false;
			}
			bool num = DoBlockEvent(fsmEvent);
			if (num && logBlockedEvents.Value)
			{
				ActionHelpers.DebugLog(base.Fsm, LogLevel.Info, "Blocked: " + fsmEvent.Name, sendToUnityLog: true);
			}
			return num;
		}

		private bool Validate()
		{
			switch (condition)
			{
			case Options.Timeout:
				return !floatParam.IsNone;
			case Options.WhileTrue:
			case Options.WhileFalse:
			case Options.UntilTrue:
			case Options.UntilFalse:
				return !boolParam.IsNone;
			case Options.UntilEvent:
				return !string.IsNullOrEmpty(eventParam.Name);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private bool DoBlockEvent(FsmEvent fsmEvent)
		{
			switch (condition)
			{
			case Options.Timeout:
				if (boolParam.Value)
				{
					if (FsmTime.RealtimeSinceStartup - base.State.RealStartTime < floatParam.Value)
					{
						return true;
					}
				}
				else if (base.State.StateTime < floatParam.Value)
				{
					return true;
				}
				return false;
			case Options.WhileFalse:
				return !boolParam.Value;
			case Options.WhileTrue:
				return boolParam.Value;
			case Options.UntilFalse:
				return boolParam.Value;
			case Options.UntilTrue:
				return !boolParam.Value;
			case Options.UntilEvent:
				if (fsmEvent.Name == eventParam.Name)
				{
					Finish();
					if (boolParam.Value)
					{
						return false;
					}
				}
				return true;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
