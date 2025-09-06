using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class PlayVibration : FsmStateAction
	{
		[ObjectType(typeof(LowFidelityVibrations))]
		public FsmEnum lowFidelityVibration;

		[ObjectType(typeof(TextAsset))]
		public FsmObject highFidelityVibration;

		[ObjectType(typeof(VibrationMotors))]
		public FsmEnum motors;

		public FsmFloat loopTime;

		public FsmBool isLooping;

		public FsmString tag;

		[ObjectType(typeof(GamepadVibration))]
		public FsmObject gamepadVibration;

		private float cooldownTimer;

		public override void Reset()
		{
			base.Reset();
			lowFidelityVibration = new FsmEnum
			{
				UseVariable = false
			};
			highFidelityVibration = new FsmObject
			{
				UseVariable = false
			};
			motors = new FsmEnum
			{
				UseVariable = false,
				Value = VibrationMotors.All
			};
			loopTime = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			Play(loop: false);
			EnqueueNextLoop();
		}

		private void Play(bool loop)
		{
			if (!ObjectPool.IsCreatingPool)
			{
				VibrationMotors vibrationMotors = VibrationMotors.All;
				if (!motors.IsNone)
				{
					vibrationMotors = (VibrationMotors)(object)motors.Value;
				}
				VibrationManager.PlayVibrationClipOneShot(VibrationData.Create((LowFidelityVibrations)(object)lowFidelityVibration.Value, highFidelityVibration.Value as TextAsset, gamepadVibration.Value as GamepadVibration), new VibrationTarget(vibrationMotors), loop, tag.Value ?? "");
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			cooldownTimer -= Time.deltaTime;
			if (cooldownTimer <= 0f)
			{
				Play(loop: false);
				EnqueueNextLoop();
			}
		}

		private void EnqueueNextLoop()
		{
			float num = 0f;
			if (!loopTime.IsNone)
			{
				num = loopTime.Value;
			}
			if (num < Mathf.Epsilon)
			{
				Finish();
			}
			else
			{
				cooldownTimer = num;
			}
		}
	}
}
