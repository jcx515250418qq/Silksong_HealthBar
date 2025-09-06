using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays a Random Audio Clip at a position defined by a Game Object or a Vector3. If a position is defined, it takes priority over the game object. You can set the relative weight of the clips to control how often they are selected.")]
	public class PlayRandomSound : FsmStateAction
	{
		[Tooltip("The GameObject to play the sound.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Use world position instead of GameObject.")]
		public FsmVector3 position;

		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		[ObjectType(typeof(AudioClip))]
		[Tooltip("A possible Audio Clip choice.")]
		public FsmObject[] audioClips;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this sound being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Volume to play the sound at.")]
		public FsmFloat volume = 1f;

		[Tooltip("Don't play the same sound twice in a row")]
		public FsmBool noRepeat;

		private int randomIndex;

		private int lastIndex = -1;

		public override void Reset()
		{
			gameObject = null;
			position = new FsmVector3
			{
				UseVariable = true
			};
			audioClips = new FsmObject[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			volume = 1f;
			noRepeat = false;
		}

		public override void OnEnter()
		{
			DoPlayRandomClip();
			Finish();
		}

		private void DoPlayRandomClip()
		{
			if (audioClips.Length == 0)
			{
				return;
			}
			if (!noRepeat.Value || weights.Length == 1)
			{
				randomIndex = ActionHelpers.GetRandomWeightedIndex(weights);
			}
			else
			{
				do
				{
					randomIndex = ActionHelpers.GetRandomWeightedIndex(weights);
				}
				while (randomIndex == lastIndex && randomIndex != -1);
				lastIndex = randomIndex;
			}
			if (randomIndex == -1)
			{
				return;
			}
			AudioClip audioClip = audioClips[randomIndex].Value as AudioClip;
			if (!(audioClip != null))
			{
				return;
			}
			if (!position.IsNone)
			{
				AudioSource.PlayClipAtPoint(audioClip, position.Value, volume.Value);
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				AudioSource.PlayClipAtPoint(audioClip, ownerDefaultTarget.transform.position, volume.Value);
			}
		}
	}
}
