namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the value of a Texture Variable.")]
	public class SetTextureValue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Texture Variable.")]
		public FsmTexture textureVariable;

		[RequiredField]
		[Tooltip("Texture Value.")]
		public FsmTexture textureValue;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			textureVariable = null;
			textureValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			textureVariable.Value = textureValue.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			textureVariable.Value = textureValue.Value;
		}
	}
}
