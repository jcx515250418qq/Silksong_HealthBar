using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DoCameraShakeV2 : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmVector2 MaxCameraDistance;

		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		[RequiredField]
		public FsmBool DoFreeze;

		public FsmFloat Delay;

		public bool CancelOnExit;

		private float delayLeft;

		private bool didShake;

		public override void Reset()
		{
			Target = null;
			Camera = GlobalSettings.Camera.MainCameraShakeManager;
			MaxCameraDistance = new FsmVector2
			{
				UseVariable = true
			};
			Profile = null;
			DoFreeze = new FsmBool(true);
			Delay = null;
			CancelOnExit = false;
		}

		public override void OnEnter()
		{
			didShake = false;
			if (Delay.Value <= 0f)
			{
				DoShake();
			}
			else
			{
				delayLeft = Delay.Value;
			}
		}

		public override void OnUpdate()
		{
			if (delayLeft > 0f)
			{
				delayLeft -= Time.deltaTime;
				if (delayLeft <= 0f)
				{
					DoShake();
				}
			}
		}

		public override void OnExit()
		{
			if (CancelOnExit && didShake)
			{
				CameraManagerReference cameraManagerReference = Camera.Value as CameraManagerReference;
				CameraShakeProfile cameraShakeProfile = Profile.Value as CameraShakeProfile;
				if (cameraManagerReference != null && (bool)cameraShakeProfile)
				{
					cameraManagerReference.CancelShake(cameraShakeProfile);
				}
			}
		}

		private void DoShake()
		{
			if (didShake)
			{
				return;
			}
			CameraManagerReference cameraManagerReference = Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null && (bool)cameraShakeProfile)
			{
				GameObject safe = Target.GetSafe(this);
				if (!MaxCameraDistance.IsNone && (bool)safe)
				{
					cameraManagerReference.DoShakeInRange(cameraShakeProfile, base.Owner, MaxCameraDistance.Value, safe.transform.position, DoFreeze.Value);
				}
				else
				{
					cameraManagerReference.DoShake(cameraShakeProfile, base.Owner, DoFreeze.Value);
				}
			}
			didShake = true;
			Finish();
		}
	}
}
