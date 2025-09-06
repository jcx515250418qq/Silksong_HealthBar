using UnityEngine;

namespace HutongGames.PlayMaker
{
	public sealed class PlayAudioSyncedVibration : FsmStateAction
	{
		[ObjectType(typeof(AudioSyncedVibration))]
		public FsmOwnerDefault target;

		public FsmFloat fadeInDuration;

		[Space]
		public FsmBool stopOnStateExit;

		public FsmFloat fadeOutDuration;

		private AudioSyncedVibration audioSyncedVibration;

		public override void Reset()
		{
			target = null;
			fadeInDuration = null;
			stopOnStateExit = null;
			fadeOutDuration = null;
		}

		public override void OnEnter()
		{
			audioSyncedVibration = target.GetSafe<AudioSyncedVibration>(this);
			if (audioSyncedVibration != null)
			{
				audioSyncedVibration.PlayVibration(fadeInDuration.Value);
			}
			Finish();
		}

		public override void OnExit()
		{
			if (stopOnStateExit.Value && audioSyncedVibration != null)
			{
				if (fadeOutDuration.Value > 0f)
				{
					audioSyncedVibration.FadeOut(fadeOutDuration.Value);
					audioSyncedVibration = null;
				}
				else
				{
					audioSyncedVibration.StopVibration();
					audioSyncedVibration = null;
				}
			}
		}
	}
}
