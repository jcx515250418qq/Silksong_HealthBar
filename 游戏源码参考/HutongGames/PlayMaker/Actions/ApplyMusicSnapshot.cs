using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Get's the name of the currently playing music cue.")]
	public sealed class ApplyMusicSnapshot : FsmStateAction
	{
		[RequiredField]
		[ObjectType(typeof(AudioMixerSnapshot))]
		public FsmObject musicSnapshot;

		public FsmFloat delayTime;

		public FsmFloat transitionTime;

		public FsmBool skipMusicMarkerBlock;

		public override void Reset()
		{
			musicSnapshot = null;
			delayTime = null;
			transitionTime = null;
			skipMusicMarkerBlock = null;
		}

		public override void OnEnter()
		{
			AudioMixerSnapshot audioMixerSnapshot = musicSnapshot.Value as AudioMixerSnapshot;
			GameManager instance = GameManager.instance;
			if (!(audioMixerSnapshot == null) && !(instance == null))
			{
				instance.AudioManager.ApplyMusicSnapshot(audioMixerSnapshot, delayTime.Value, transitionTime.Value, !skipMusicMarkerBlock.Value);
			}
			Finish();
		}
	}
}
