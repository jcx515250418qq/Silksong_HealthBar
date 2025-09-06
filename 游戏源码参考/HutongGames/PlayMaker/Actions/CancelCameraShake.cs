using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	public class CancelCameraShake : FsmStateAction
	{
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		public override void Reset()
		{
			Camera = GlobalSettings.Camera.MainCameraShakeManager;
			Profile = null;
		}

		public override void OnEnter()
		{
			CameraManagerReference cameraManagerReference = Camera.Value as CameraManagerReference;
			CameraShakeProfile shake = Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null)
			{
				cameraManagerReference.CancelShake(shake);
			}
			Finish();
		}
	}
}
