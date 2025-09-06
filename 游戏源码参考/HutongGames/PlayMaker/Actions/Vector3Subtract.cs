namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Subtracts a Vector3 value from a Vector3 variable.")]
	public class Vector3Subtract : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to subtract from.")]
		public FsmVector3 vector3Variable;

		[RequiredField]
		[Tooltip("The Vector3 to subtract.")]
		public FsmVector3 subtractVector;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			vector3Variable = null;
			subtractVector = new FsmVector3
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			vector3Variable.Value -= subtractVector.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			vector3Variable.Value -= subtractVector.Value;
		}
	}
}
