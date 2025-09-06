using UnityEngine;

namespace HutongGames.PlayMaker
{
	[ActionCategory("Vibration")]
	public sealed class VibrationRegionPlayOneShotSynced : FsmStateAction
	{
		public FsmOwnerDefault target;

		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		[ObjectType(typeof(AudioSource))]
		public FsmOwnerDefault audioSource;

		public FsmBool requireInside;

		public FsmBool loop;

		public FsmBool isRealTime;

		public FsmString tag;

		public FsmBool stopOnRegionExit;

		public FsmBool stopOnStateExit;

		private HeroVibrationRegion vibrationRegion;

		private VibrationEmission emission;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			vibrationDataAsset = null;
			audioSource = new FsmOwnerDefault();
			requireInside = null;
			loop = null;
			isRealTime = null;
			tag = null;
			stopOnRegionExit = null;
			stopOnStateExit = null;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				vibrationRegion = safe.GetComponent<HeroVibrationRegion>();
				if (vibrationRegion != null)
				{
					VibrationDataAsset vibrationDataAsset = this.vibrationDataAsset.Value as VibrationDataAsset;
					if (vibrationDataAsset != null)
					{
						HeroVibrationRegion.VibrationSettings vibrationSettings = HeroVibrationRegion.VibrationSettings.None;
						if (loop.Value)
						{
							vibrationSettings |= HeroVibrationRegion.VibrationSettings.Loop;
						}
						if (isRealTime.Value)
						{
							vibrationSettings |= HeroVibrationRegion.VibrationSettings.RealTime;
						}
						if (stopOnRegionExit.Value)
						{
							vibrationSettings |= HeroVibrationRegion.VibrationSettings.StopOnExit;
						}
						emission = vibrationRegion.PlayVibrationOneShot(vibrationDataAsset, requireInside.Value, vibrationSettings, tag.Value);
						AudioVibrationSyncer.StartSyncedEmission(emission, audioSource.GetSafe<AudioSource>(this));
					}
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

		public override void OnExit()
		{
			if (stopOnStateExit.Value && emission != null)
			{
				emission.Stop();
				emission = null;
			}
		}
	}
}
