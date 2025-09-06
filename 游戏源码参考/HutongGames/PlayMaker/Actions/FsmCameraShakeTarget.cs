using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Serializable]
	public class FsmCameraShakeTarget
	{
		[ObjectType(typeof(CameraManagerReference))]
		public FsmObject Camera;

		[ObjectType(typeof(CameraShakeProfile))]
		public FsmObject Profile;

		public void DoShake(UnityEngine.Object source)
		{
			CameraManagerReference cameraManagerReference = Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null && (bool)cameraShakeProfile)
			{
				cameraManagerReference.DoShake(cameraShakeProfile, source);
			}
		}

		public void CancelShake()
		{
			CameraManagerReference cameraManagerReference = Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null && (bool)cameraShakeProfile)
			{
				cameraManagerReference.CancelShake(cameraShakeProfile);
			}
		}
	}
}
