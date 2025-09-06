using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CreateNoiseContinuous : FsmStateAction
	{
		public FsmOwnerDefault From;

		public FsmVector2 LocalOrigin;

		public FsmFloat Radius;

		public FsmFloat InitialDelay;

		public FsmFloat Delay;

		private GameObject obj;

		private double nextNoiseTime;

		public override void Reset()
		{
			From = null;
			LocalOrigin = null;
			Radius = null;
			Delay = null;
		}

		public override void OnEnter()
		{
			obj = From.GetSafe(this);
			if (!obj)
			{
				Finish();
			}
			else if (InitialDelay.Value <= 0f)
			{
				Noise();
				nextNoiseTime = Time.timeAsDouble + (double)Delay.Value;
			}
			else
			{
				nextNoiseTime = Time.timeAsDouble + (double)InitialDelay.Value;
			}
		}

		public override void OnUpdate()
		{
			if (!(Time.timeAsDouble < nextNoiseTime))
			{
				nextNoiseTime = Time.timeAsDouble + (double)Delay.Value;
				Noise();
			}
		}

		private void Noise()
		{
			NoiseMaker.CreateNoise(obj.transform.TransformPoint(LocalOrigin.Value), Radius.Value, NoiseMaker.Intensities.Normal);
		}
	}
}
