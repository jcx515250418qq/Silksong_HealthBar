using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ScreenFaderAlpha : FsmStateAction
	{
		[RequiredField]
		public FsmFloat EndAlpha;

		public FsmFloat Duration;

		public override void Reset()
		{
			EndAlpha = null;
			Duration = null;
		}

		public override void OnEnter()
		{
			Color colour;
			Color startColour = (colour = ScreenFaderUtils.GetColour());
			colour.a = EndAlpha.Value;
			ScreenFaderUtils.Fade(startColour, colour, Duration.Value);
			Finish();
		}
	}
}
