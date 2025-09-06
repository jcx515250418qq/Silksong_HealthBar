using System;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sends a Random Event picked from an array of Events. Will optionally decrease the probability of the same event firing in succession, making it seem more \"fair\".")]
	public class SendRandomEventFair : FsmStateAction
	{
		[Serializable]
		public class ProbabilityFsmEvent : Probability.ProbabilityBase<FsmEvent>
		{
			public FsmEvent SendEvent;

			public override FsmEvent Item => SendEvent;
		}

		public ProbabilityFsmEvent[] Events;

		[ArrayEditor(VariableType.Float, "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray TrackingArray;

		[HideIf("IsNotUsingTrackingArray")]
		public FsmFloat MissedMultiplier;

		public bool IsNotUsingTrackingArray()
		{
			return TrackingArray.IsNone;
		}

		public override void Reset()
		{
			Events = null;
			TrackingArray = null;
			MissedMultiplier = 2f;
		}

		public override void OnEnter()
		{
			float[] array = null;
			if (!TrackingArray.IsNone)
			{
				array = ((TrackingArray.floatValues.Length == Events.Length) ? TrackingArray.floatValues : Events.Select((ProbabilityFsmEvent e) => e.Probability).ToArray());
			}
			int chosenIndex;
			FsmEvent randomItemByProbability = Probability.GetRandomItemByProbability<ProbabilityFsmEvent, FsmEvent>(Events, out chosenIndex, array);
			if (!TrackingArray.IsNone)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (i == chosenIndex)
					{
						array[i] = Events[i].Probability;
					}
					else
					{
						array[i] *= MissedMultiplier.Value;
					}
				}
				TrackingArray.floatValues = array;
			}
			if (randomItemByProbability != null)
			{
				base.Fsm.Event(randomItemByProbability);
			}
			Finish();
		}
	}
}
