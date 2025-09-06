using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DoCameraShakeRepeatingV2 : FsmStateAction
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

		public FsmFloat RepeatDelay;

		public FsmBool vibrate;

		private float delayLeft;

		private float repeatDelayLeft;

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
			RepeatDelay = null;
			vibrate = true;
		}

		public override void OnEnter()
		{
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
			if (repeatDelayLeft > 0f)
			{
				repeatDelayLeft -= Time.deltaTime;
				if (repeatDelayLeft <= 0f)
				{
					DoShake();
				}
			}
		}

		private void DoShake()
		{
			CameraManagerReference cameraManagerReference = Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null && (bool)cameraShakeProfile)
			{
				GameObject safe = Target.GetSafe(this);
				if (!MaxCameraDistance.IsNone && (bool)safe)
				{
					cameraManagerReference.DoShakeInRange(cameraShakeProfile, base.Owner, MaxCameraDistance.Value, safe.transform.position, DoFreeze.Value, vibrate.Value);
				}
				else
				{
					cameraManagerReference.DoShake(cameraShakeProfile, base.Owner, DoFreeze.Value, vibrate.Value);
				}
			}
			repeatDelayLeft = RepeatDelay.Value;
		}
	}
}
