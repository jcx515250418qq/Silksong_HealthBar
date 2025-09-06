using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DoSurfaceWaterSplash : FsmStateAction
	{
		public enum SplashTypes
		{
			In = 0,
			Out = 1
		}

		public FsmOwnerDefault Splasher;

		public FsmGameObject SplashSurface;

		[ObjectType(typeof(SplashTypes))]
		public FsmEnum SplashType;

		public override void Reset()
		{
			Splasher = null;
			SplashSurface = null;
			SplashType = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Splasher.GetSafe(this);
			if ((bool)safe && (bool)SplashSurface.Value)
			{
				SurfaceWaterRegion componentInParent = SplashSurface.Value.GetComponentInParent<SurfaceWaterRegion>();
				if ((bool)componentInParent)
				{
					switch ((SplashTypes)(object)SplashType.Value)
					{
					case SplashTypes.In:
						componentInParent.DoSplashIn(safe.transform, isBigSplash: false);
						break;
					case SplashTypes.Out:
						componentInParent.DoSplashOut(safe.transform, Vector2.zero);
						break;
					}
				}
			}
			Finish();
		}
	}
}
