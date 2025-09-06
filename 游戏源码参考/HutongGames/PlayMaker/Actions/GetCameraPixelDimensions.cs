namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets the value of a Float Variable.")]
	public class GetCameraPixelDimensions : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmFloat cameraWidth;

		[UIHint(UIHint.Variable)]
		public FsmFloat cameraHeight;

		public bool everyFrame;

		public override void Reset()
		{
			cameraWidth = null;
			cameraHeight = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetCamera();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetCamera();
		}

		private void DoGetCamera()
		{
		}
	}
}
