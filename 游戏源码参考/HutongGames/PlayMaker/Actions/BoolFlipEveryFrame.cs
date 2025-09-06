namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Flips the value of a Bool Variable.")]
	public class BoolFlipEveryFrame : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Bool variable to flip.")]
		public FsmBool boolVariable;

		public bool everyFrame;

		public override void Reset()
		{
			boolVariable = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoFlip();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFlip();
		}

		private void DoFlip()
		{
			boolVariable.Value = !boolVariable.Value;
		}
	}
}
