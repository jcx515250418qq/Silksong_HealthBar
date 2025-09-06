using System;
using System.Collections.Generic;
using TeamCherry.PS5;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class PlayVibrationV2 : FsmStateAction
	{
		[Serializable]
		public class FSMVibrationData
		{
			[ObjectType(typeof(LowFidelityVibrations))]
			public FsmEnum lowFidelityVibration;

			[ObjectType(typeof(TextAsset))]
			public FsmObject highFidelityVibration;

			[ObjectType(typeof(GamepadVibration))]
			public FsmObject gamepadVibration;

			[ObjectType(typeof(PS5VibrationData))]
			public FsmObject ps5Vibration;

			public VibrationData VibrationData => this;

			public void Reset()
			{
				lowFidelityVibration = new FsmEnum
				{
					UseVariable = false
				};
				highFidelityVibration = new FsmObject
				{
					UseVariable = false
				};
				gamepadVibration = new FsmObject
				{
					UseVariable = false
				};
				ps5Vibration = new FsmObject
				{
					UseVariable = false
				};
			}

			public static implicit operator VibrationData(FSMVibrationData wrapper)
			{
				if (wrapper != null)
				{
					LowFidelityVibrations num = (LowFidelityVibrations)(object)wrapper.lowFidelityVibration.Value;
					TextAsset textAsset = wrapper.highFidelityVibration.Value as TextAsset;
					GamepadVibration gamepadVibration = wrapper.gamepadVibration.Value as GamepadVibration;
					PS5VibrationData pS5VibrationData = wrapper.ps5Vibration.Value as PS5VibrationData;
					return VibrationData.Create(num, textAsset, gamepadVibration, pS5VibrationData);
				}
				return default(VibrationData);
			}
		}

		public FSMVibrationData vibrationData;

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
			vibrationData = new FSMVibrationData();
			vibrationData.Reset();
			vibrationDataAsset = null;
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
			VibrationMotors vibrationMotors = VibrationMotors.All;
			if (!motors.IsNone)
			{
				vibrationMotors = (VibrationMotors)(object)motors.Value;
				if (vibrationMotors == VibrationMotors.None && this.vibrationData.gamepadVibration != null)
				{
					vibrationMotors = VibrationMotors.All;
				}
			}
			VibrationData vibrationData = this.vibrationData;
			if ((bool)vibrationDataAsset.Value)
			{
				vibrationData = (VibrationDataAsset)vibrationDataAsset.Value;
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
			if (stopOnStateExit.Value || loopAuto.Value)
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
