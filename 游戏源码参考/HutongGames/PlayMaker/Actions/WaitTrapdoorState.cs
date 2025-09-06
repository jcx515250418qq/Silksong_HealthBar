namespace HutongGames.PlayMaker.Actions
{
	public sealed class WaitTrapdoorState : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Trapdoor))]
		public FsmOwnerDefault Target;

		public FsmBool everyFrame;

		public FsmEvent openedEvent;

		public FsmEvent closedEvent;

		private Trapdoor target;

		public override void Reset()
		{
			Target = null;
			everyFrame = null;
			openedEvent = null;
			closedEvent = null;
		}

		public override void OnEnter()
		{
			target = Target.GetSafe<Trapdoor>(this);
			if (target != null)
			{
				OnUpdate();
			}
			if (!everyFrame.Value || target == null)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (target.IsOpen)
			{
				base.Fsm.Event(openedEvent);
			}
			else
			{
				base.Fsm.Event(closedEvent);
			}
		}
	}
}
