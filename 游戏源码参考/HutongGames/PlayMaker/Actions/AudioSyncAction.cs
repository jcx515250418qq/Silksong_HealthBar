using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class AudioSyncAction : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault SyncSource;

		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault SyncTarget;

		[Range(0f, 1f)]
		public FsmFloat Volume;

		public FsmBool CopyVolume;

		public FsmBool CopyClip;

		public FsmBool StartPlaying;

		public override void Reset()
		{
			SyncTarget = null;
			SyncTarget = null;
			Volume = new FsmFloat
			{
				UseVariable = true
			};
			CopyVolume = null;
			CopyClip = null;
			StartPlaying = null;
		}

		public override void OnEnter()
		{
			AudioSource safe = SyncTarget.GetSafe<AudioSource>(this);
			if (safe != null)
			{
				AudioSource safe2 = SyncSource.GetSafe<AudioSource>(this);
				if (safe2 != null)
				{
					if (CopyVolume.Value)
					{
						safe.volume = safe2.volume;
					}
					else if (!Volume.IsNone)
					{
						safe.volume = Volume.Value;
					}
					if (CopyClip.Value)
					{
						safe.clip = safe2.clip;
					}
					if (safe.clip != null && safe.clip == safe2.clip)
					{
						safe.timeSamples = safe2.timeSamples;
					}
					if (StartPlaying.Value)
					{
						safe.Play();
					}
				}
			}
			Finish();
		}
	}
}
