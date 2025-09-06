namespace HutongGames.PlayMaker.Actions
{
	public class SetStateTransitionAnimator : FSMUtility.GetComponentFsmStateAction<StateTransitionAnimator>
	{
		public FsmBool SetState;

		public FsmBool IsInstant;

		public override void Reset()
		{
			base.Reset();
			SetState = null;
			IsInstant = null;
		}

		protected override void DoAction(StateTransitionAnimator component)
		{
			component.SetState(SetState.Value, IsInstant.Value);
		}
	}
}
