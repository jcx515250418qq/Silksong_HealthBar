using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetGeoDrop : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt smallGeo;

		public FsmInt mediumGeo;

		public FsmInt largeGeo;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			smallGeo = new FsmInt();
			mediumGeo = new FsmInt();
			largeGeo = new FsmInt();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					if (!smallGeo.IsNone)
					{
						component.SetGeoSmall(smallGeo.Value);
					}
					if (!mediumGeo.IsNone)
					{
						component.SetGeoMedium(mediumGeo.Value);
					}
					if (!largeGeo.IsNone)
					{
						component.SetGeoLarge(largeGeo.Value);
					}
				}
			}
			Finish();
		}
	}
}
