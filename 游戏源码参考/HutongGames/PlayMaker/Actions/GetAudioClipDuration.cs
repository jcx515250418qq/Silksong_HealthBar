using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetAudioClipDuration : FsmStateAction
	{
		[ObjectType(typeof(AudioClip))]
		public FsmObject Clip;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDuration;

		public override void Reset()
		{
			Clip = null;
			StoreDuration = null;
		}

		public override void OnEnter()
		{
			AudioClip audioClip = Clip.Value as AudioClip;
			StoreDuration.Value = (audioClip ? audioClip.length : 0f);
			Finish();
		}
	}
}
