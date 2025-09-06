namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets the target FSM for all subsequent events sent by this state. The default 'Self' sends events to this FSM.")]
	public class SetEventTarget : FsmStateAction
	{
		[Tooltip("Set the target.")]
		public FsmEventTarget eventTarget;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			eventTarget = null;
			everyFrame = true;
		}

		public override void Awake()
		{
			base.BlocksFinish = false;
		}

		public override void OnEnter()
		{
			base.Fsm.EventTarget = eventTarget;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			base.Fsm.EventTarget = eventTarget;
		}
	}
}
