using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Quits the player application.")]
	public class ApplicationQuit : FsmStateAction
	{
		[Tooltip("An optional exit code to return when the player application terminates on Windows, Mac and Linux. Defaults to 0.")]
		public FsmInt exitCode;

		public override void Reset()
		{
			exitCode = 0;
		}

		public override void OnEnter()
		{
			Application.Quit(exitCode.Value);
			Finish();
		}
	}
}
