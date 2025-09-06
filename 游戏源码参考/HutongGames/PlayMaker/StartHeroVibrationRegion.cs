using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Vibration")]
	public sealed class StartHeroVibrationRegion : FsmStateAction
	{
		public FsmOwnerDefault target;

		public FsmBool stopOnExit;

		private HeroVibrationRegion vibrationRegion;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			stopOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				vibrationRegion = safe.GetComponent<HeroVibrationRegion>();
				if (vibrationRegion != null)
				{
					vibrationRegion.StartVibration();
				}
				else
				{
					Debug.LogError($"{this} in {base.Owner} is missing Hero Vibration Region component");
				}
			}
			else
			{
				Debug.LogError($"{this} in {base.Owner} is missing Hero Vibration Region object");
			}
			Finish();
		}

		public override void OnExit()
		{
			if (stopOnExit.Value && vibrationRegion != null)
			{
				vibrationRegion.StopVibration();
			}
		}
	}
}
