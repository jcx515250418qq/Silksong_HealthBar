using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ScreenFaderCurveWait : FsmStateAction
	{
		public FsmColor StartColour;

		[RequiredField]
		public FsmColor EndColour;

		public FsmAnimationCurve Curve;

		public FsmFloat Duration;

		private float elapsed;

		public override void Reset()
		{
			StartColour = null;
			EndColour = null;
			Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			Duration = null;
		}

		public override void OnEnter()
		{
			elapsed = 0f;
			if (Duration.Value > 0f)
			{
				ScreenFaderUtils.SetColour(StartColour.Value);
				return;
			}
			ScreenFaderUtils.SetColour(EndColour.Value);
			Finish();
		}

		public override void OnUpdate()
		{
			elapsed += Time.deltaTime;
			float t = Curve.curve.Evaluate(elapsed / Duration.Value);
			ScreenFaderUtils.SetColour(Color.Lerp(StartColour.Value, EndColour.Value, t));
			if (elapsed >= Duration.Value)
			{
				ScreenFaderUtils.SetColour(EndColour.Value);
				Finish();
			}
		}
	}
}
