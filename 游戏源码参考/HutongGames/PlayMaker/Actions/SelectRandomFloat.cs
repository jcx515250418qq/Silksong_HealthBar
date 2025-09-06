namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Select a random float from an array of floats.")]
	public class SelectRandomFloat : FsmStateAction
	{
		[CompoundArray("Floats", "Float", "Weight")]
		[Tooltip("A possible float choice.")]
		public FsmFloat[] floats;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this float being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected float in a Float Variable.")]
		public FsmFloat storeFloat;

		public override void Reset()
		{
			floats = new FsmFloat[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			storeFloat = null;
		}

		public override void OnEnter()
		{
			DoSelectRandomString();
			Finish();
		}

		private void DoSelectRandomString()
		{
			if (floats != null && floats.Length != 0 && storeFloat != null)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
				if (randomWeightedIndex != -1)
				{
					storeFloat.Value = floats[randomWeightedIndex].Value;
				}
			}
		}
	}
}
