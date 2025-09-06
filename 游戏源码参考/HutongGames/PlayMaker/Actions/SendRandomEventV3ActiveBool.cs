namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event. Use ints to keep events from being fired x times in a row.")]
	public class SendRandomEventV3ActiveBool : FsmStateAction
	{
		[CompoundArray("Events", "Event", "Weight")]
		public FsmEvent[] events;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		[UIHint(UIHint.Variable)]
		public FsmInt[] trackingInts;

		public FsmInt[] eventMax;

		[UIHint(UIHint.Variable)]
		public FsmInt[] trackingIntsMissed;

		public FsmInt[] missedMax;

		[UIHint(UIHint.Variable)]
		public FsmBool activeBool;

		private int loops;

		private DelayedEvent delayedEvent;

		public override void Reset()
		{
			events = new FsmEvent[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
		}

		public override void OnEnter()
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			if (activeBool.IsNone || activeBool.Value)
			{
				while (!flag)
				{
					int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
					if (randomWeightedIndex != -1)
					{
						for (int i = 0; i < trackingIntsMissed.Length; i++)
						{
							if (trackingIntsMissed[i].Value >= missedMax[i].Value)
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
								trackingInts[j].Value = 0;
								trackingIntsMissed[j].Value++;
							}
							trackingIntsMissed[num].Value = 0;
							trackingInts[num].Value = 1;
							loops = 0;
							base.Fsm.Event(events[num]);
						}
						else if (trackingInts[randomWeightedIndex].Value < eventMax[randomWeightedIndex].Value)
						{
							int value = ++trackingInts[randomWeightedIndex].Value;
							for (int k = 0; k < trackingInts.Length; k++)
							{
								trackingInts[k].Value = 0;
								trackingIntsMissed[k].Value++;
							}
							trackingInts[randomWeightedIndex].Value = value;
							trackingIntsMissed[randomWeightedIndex].Value = 0;
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
