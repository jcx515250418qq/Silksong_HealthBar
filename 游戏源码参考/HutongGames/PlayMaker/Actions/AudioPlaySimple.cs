using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
	public class AudioPlaySimple : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		[ObjectType(typeof(AudioClip))]
		[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		public FsmObject oneShotClip;

		public override void Reset()
		{
			gameObject = null;
			volume = 1f;
			oneShotClip = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					if (oneShotClip.Value is AudioClip clip)
					{
						if (!volume.IsNone)
						{
							component.PlayOneShot(clip, volume.Value);
						}
						else
						{
							component.PlayOneShot(clip);
						}
					}
					else
					{
						if (!component.isPlaying)
						{
							component.Play();
						}
						if (!volume.IsNone)
						{
							component.volume = volume.Value;
						}
					}
				}
			}
			Finish();
		}
	}
}
