namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Remove all items from an Array.")]
	public class ArrayRemoveAll : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to remove all items from.")]
		public FsmArray array;

		public override void Reset()
		{
			array = null;
		}

		public override void OnEnter()
		{
			array.Reset();
			Finish();
		}
	}
}
