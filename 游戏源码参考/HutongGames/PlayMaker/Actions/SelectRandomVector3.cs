namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Select a Random Vector3 from a Vector3 array.")]
	public class SelectRandomVector3 : FsmStateAction
	{
		[CompoundArray("Vectors", "Vector", "Weight")]
		[Tooltip("A possible Vector3 choice.")]
		public FsmVector3[] vector3Array;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this Vector3 being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected Vector3 in a Vector3 Variable.")]
		public FsmVector3 storeVector3;

		public override void Reset()
		{
			vector3Array = new FsmVector3[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			storeVector3 = null;
		}

		public override void OnEnter()
		{
			DoSelectRandomColor();
			Finish();
		}

		private void DoSelectRandomColor()
		{
			if (vector3Array != null && vector3Array.Length != 0 && storeVector3 != null)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
				if (randomWeightedIndex != -1)
				{
					storeVector3.Value = vector3Array[randomWeightedIndex].Value;
				}
			}
		}
	}
}
