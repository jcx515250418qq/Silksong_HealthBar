using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class LimitedRemoveAudioLooped : FsmStateAction
	{
		public FsmString groupId;

		[Space]
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault audioSource;

		public FsmBool stopSource;

		public override void Reset()
		{
			groupId = null;
			audioSource = null;
			stopSource = null;
		}

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(groupId.Value))
			{
				Finish();
				return;
			}
			AudioSource safe = audioSource.GetSafe<AudioSource>(this);
			if (safe == null)
			{
				Finish();
				return;
			}
			AudioGroupManager.RemoveLoopClip(groupId.Value, safe);
			if (stopSource.Value)
			{
				safe.Stop();
			}
			Finish();
		}
	}
}
