using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Stops audio when state exited.")]
	public class AudioPlayInStateConditional : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		public FsmBool activeBool;

		private AudioSource audio;

		private bool didPlay;

		public override void Reset()
		{
			gameObject = null;
			volume = 1f;
			activeBool = null;
		}

		public override void OnEnter()
		{
			didPlay = false;
			if (!activeBool.Value)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				audio = ownerDefaultTarget.GetComponent<AudioSource>();
				if (audio != null)
				{
					if (!audio.isPlaying)
					{
						audio.Play();
					}
					if (!volume.IsNone)
					{
						audio.volume = volume.Value;
					}
				}
			}
			didPlay = true;
		}

		public override void OnExit()
		{
			if (audio != null && didPlay)
			{
				audio.Stop();
			}
		}
	}
}
