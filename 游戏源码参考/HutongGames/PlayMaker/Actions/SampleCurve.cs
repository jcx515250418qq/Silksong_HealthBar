namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Gets the value of a curve at a given time and stores it in a Float Variable. NOTE: This can be used for more than just animation! It's a general way to transform an input number into an output number using a curve (e.g., linear input -> bell curve).")]
	public class SampleCurve : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Click to edit the curve.")]
		public FsmAnimationCurve curve;

		[RequiredField]
		[Tooltip("Sample the curve at this point.")]
		public FsmFloat sampleAt;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the sampled value in a float variable.")]
		public FsmFloat storeValue;

		[Tooltip("Do it every frame. Useful if Sample At is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			curve = null;
			sampleAt = null;
			storeValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSampleCurve();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSampleCurve();
		}

		private void DoSampleCurve()
		{
			if (curve != null && curve.curve != null && storeValue != null)
			{
				storeValue.Value = curve.curve.Evaluate(sampleAt.Value);
			}
		}
	}
}
