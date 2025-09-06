using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CreateNoiseRect : FsmStateAction
	{
		public FsmOwnerDefault From;

		public FsmVector2 LocalOrigin;

		public FsmVector2 Size;

		[ObjectType(typeof(NoiseMaker.Intensities))]
		public FsmEnum Intensity;

		public override void Reset()
		{
			From = null;
			LocalOrigin = null;
			Size = null;
			Intensity = null;
		}

		public override void OnEnter()
		{
			GameObject safe = From.GetSafe(this);
			if ((bool)safe)
			{
				NoiseMaker.CreateNoise(safe.transform.TransformPoint(LocalOrigin.Value), Size.Value, (NoiseMaker.Intensities)(object)Intensity.Value);
			}
			Finish();
		}
	}
}
