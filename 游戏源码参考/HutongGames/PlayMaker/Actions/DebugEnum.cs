namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of an Enum Variable in the PlayMaker Log Window.")]
	public class DebugEnum : BaseLogAction
	{
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		[UIHint(UIHint.Variable)]
		[Tooltip("The Enum Variable to debug.")]
		public FsmEnum enumVariable;

		public override void Reset()
		{
			logLevel = LogLevel.Info;
			enumVariable = null;
			base.Reset();
		}

		public override void OnEnter()
		{
			string text = "None";
			if (!enumVariable.IsNone)
			{
				text = enumVariable.Name + ": " + enumVariable.Value;
			}
			ActionHelpers.DebugLog(base.Fsm, logLevel, text, sendToUnityLog);
			Finish();
		}
	}
}
