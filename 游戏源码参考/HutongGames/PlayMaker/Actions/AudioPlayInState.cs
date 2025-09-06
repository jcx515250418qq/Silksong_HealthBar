using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Stops audio when state exited.")]
	public class AudioPlayInState : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		private AudioSource audio;

		public override void Reset()
		{
			gameObject = null;
			volume = 1f;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget != null))
			{
				return;
			}
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

		public override void OnExit()
		{
			if (audio != null)
			{
				audio.Stop();
			}
		}
	}
}
