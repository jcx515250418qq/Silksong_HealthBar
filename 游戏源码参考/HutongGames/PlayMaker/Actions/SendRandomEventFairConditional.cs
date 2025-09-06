using System;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sends a Random Event picked from an array of Events. Will optionally decrease the probability of the same event firing in succession, making it seem more \"fair\".")]
	public class SendRandomEventFairConditional : FsmStateAction
	{
		[Serializable]
		public class ProbabilityFsmEvent : Probability.ProbabilityBase<FsmEvent>
		{
			public FsmBool Condition = new FsmBool
			{
				UseVariable = true
			};

			public FsmBool TargetCondition = true;

			public FsmEvent SendEvent;

			public override FsmEvent Item => SendEvent;
		}

		public ProbabilityFsmEvent[] Events;

		[ArrayEditor(VariableType.Float, "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray TrackingArray;

		public FsmFloat MissedMultiplier;

		private float[] selfTrackingArray;

		private bool[] conditionArray;

		public override void Reset()
		{
			Events = null;
			TrackingArray = null;
			MissedMultiplier = 2f;
		}

		public override void OnEnter()
		{
			float[] array = (TrackingArray.IsNone ? selfTrackingArray : TrackingArray.floatValues);
			if (array == null || array.Length != Events.Length)
			{
				array = Events.Select((ProbabilityFsmEvent e) => e.Probability).ToArray();
			}
			if (conditionArray == null || conditionArray.Length != Events.Length)
			{
				conditionArray = Events.Select((ProbabilityFsmEvent e) => e.Condition.IsNone || e.Condition.Value == e.TargetCondition.Value).ToArray();
			}
			int chosenIndex;
			FsmEvent randomItemByProbability = Probability.GetRandomItemByProbability<ProbabilityFsmEvent, FsmEvent>(Events, out chosenIndex, array, conditionArray);
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
			if (!TrackingArray.IsNone)
			{
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
