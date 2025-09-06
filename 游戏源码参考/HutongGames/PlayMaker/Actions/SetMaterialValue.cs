namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the value of a Material Variable.")]
	public class SetMaterialValue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Material Variable.")]
		public FsmMaterial materialVariable;

		[RequiredField]
		[Tooltip("Material Value.")]
		public FsmMaterial materialValue;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			materialVariable = null;
			materialValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			materialVariable.Value = materialValue.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			materialVariable.Value = materialValue.Value;
		}
	}
}
