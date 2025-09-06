using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CreateNoiseV2 : FsmStateAction
	{
		public FsmOwnerDefault From;

		public FsmVector2 LocalOrigin;

		public FsmFloat Radius;

		[ObjectType(typeof(NoiseMaker.Intensities))]
		public FsmEnum Intensity;

		public override void Reset()
		{
			From = null;
			LocalOrigin = null;
			Radius = null;
			Intensity = null;
		}

		public override void OnEnter()
		{
			GameObject safe = From.GetSafe(this);
			if ((bool)safe)
			{
				NoiseMaker.CreateNoise(safe.transform.TransformPoint(LocalOrigin.Value), Radius.Value, (NoiseMaker.Intensities)(object)Intensity.Value);
			}
			Finish();
		}
	}
}
