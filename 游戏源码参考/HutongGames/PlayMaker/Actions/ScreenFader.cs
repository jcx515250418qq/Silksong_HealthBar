namespace HutongGames.PlayMaker.Actions
{
	public class ScreenFader : FsmStateAction
	{
		public FsmColor startColour;

		[RequiredField]
		public FsmColor endColour;

		public FsmFloat duration;

		public override void Reset()
		{
			startColour = null;
			endColour = null;
			duration = null;
		}

		public override void OnEnter()
		{
			ScreenFaderUtils.Fade(startColour.IsNone ? ScreenFaderUtils.GetColour() : startColour.Value, endColour.Value, duration.Value);
			Finish();
		}
	}
}
