using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CameraFollowInState : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool KeepOffset;

		private Vector2 initialOffset;

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
			Vector2 vector = followTarget.position;
			Vector2 vector2 = camTarget.transform.position;
			initialOffset = vector - vector2;
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
			Vector2 vector = followTarget.position;
			if (KeepOffset.Value)
			{
				vector += initialOffset;
			}
			camCtrl.SnapTo(vector.x, vector.y);
		}
	}
}
