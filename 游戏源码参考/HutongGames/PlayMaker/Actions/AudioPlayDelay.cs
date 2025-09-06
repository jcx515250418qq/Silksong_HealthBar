using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
	public class AudioPlayDelay : FsmStateAction
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

		public FsmFloat delay;

		private float timer;

		public override void Reset()
		{
			gameObject = null;
			volume = 1f;
			oneShotClip = null;
			delay = 1f;
		}

		public override void OnEnter()
		{
			timer = 0f;
		}

		public override void OnUpdate()
		{
			if (timer < delay.Value)
			{
				timer += Time.deltaTime;
				return;
			}
			DoPlay();
			Finish();
		}

		public void DoPlay()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget != null))
			{
				return;
			}
			AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
			if (!(component != null))
			{
				return;
			}
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
				return;
			}
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
