using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Randomise the current time of an audiosource. Leave TimeMin and TimeMax at 0 to use clip length.")]
	public class RandomiseAudioPosition : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		public FsmFloat timeMin;

		public FsmFloat timeMax;

		public override void Reset()
		{
			gameObject = null;
			timeMin = 0f;
			timeMax = 0f;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					float time = ((timeMin.Value == 0f && timeMax.Value == 0f) ? Random.Range(0f, component.clip.length) : Random.Range(timeMin.Value, timeMax.Value));
					component.time = time;
				}
			}
			Finish();
		}
	}
}
