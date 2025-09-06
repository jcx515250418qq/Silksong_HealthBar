using System;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	public class SetPlayerDataVariable : PlayerDataVariableAction
	{
		[RequiredField]
		public FsmString VariableName;

		[RequiredField]
		public FsmVar SetValue;

		public override void Reset()
		{
			VariableName = null;
			SetValue = null;
		}

		public override void OnEnter()
		{
			if (!VariableName.IsNone && !SetValue.IsNone)
			{
				SetValue.UpdateValue();
				PlayerData.instance.SetVariable(VariableName.Value, SetValue.GetValue(), SetValue.RealType);
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
			return SetValue.RealType;
		}
	}
}
