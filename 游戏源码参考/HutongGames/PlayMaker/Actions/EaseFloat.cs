namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Float")]
	public class EaseFloat : EaseFsmAction
	{
		[RequiredField]
		[Tooltip("The float value to ease from.")]
		public FsmFloat fromValue;

		[RequiredField]
		[Tooltip("The float value to ease to.")]
		public FsmFloat toValue;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Float Variable.")]
		public FsmFloat floatVariable;

		private bool finishInNextStep;

		private bool isSetup;

		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			fromValue = null;
			toValue = null;
			finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (!isSetup)
			{
				isSetup = true;
				fromFloats = new float[1];
				toFloats = new float[1];
				resultFloats = new float[1];
			}
			fromFloats[0] = fromValue.Value;
			toFloats[0] = toValue.Value;
			finishInNextStep = false;
			if (delay.Value <= 0f)
			{
				floatVariable.Value = (reverse.IsNone ? fromValue.Value : (reverse.Value ? toValue.Value : fromValue.Value));
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!floatVariable.IsNone && isRunning)
			{
				floatVariable.Value = resultFloats[0];
			}
			if (finishInNextStep)
			{
				Finish();
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
			}
			if (finishAction && !finishInNextStep)
			{
				if (!floatVariable.IsNone)
				{
					floatVariable.Value = (reverse.IsNone ? toValue.Value : (reverse.Value ? fromValue.Value : toValue.Value));
				}
				finishInNextStep = true;
			}
		}
	}
}
