using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class LimitedPlayAudioBase : FsmStateAction
	{
		public FsmString groupId;

		public FsmInt maxPerFrame = 2;

		public FsmInt maxActive = 5;

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
		}

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(groupId.Value))
			{
				Finish();
				return;
			}
			AudioGroupManager.EnsureGroupExists(groupId.Value, maxActive.Value, maxPerFrame.Value);
			if (!AudioGroupManager.CanPlay(groupId.Value, out var group))
			{
				Finish();
				return;
			}
			if (PlayAudio(out var audioSource))
			{
				group.AddSource(audioSource);
			}
			Finish();
		}

		protected abstract bool PlayAudio(out AudioSource audioSource);
	}
}
