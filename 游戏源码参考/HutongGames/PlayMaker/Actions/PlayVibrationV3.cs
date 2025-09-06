using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class PlayVibrationV3 : FsmStateAction
	{
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		[ObjectType(typeof(VibrationMotors))]
		public FsmEnum motors;

		public FsmFloat loopTime;

		public FsmBool loopAuto;

		public FsmString tag;

		public FsmBool stopOnStateExit;

		private float cooldownTimer;

		private List<VibrationEmission> emissions = new List<VibrationEmission>();

		public override void Reset()
		{
			base.Reset();
			vibrationDataAsset = null;
			motors = new FsmEnum
			{
				UseVariable = true,
				Value = VibrationMotors.All
			};
			loopTime = new FsmFloat
			{
				UseVariable = true
			};
			loopAuto = null;
			tag = null;
			stopOnStateExit = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			bool value = loopAuto.Value;
			Play(value);
			if (value)
			{
				Finish();
			}
			else
			{
				EnqueueNextLoop();
			}
		}

		private void Play(bool loop)
		{
			if (ObjectPool.IsCreatingPool)
			{
				return;
			}
			VibrationData vibrationData = (VibrationDataAsset)vibrationDataAsset.Value;
			vibrationData = (VibrationDataAsset)vibrationDataAsset.Value;
			VibrationMotors vibrationMotors = VibrationMotors.All;
			if (!motors.IsNone)
			{
				vibrationMotors = (VibrationMotors)(object)motors.Value;
				if (vibrationMotors == VibrationMotors.None && vibrationData.GamepadVibration != null)
				{
					vibrationMotors = VibrationMotors.All;
				}
			}
			emissions.Add(VibrationManager.PlayVibrationClipOneShot(vibrationData, new VibrationTarget(vibrationMotors), loop, tag.Value ?? ""));
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

		public override void OnExit()
		{
			if (stopOnStateExit.Value)
			{
				foreach (VibrationEmission emission in emissions)
				{
					emission?.Stop();
				}
			}
			emissions.Clear();
		}

		private void EnqueueNextLoop()
		{
			if (!loopAuto.Value)
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
}
