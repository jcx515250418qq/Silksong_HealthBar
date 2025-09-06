using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class LimitedPlayAudioLooped : FsmStateAction
	{
		public FsmString groupId;

		public FsmInt maxPerFrame = 2;

		public FsmInt maxActive = 5;

		[Space]
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault audioSource;

		[RequiredField]
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume;

		public override void Reset()
		{
			groupId = null;
			maxPerFrame = new FsmInt
			{
				UseVariable = true
			};
			maxActive = new FsmInt
			{
				UseVariable = true
			};
			audioSource = null;
			audioClip = null;
			volume = new FsmFloat
			{
				UseVariable = true
			};
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
			AudioGroupManager.EnsureGroupExists(groupId.Value, maxActive.Value, maxPerFrame.Value);
			AudioGroupManager.PlayLoopClip(groupId.Value, safe, audioClip.Value as AudioClip, volume.IsNone ? 1f : volume.Value);
			Finish();
		}
	}
}
