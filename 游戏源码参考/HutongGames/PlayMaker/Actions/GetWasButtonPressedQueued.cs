using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	public class GetWasButtonPressedQueued : FsmStateAction
	{
		[ObjectType(typeof(HeroActionButton))]
		public FsmEnum Button;

		public FsmBool Consume;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreWasPressed;

		public FsmEvent PressedEvent;

		public FsmEvent NotPressedEvent;

		public FsmBool IsActive;

		public FsmBool EveryFrame;

		private HeroController hc;

		public override void Reset()
		{
			Button = null;
			Consume = null;
			StoreWasPressed = null;
			PressedEvent = null;
			NotPressedEvent = null;
			IsActive = true;
			EveryFrame = true;
		}

		public override void OnEnter()
		{
			hc = base.Owner.GetComponent<HeroController>();
			DoAction();
			if (!EveryFrame.Value)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		public override void OnExit()
		{
			hc = null;
		}

		private void DoAction()
		{
			if ((!hc || !hc.IsPaused()) && IsActive.Value)
			{
				InputHandler instance = ManagerSingleton<InputHandler>.Instance;
				HeroActionButton heroAction = (HeroActionButton)(object)Button.Value;
				bool wasButtonPressedQueued = instance.GetWasButtonPressedQueued(heroAction, Consume.Value);
				StoreWasPressed.Value = wasButtonPressedQueued;
				base.Fsm.Event(wasButtonPressedQueued ? PressedEvent : NotPressedEvent);
			}
		}
	}
}
