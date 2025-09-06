namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Adds a text area to the action list for notes etc. Use this to document your project.")]
	public class Comment : FsmStateAction
	{
		[UIHint(UIHint.Comment)]
		[Tooltip("Any comment you care to make...")]
		public string comment;

		public override void Reset()
		{
			comment = "Double-Click To Edit";
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
