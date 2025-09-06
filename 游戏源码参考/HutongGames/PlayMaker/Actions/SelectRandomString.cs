namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Select a Random String from an array of Strings.")]
	public class SelectRandomString : FsmStateAction
	{
		[CompoundArray("Strings", "String", "Weight")]
		[Tooltip("A possible String choice.")]
		public FsmString[] strings;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this string being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the chosen String in a String Variable.")]
		public FsmString storeString;

		public override void Reset()
		{
			strings = new FsmString[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			storeString = null;
		}

		public override void OnEnter()
		{
			DoSelectRandomString();
			Finish();
		}

		private void DoSelectRandomString()
		{
			if (strings != null && strings.Length != 0 && storeString != null)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
				if (randomWeightedIndex != -1)
				{
					storeString.Value = strings[randomWeightedIndex].Value;
				}
			}
		}
	}
}
