namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Sends a log message to the Console Log Window.")]
	public class DebugLogConsole : BaseLogAction
	{
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		[Tooltip("Text to send to the log.")]
		public FsmString text;

		public override void Reset()
		{
			logLevel = LogLevel.Info;
			text = "";
			base.Reset();
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
