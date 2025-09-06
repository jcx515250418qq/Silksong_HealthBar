namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Select a random Int from an array of Ints.")]
	public class SelectRandomInt : FsmStateAction
	{
		[CompoundArray("Ints", "Int", "Weight")]
		[Tooltip("A possible int choice.")]
		public FsmInt[] ints;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this int being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected int in an Int Variable.")]
		public FsmInt storeInt;

		public override void Reset()
		{
			ints = new FsmInt[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			storeInt = null;
		}

		public override void OnEnter()
		{
			DoSelectRandomString();
			Finish();
		}

		private void DoSelectRandomString()
		{
			if (ints != null && ints.Length != 0 && storeInt != null)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
				if (randomWeightedIndex != -1)
				{
					storeInt.Value = ints[randomWeightedIndex].Value;
				}
			}
		}
	}
}
