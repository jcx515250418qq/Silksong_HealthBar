namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Select a Random Vector2 from a Vector2 array.")]
	public class SelectRandomVector2 : FsmStateAction
	{
		[CompoundArray("Vectors", "Vector", "Weight")]
		[Tooltip("A possible Vector2 choice.")]
		public FsmVector2[] vector2Array;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this Vector2 being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected Vector2 in a Vector2 Variable.")]
		public FsmVector2 storeVector2;

		public override void Reset()
		{
			vector2Array = new FsmVector2[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			storeVector2 = null;
		}

		public override void OnEnter()
		{
			DoSelectRandom();
			Finish();
		}

		private void DoSelectRandom()
		{
			if (vector2Array != null && vector2Array.Length != 0 && storeVector2 != null)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
				if (randomWeightedIndex != -1)
				{
					storeVector2.Value = vector2Array[randomWeightedIndex].Value;
				}
			}
		}
	}
}
