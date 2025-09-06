using System;
using System.Linq;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SelectRandomAudioClipFair : FsmStateAction
	{
		[Serializable]
		public class ProbabilityAudioClip : Probability.ProbabilityBase<AudioClip>
		{
			public AudioClip Clip;

			public override AudioClip Item => Clip;
		}

		public ProbabilityAudioClip[] Clips;

		[ArrayEditor(VariableType.Float, "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray TrackingArray;

		[HideIf("IsNotUsingTrackingArray")]
		public FsmFloat MissedMultiplier;

		[ObjectType(typeof(AudioClip))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreClip;

		public bool IsNotUsingTrackingArray()
		{
			return TrackingArray.IsNone;
		}

		public override void Reset()
		{
			Clips = null;
			TrackingArray = null;
			MissedMultiplier = 2f;
			StoreClip = null;
		}

		public override void OnEnter()
		{
			float[] array = null;
			if (!TrackingArray.IsNone)
			{
				array = ((TrackingArray.floatValues.Length == Clips.Length) ? TrackingArray.floatValues : Clips.Select((ProbabilityAudioClip e) => e.Probability).ToArray());
			}
			int chosenIndex;
			AudioClip randomItemByProbability = Probability.GetRandomItemByProbability<ProbabilityAudioClip, AudioClip>(Clips, out chosenIndex, array);
			if (!TrackingArray.IsNone)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (i == chosenIndex)
					{
						array[i] = Clips[i].Probability;
					}
					else
					{
						array[i] *= MissedMultiplier.Value;
					}
				}
				TrackingArray.floatValues = array;
			}
			StoreClip.Value = randomItemByProbability;
			Finish();
		}
	}
}
