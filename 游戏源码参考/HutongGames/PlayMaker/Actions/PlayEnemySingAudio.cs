using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class PlayEnemySingAudio : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with the AudioSource component.")]
		public FsmOwnerDefault gameObject;

		public RandomAudioClipTable singAudioTable;

		public override void Reset()
		{
			gameObject = null;
			singAudioTable = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				base.audio.clip = singAudioTable.SelectClip(forcePlay: true);
				base.audio.pitch = singAudioTable.SelectPitch();
				base.audio.Stop();
				base.audio.time = Random.Range(0f, 0.2f);
				base.audio.Play();
			}
			Finish();
		}

		public override void OnExit()
		{
			base.audio.Stop();
		}
	}
}
