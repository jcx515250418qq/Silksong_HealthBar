using System;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	public class PlayerDataVariableTest : PlayerDataVariableAction
	{
		[RequiredField]
		public FsmString VariableName;

		[RequiredField]
		public FsmVar ExpectedValue;

		public FsmEvent IsExpectedEvent;

		public FsmEvent IsNotExpectedEvent;

		public override void Reset()
		{
			VariableName = null;
			ExpectedValue = null;
			IsExpectedEvent = null;
			IsNotExpectedEvent = null;
		}

		public override void OnEnter()
		{
			if (!VariableName.IsNone && !ExpectedValue.IsNone)
			{
				object variable = PlayerData.instance.GetVariable(VariableName.Value, ExpectedValue.RealType);
				base.Fsm.Event((variable != null && variable.Equals(ExpectedValue.GetValue())) ? IsExpectedEvent : IsNotExpectedEvent);
			}
			Finish();
		}

		public override bool GetShouldErrorCheck()
		{
			return !VariableName.UsesVariable;
		}

		public override string GetVariableName()
		{
			return VariableName.Value;
		}

		public override Type GetVariableType()
		{
			return ExpectedValue.RealType;
		}
	}
}
