using UnityEngine;
using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Transition to an audio snapshot. Easy and fun.")]
	public class TransitionToAudioSnapshot : ComponentAction<AudioSource>
	{
		[ObjectType(typeof(AudioMixerSnapshot))]
		public FsmObject snapshot;

		public FsmFloat transitionTime;

		public override void Reset()
		{
			snapshot = null;
			transitionTime = 1f;
		}

		public override void OnEnter()
		{
			AudioMixerSnapshot audioMixerSnapshot = snapshot.Value as AudioMixerSnapshot;
			if (audioMixerSnapshot != null)
			{
				audioMixerSnapshot.TransitionTo(transitionTime.Value);
			}
			Finish();
		}
	}
}
