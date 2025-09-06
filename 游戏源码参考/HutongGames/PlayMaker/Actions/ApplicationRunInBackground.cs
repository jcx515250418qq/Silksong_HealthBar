using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Sets if the Application should play in the background. Useful for servers or testing network games on one machine.")]
	public class ApplicationRunInBackground : FsmStateAction
	{
		[Tooltip("Should the Application play in the background.")]
		public FsmBool runInBackground;

		public override void Reset()
		{
			runInBackground = true;
		}

		public override void OnEnter()
		{
			Application.runInBackground = runInBackground.Value;
			Finish();
		}
	}
}
