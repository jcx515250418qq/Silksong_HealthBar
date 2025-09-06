namespace HutongGames.PlayMaker.Actions
{
	public class FadeOutTransitionAudioFaders : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			TransitionAudioFader.FadeOutAllFaders();
			Finish();
		}
	}
}
