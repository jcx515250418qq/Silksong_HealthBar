using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DoCameraShake : FsmStateAction
	{
		public FsmOwnerDefault VisibleRenderer;

		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		public bool cancelOnExit;

		[RequiredField]
		public FsmBool DoFreeze;

		public FsmFloat Delay;

		private float delayLeft;

		private bool didShake;

		public override void Reset()
		{
			VisibleRenderer = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.SpecifyGameObject,
				GameObject = null
			};
			Camera = GlobalSettings.Camera.MainCameraShakeManager;
			Profile = null;
			cancelOnExit = false;
			DoFreeze = new FsmBool(true);
			Delay = null;
		}

		public override void OnEnter()
		{
			didShake = false;
			GameObject safe = VisibleRenderer.GetSafe(this);
			if ((bool)safe)
			{
				Renderer component = safe.GetComponent<Renderer>();
				if ((bool)component && !component.isVisible)
				{
					Finish();
					return;
				}
			}
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
			if (cancelOnExit && didShake)
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
			if (!didShake)
			{
				CameraManagerReference cameraManagerReference = Camera.Value as CameraManagerReference;
				CameraShakeProfile cameraShakeProfile = Profile.Value as CameraShakeProfile;
				if (cameraManagerReference != null && (bool)cameraShakeProfile)
				{
					cameraManagerReference.DoShake(cameraShakeProfile, base.Owner, DoFreeze.Value);
				}
				didShake = true;
				Finish();
			}
		}
	}
}
