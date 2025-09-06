namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Immediately return to the previously active state.")]
	public class GotoPreviousState : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			if (base.Fsm.PreviousActiveState != null)
			{
				Log("Goto Previous State: " + base.Fsm.PreviousActiveState.Name);
				base.Fsm.GotoPreviousState();
			}
			Finish();
		}
	}
}
