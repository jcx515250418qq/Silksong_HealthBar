namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class StopVibration : FsmStateAction
	{
		private FsmString fsmTag;

		public override void Reset()
		{
			base.Reset();
			fsmTag = new FsmString
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (fsmTag == null || fsmTag.IsNone || string.IsNullOrEmpty(fsmTag.Value))
			{
				VibrationManager.StopAllVibration();
			}
			else
			{
				VibrationManager.GetMixer()?.StopAllEmissionsWithTag(fsmTag.Value);
			}
			Finish();
		}
	}
}
