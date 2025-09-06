namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to an Integer Variable.")]
	public class IntAdd : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to add to.")]
		public FsmInt intVariable;

		[RequiredField]
		[Tooltip("The value to add.")]
		public FsmInt add;

		[Tooltip("Repeat every frame. NOTE: This operation will NOT be frame rate independent!")]
		public bool everyFrame;

		public override void Reset()
		{
			intVariable = null;
			add = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			intVariable.Value += add.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			intVariable.Value += add.Value;
		}
	}
}
