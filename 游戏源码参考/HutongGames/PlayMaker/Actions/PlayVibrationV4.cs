namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class PlayVibrationV4 : FsmStateAction
	{
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		public FsmFloat strength;

		[ObjectType(typeof(VibrationMotors))]
		public FsmEnum motors;

		public FsmBool loop;

		public FsmString tag;

		public FsmBool stopOnStateExit;

		public FsmBool waitUntilFinished;

		private VibrationEmission emission;

		public override void Reset()
		{
			base.Reset();
			vibrationDataAsset = null;
			strength = 1f;
			motors = new FsmEnum
			{
				UseVariable = true,
				Value = VibrationMotors.All
			};
			loop = null;
			tag = null;
			stopOnStateExit = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			bool value = loop.Value;
			Play(value);
			if (value || !waitUntilFinished.Value)
			{
				Finish();
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
			emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, new VibrationTarget(vibrationMotors), loop, tag.Value ?? "");
			if (emission != null)
			{
				emission.SetStrength(strength.Value);
			}
		}

		public override void OnUpdate()
		{
			if (emission != null && emission.IsPlaying)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (stopOnStateExit.Value)
			{
				emission?.Stop();
				emission = null;
			}
		}
	}
}
