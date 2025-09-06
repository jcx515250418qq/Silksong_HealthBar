using PlayMaker.ConditionalExpression;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Checks if the conditional expression Is True or Is False. Stops execution of the game if the assertion fails.\nThis is a useful way to check your assumptions. If you expect a certain value use an Assert to make sure!\nOnly runs in Editor.")]
	[SeeAlso("{{Debugging}}")]
	public class Assert : FsmStateAction, IEvaluatorContext
	{
		public enum AssertType
		{
			IsTrue = 0,
			IsFalse = 1
		}

		[UIHint(UIHint.TextArea)]
		[Tooltip("Enter an expression to evaluate.\n\nExamples:\nhealth <= maxHealth\nlives < 100\n\nHint:Use $(for variable names with spaces)")]
		public FsmString expression;

		[Tooltip("Expected result of the expression.")]
		public AssertType assert;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		private string cachedExpression;

		public CompiledAst Ast { get; set; }

		public string LastErrorMessage { get; set; }

		FsmVar IEvaluatorContext.GetVariable(string name)
		{
			NamedVariable variable = base.Fsm.Variables.GetVariable(name);
			if (variable != null)
			{
				return new FsmVar(variable);
			}
			throw new VariableNotFoundException(name);
		}
	}
}
