using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class StartNeedolinAudioLoop : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault AudioSource;

		[ObjectType(typeof(AudioClip))]
		public FsmObject DefaultClip;

		public override void Reset()
		{
			AudioSource = null;
			DefaultClip = null;
		}

		public override void OnEnter()
		{
			AudioSource component = AudioSource.GetSafe(this).GetComponent<AudioSource>();
			AudioClip defaultClip = DefaultClip.Value as AudioClip;
			OverrideNeedolinLoop.StartSyncedAudio(component, defaultClip);
			Finish();
		}
	}
}
