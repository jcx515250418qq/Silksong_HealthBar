using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
	public class AudioPlaySimpleBool : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		public FsmFloat startTime;

		[ObjectType(typeof(AudioClip))]
		[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		public FsmObject oneShotClip;

		public FsmBool doPlay;

		private AudioSource audio;

		public override void Reset()
		{
			gameObject = null;
			startTime = 0f;
			volume = 1f;
			oneShotClip = null;
			doPlay = true;
		}

		public override void OnEnter()
		{
			if (doPlay.Value)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					audio = ownerDefaultTarget.GetComponent<AudioSource>();
					if (audio != null)
					{
						AudioClip audioClip = oneShotClip.Value as AudioClip;
						if (audioClip == null)
						{
							if (!volume.IsNone)
							{
								audio.volume = volume.Value;
							}
							if (!startTime.IsNone)
							{
								audio.time = startTime.Value;
							}
							if (!audio.isPlaying)
							{
								audio.Play();
							}
						}
						else if (!volume.IsNone)
						{
							audio.PlayOneShot(audioClip, volume.Value);
						}
						else
						{
							audio.PlayOneShot(audioClip);
						}
					}
				}
			}
			Finish();
		}
	}
}
