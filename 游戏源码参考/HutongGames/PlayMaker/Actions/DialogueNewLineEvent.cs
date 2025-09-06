using System;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class DialogueNewLineEvent : FsmStateAction
	{
		[Serializable]
		private enum LineOwner
		{
			Any = 0,
			Player = 1,
			Other = 2
		}

		[RequiredField]
		[CheckForComponent(typeof(NPCControlBase))]
		public FsmOwnerDefault Target;

		public FsmBool IgnoreFirstLine;

		[ObjectType(typeof(LineOwner))]
		public FsmEnum LineSpeaker;

		public FsmString ExpectedLineEvent;

		public FsmEvent LineStartedEvent;

		private NPCControlBase target;

		private bool seenFirstLine;

		private bool registeredEvent;

		public override void Reset()
		{
			Target = null;
			IgnoreFirstLine = null;
			ExpectedLineEvent = new FsmString
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			target = Target.GetSafe<NPCControlBase>(this);
			if (target != null)
			{
				seenFirstLine = false;
				RegisterEvents();
			}
			Finish();
		}

		public override void OnExit()
		{
			base.OnExit();
			UnregisterEvents();
			target = null;
		}

		private void RegisterEvents()
		{
			if (!registeredEvent)
			{
				registeredEvent = true;
				target.StartedNewLine += TargetOnStartedNewLine;
			}
		}

		private void UnregisterEvents()
		{
			if (registeredEvent)
			{
				registeredEvent = false;
				if (target != null)
				{
					target.StartedNewLine -= TargetOnStartedNewLine;
				}
			}
		}

		private void TargetOnStartedNewLine(DialogueBox.DialogueLine line)
		{
			bool num = !seenFirstLine;
			seenFirstLine = true;
			if (num && IgnoreFirstLine.Value)
			{
				return;
			}
			switch ((LineOwner)(object)LineSpeaker.Value)
			{
			case LineOwner.Player:
				if (!line.IsPlayer)
				{
					return;
				}
				break;
			case LineOwner.Other:
				if (line.IsPlayer)
				{
					return;
				}
				break;
			}
			if (!ExpectedLineEvent.IsNone)
			{
				string value = ExpectedLineEvent.Value;
				if (!string.IsNullOrEmpty(value))
				{
					string @event = line.Event;
					if (string.IsNullOrEmpty(@event))
					{
						return;
					}
					value = value.Trim();
					@event = @event.Trim();
					if (value != @event)
					{
						return;
					}
				}
			}
			base.Fsm.Event(LineStartedEvent);
		}
	}
}
