using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CreateNoise : FsmStateAction
	{
		public FsmOwnerDefault From;

		public FsmVector2 LocalOrigin;

		public FsmFloat Radius;

		public override void Reset()
		{
			From = null;
			LocalOrigin = null;
			Radius = null;
		}

		public override void OnEnter()
		{
			GameObject safe = From.GetSafe(this);
			if ((bool)safe)
			{
				NoiseMaker.CreateNoise(safe.transform.TransformPoint(LocalOrigin.Value), Radius.Value, NoiseMaker.Intensities.Normal);
			}
			Finish();
		}
	}
}
