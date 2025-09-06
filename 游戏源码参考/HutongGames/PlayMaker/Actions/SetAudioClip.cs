using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioClip : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with the AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip to set.")]
		public FsmObject audioClip;

		public FsmBool autoPlay;

		public FsmBool stopOnExit;

		public override void Reset()
		{
			gameObject = null;
			audioClip = null;
			autoPlay = null;
			stopOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				AudioClip clip = base.audio.clip;
				base.audio.clip = audioClip.Value as AudioClip;
				if (autoPlay.Value)
				{
					if (base.audio.clip != clip)
					{
						base.audio.Stop();
						base.audio.time = 0f;
					}
					base.audio.Play();
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (stopOnExit.Value && autoPlay.Value)
			{
				base.audio.Stop();
			}
			base.OnExit();
		}
	}
}
