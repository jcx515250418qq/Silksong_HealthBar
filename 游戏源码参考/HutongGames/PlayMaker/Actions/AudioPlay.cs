using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[ActionTarget(typeof(AudioSource), "gameObject", false)]
	[ActionTarget(typeof(AudioClip), "oneShotClip", false)]
	[Tooltip("Plays the AudioClip defined in an Audio Source component on a GameObject. Set the clip using {{Set Audio Clip}}. Optionally plays a one shot Audio Clip.")]
	public class AudioPlay : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Volume to play the sound at. Can be modified with {{Set Audio Volume}}.")]
		public FsmFloat volume;

		[ObjectType(typeof(AudioClip))]
		[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		public FsmObject oneShotClip;

		[Tooltip("Wait until the end of the clip to send the Finish Event. Set to false to send the finish event immediately.")]
		public FsmBool WaitForEndOfClip;

		[Tooltip("Send this event when the sound is finished playing. NOTE: currently also sent when the sound is paused...")]
		public FsmEvent finishedEvent;

		public override void Reset()
		{
			gameObject = null;
			volume = 1f;
			oneShotClip = null;
			finishedEvent = null;
			WaitForEndOfClip = true;
		}

		public override void OnEnter()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
			}
			else
			{
				if (base.audio == null)
				{
					return;
				}
				AudioClip audioClip = oneShotClip.Value as AudioClip;
				if (audioClip == null)
				{
					base.audio.Play();
					if (!volume.IsNone)
					{
						base.audio.volume = volume.Value;
					}
					if (!WaitForEndOfClip.Value)
					{
						base.Fsm.Event(finishedEvent);
						Finish();
					}
				}
				else
				{
					if (!volume.IsNone)
					{
						base.audio.PlayOneShot(audioClip, volume.Value);
					}
					else
					{
						base.audio.PlayOneShot(audioClip);
					}
					if (!WaitForEndOfClip.Value)
					{
						base.Fsm.Event(finishedEvent);
						Finish();
					}
				}
			}
		}

		public override void OnUpdate()
		{
			if (base.audio == null)
			{
				Finish();
			}
			else if (!base.audio.isPlaying)
			{
				base.Fsm.Event(finishedEvent);
				Finish();
			}
			else if (!volume.IsNone && volume.Value != base.audio.volume)
			{
				base.audio.volume = volume.Value;
			}
		}
	}
}
