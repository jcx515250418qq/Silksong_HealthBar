using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class LimitedPlayAudioClipOnSource : LimitedPlayAudioBase
	{
		[Space]
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault audioSource;

		[RequiredField]
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		public FsmBool playOneShot;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume;

		public override void Reset()
		{
			base.Reset();
			audioSource = null;
			audioClip = null;
			playOneShot = null;
			volume = new FsmFloat
			{
				UseVariable = true
			};
		}

		protected override bool PlayAudio(out AudioSource audioSource)
		{
			AudioSource safe = this.audioSource.GetSafe<AudioSource>(this);
			if (safe == null)
			{
				audioSource = null;
				return false;
			}
			AudioClip audioClip = this.audioClip.Value as AudioClip;
			if (audioClip == null)
			{
				audioSource = null;
				return false;
			}
			bool flag = ((!playOneShot.Value) ? AudioGroupManager.PlayClip(groupId.Value, safe, audioClip) : AudioGroupManager.PlayOneShotClip(groupId.Value, safe, audioClip));
			if (flag && !volume.IsNone)
			{
				safe.volume = volume.Value;
			}
			audioSource = safe;
			return flag;
		}
	}
}
