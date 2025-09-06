using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomSingle : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmGameObject gameObject;

		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		public FsmFloat pitchMin = 1f;

		public FsmFloat pitchMax = 2f;

		private AudioSource audio;

		public override void Reset()
		{
			gameObject = null;
			audioClip = null;
			pitchMin = 1f;
			pitchMax = 1f;
		}

		public override void OnEnter()
		{
			DoPlayRandomClip();
			Finish();
		}

		private void DoPlayRandomClip()
		{
			AudioClip clip = audioClip.Value as AudioClip;
			audio = gameObject.Value.GetComponent<AudioSource>();
			float pitch = Random.Range(pitchMin.Value, pitchMax.Value);
			audio.pitch = pitch;
			audio.PlayOneShot(clip);
		}
	}
}
