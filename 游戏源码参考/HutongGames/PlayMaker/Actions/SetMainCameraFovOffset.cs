using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetMainCameraFovOffset : FsmStateAction
	{
		public FsmFloat FovOffset;

		public FsmFloat TransitionTime;

		public FsmAnimationCurve TransitionCurve;

		public override void Reset()
		{
			FovOffset = null;
			TransitionTime = null;
			TransitionCurve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
		}

		public override void OnEnter()
		{
			GameCameras.instance.forceCameraAspect.SetFovOffset(FovOffset.Value, TransitionTime.Value, TransitionCurve.curve);
			Finish();
		}
	}
}
