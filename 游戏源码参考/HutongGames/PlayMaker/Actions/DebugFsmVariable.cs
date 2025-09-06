namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Print the value of any FSM Variable in the PlayMaker Log Window.")]
	public class DebugFsmVariable : BaseLogAction
	{
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to debug.")]
		public FsmVar variable;

		public override void Reset()
		{
			logLevel = LogLevel.Info;
			variable = null;
			base.Reset();
		}

		public override void OnEnter()
		{
			ActionHelpers.DebugLog(base.Fsm, logLevel, variable.DebugString(), sendToUnityLog);
			Finish();
		}
	}
}
