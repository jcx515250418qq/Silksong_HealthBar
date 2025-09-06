using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CameraFollowYInState : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool KeepOffset;

		private float initialOffset;

		private Transform followTarget;

		private CameraController camCtrl;

		private CameraTarget camTarget;

		public override void Reset()
		{
			Target = null;
			KeepOffset = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			followTarget = safe.transform;
			GameCameras instance = GameCameras.instance;
			camCtrl = instance.cameraController;
			camTarget = instance.cameraTarget;
			float y = followTarget.position.y;
			float y2 = camTarget.transform.position.y;
			initialOffset = y - y2;
			camCtrl.SetMode(CameraController.CameraMode.PANNING);
			AdjustCameraPosition();
		}

		public override void OnUpdate()
		{
			AdjustCameraPosition();
		}

		public override void OnExit()
		{
			camCtrl.SetMode(CameraController.CameraMode.PREVIOUS);
		}

		private void AdjustCameraPosition()
		{
			float num = followTarget.position.y;
			if (KeepOffset.Value)
			{
				num += initialOffset;
			}
			camCtrl.SnapTargetToY(num);
		}
	}
}
