namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Resize an array.")]
	public class ArrayResize : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to resize")]
		public FsmArray array;

		[Tooltip("The new size of the array.")]
		public FsmInt newSize;

		[Tooltip("The event to trigger if the new size is out of range")]
		public FsmEvent sizeOutOfRangeEvent;

		public override void OnEnter()
		{
			if (newSize.Value >= 0)
			{
				array.Resize(newSize.Value);
			}
			else
			{
				LogError("Size out of range: " + newSize.Value);
				base.Fsm.Event(sizeOutOfRangeEvent);
			}
			Finish();
		}
	}
}
