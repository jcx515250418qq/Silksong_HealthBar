using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event. Use ints to keep events from being fired x times in a row.")]
	public class SendRandomEventV4 : FsmStateAction
	{
		[CompoundArray("Events", "Event", "Weight")]
		public FsmEvent[] events;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		public FsmInt[] eventMax;

		public FsmInt[] missedMax;

		[UIHint(UIHint.Variable)]
		public FsmBool activeBool;

		private int[] trackingInts;

		private int[] trackingIntsMissed;

		private bool setupArrays;

		private int loops;

		private DelayedEvent delayedEvent;

		public override void Reset()
		{
			events = new FsmEvent[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			if (!activeBool.UseVariable)
			{
				Debug.LogWarning("Encountered broken activeBool in SendRandomEventV4! Fixing automatically.", base.Owner);
				activeBool.UseVariable = true;
			}
			if (activeBool.IsNone || activeBool.Value)
			{
				if (!setupArrays)
				{
					trackingInts = new int[eventMax.Length];
					trackingIntsMissed = new int[missedMax.Length];
					setupArrays = true;
				}
				bool flag = false;
				bool flag2 = false;
				int num = 0;
				while (!flag)
				{
					int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
					if (randomWeightedIndex != -1)
					{
						for (int i = 0; i < trackingIntsMissed.Length; i++)
						{
							if (trackingIntsMissed[i] >= missedMax[i].Value)
							{
								flag2 = true;
								num = i;
							}
						}
						if (flag2)
						{
							flag = true;
							for (int j = 0; j < trackingInts.Length; j++)
							{
								trackingInts[j] = 0;
								trackingIntsMissed[j]++;
							}
							trackingIntsMissed[num] = 0;
							trackingInts[num] = 1;
							loops = 0;
							base.Fsm.Event(events[num]);
						}
						else if (trackingInts[randomWeightedIndex] < eventMax[randomWeightedIndex].Value)
						{
							int num2 = ++trackingInts[randomWeightedIndex];
							for (int k = 0; k < trackingInts.Length; k++)
							{
								trackingInts[k] = 0;
								trackingIntsMissed[k]++;
							}
							trackingInts[randomWeightedIndex] = num2;
							trackingIntsMissed[randomWeightedIndex] = 0;
							flag = true;
							loops = 0;
							base.Fsm.Event(events[randomWeightedIndex]);
						}
					}
					loops++;
					if (loops > 100)
					{
						base.Fsm.Event(events[0]);
						flag = true;
						Finish();
					}
				}
			}
			Finish();
		}
	}
}
