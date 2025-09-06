namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the name of the previously active state and stores it in a String Variable.")]
	public class GetPreviousStateName : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the name in a String Variable.")]
		public FsmString storeName;

		public override void Reset()
		{
			storeName = null;
		}

		public override void OnEnter()
		{
			storeName.Value = ((base.Fsm.PreviousActiveState == null) ? null : base.Fsm.PreviousActiveState.Name);
			Finish();
		}
	}
}
