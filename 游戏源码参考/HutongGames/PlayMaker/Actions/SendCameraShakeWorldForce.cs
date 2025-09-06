using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	public class SendCameraShakeWorldForce : FsmStateAction
	{
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		[ObjectType(typeof(CameraShakeWorldForceIntensities))]
		public FsmEnum WorldForce;

		public override void Reset()
		{
			Camera = GlobalSettings.Camera.MainCameraShakeManager;
			WorldForce = null;
		}

		public override void OnEnter()
		{
			CameraManagerReference cameraManagerReference = Camera.Value as CameraManagerReference;
			if (cameraManagerReference != null)
			{
				cameraManagerReference.SendWorldForce((CameraShakeWorldForceIntensities)(object)WorldForce.Value);
			}
			Finish();
		}
	}
}
