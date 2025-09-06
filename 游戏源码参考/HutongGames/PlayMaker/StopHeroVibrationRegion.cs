using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Vibration")]
	public sealed class StopHeroVibrationRegion : FsmStateAction
	{
		public FsmOwnerDefault target;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HeroVibrationRegion component = safe.GetComponent<HeroVibrationRegion>();
				if (component != null)
				{
					component.StopVibration();
				}
				else
				{
					Debug.LogError($"{this} in {base.Owner} is missing Hero Vibration Region component");
				}
			}
			else
			{
				Debug.LogWarning($"{this} in {base.Owner} is missing Hero Vibration Region object");
			}
			Finish();
		}
	}
}
