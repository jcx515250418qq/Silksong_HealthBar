using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mathos.Parser;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Math expression action. Enter the expression using variable names and common math syntax. Uses Mathos parser.")]
	public class MathExpression : FsmStateAction
	{
		public class Property
		{
			public string path;
		}

		[UIHint(UIHint.TextArea)]
		[Tooltip("Expression to evaluate. Accepts float, int, and bool variable names. Also: Time.deltaTime, ")]
		public FsmString expression;

		[Tooltip("Store the result in a float variable")]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResultAsFloat;

		[Tooltip("Store the result in an int variable")]
		[UIHint(UIHint.Variable)]
		public FsmInt storeResultAsInt;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private MathParser parser;

		private string cachedExpression;

		private ReadOnlyCollection<string> tokens;

		private readonly List<NamedVariable> usedVariables = new List<NamedVariable>();

		public override void Awake()
		{
			parser = new MathParser();
			parser.LocalVariables["Time.deltaTime"] = 0.0;
		}

		public override void OnEnter()
		{
			DoMathExpression();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			parser.LocalVariables["Time.deltaTime"] = Time.deltaTime;
			DoMathExpression();
		}

		private void DoMathExpression()
		{
			double num = ParseExpression();
			if (!storeResultAsFloat.IsNone)
			{
				storeResultAsFloat.Value = (float)num;
			}
			if (!storeResultAsInt.IsNone)
			{
				storeResultAsInt.Value = Mathf.FloorToInt((float)num);
			}
		}

		public double ParseExpression()
		{
			if (expression.Value != cachedExpression)
			{
				BuildAndCacheExpression();
			}
			for (int i = 0; i < usedVariables.Count; i++)
			{
				NamedVariable namedVariable = usedVariables[i];
				switch (namedVariable.VariableType)
				{
				case VariableType.Float:
					parser.LocalVariables[namedVariable.Name] = ((FsmFloat)namedVariable).Value;
					break;
				case VariableType.Int:
					parser.LocalVariables[namedVariable.Name] = ((FsmInt)namedVariable).Value;
					break;
				case VariableType.Bool:
					parser.LocalVariables[namedVariable.Name] = (((FsmBool)namedVariable).Value ? 1 : 0);
					break;
				}
			}
			return parser.Parse(tokens);
		}

		private void BuildAndCacheExpression()
		{
			if (parser == null)
			{
				parser = new MathParser();
			}
			tokens = parser.GetTokens(expression.Value);
			foreach (NamedVariable usedVariable in usedVariables)
			{
				parser.LocalVariables.Remove(usedVariable.Name);
			}
			usedVariables.Clear();
			foreach (string token in tokens)
			{
				NamedVariable namedVariable = base.Fsm.Variables.FindVariable(token) ?? FsmVariables.GlobalVariables.FindVariable(token);
				if (namedVariable != null && !usedVariables.Contains(namedVariable))
				{
					usedVariables.Add(namedVariable);
				}
			}
			cachedExpression = expression.Value;
		}
	}
}
