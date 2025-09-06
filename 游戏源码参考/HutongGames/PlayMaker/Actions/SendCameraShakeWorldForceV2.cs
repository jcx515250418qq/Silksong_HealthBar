using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	public class SendCameraShakeWorldForceV2 : FsmStateAction
	{
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		[ObjectType(typeof(CameraShakeWorldForceIntensities))]
		public FsmEnum WorldForce;

		public FsmBool EveryFrame;

		public FsmFloat Rate;

		private CameraManagerReference camera;

		private bool hasReference;

		public override void Reset()
		{
			Camera = GlobalSettings.Camera.MainCameraShakeManager;
			WorldForce = null;
			EveryFrame = null;
			Rate = 0.1f;
		}

		public override void OnEnter()
		{
			camera = Camera.Value as CameraManagerReference;
			hasReference = camera != null;
			if (hasReference)
			{
				camera.SendWorldForce((CameraShakeWorldForceIntensities)(object)WorldForce.Value);
			}
			if (!EveryFrame.Value || !hasReference)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!hasReference)
			{
				Finish();
			}
			else
			{
				camera.SendWorldShaking((CameraShakeWorldForceIntensities)(object)WorldForce.Value);
			}
		}
	}
}
