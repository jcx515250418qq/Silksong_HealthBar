using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CameraRumbleSequence : FsmStateAction
	{
		public FsmFloat StartDelay;

		public FsmCameraShakeTarget Rumble;

		public FsmCameraShakeTarget EndShake;

		private float delayTimeLeft;

		public override void Reset()
		{
			StartDelay = null;
			Rumble = new FsmCameraShakeTarget
			{
				Camera = GlobalSettings.Camera.MainCameraShakeManager
			};
			EndShake = new FsmCameraShakeTarget
			{
				Camera = GlobalSettings.Camera.MainCameraShakeManager
			};
		}

		public override void OnEnter()
		{
			delayTimeLeft = StartDelay.Value;
			if (delayTimeLeft <= 0f)
			{
				Rumble.DoShake(base.Owner);
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (delayTimeLeft > 0f)
			{
				delayTimeLeft -= Time.deltaTime;
				if (delayTimeLeft <= 0f)
				{
					Rumble.DoShake(base.Owner);
					Finish();
				}
			}
		}

		public override void OnExit()
		{
			Rumble.CancelShake();
			EndShake.DoShake(base.Owner);
		}
	}
}
