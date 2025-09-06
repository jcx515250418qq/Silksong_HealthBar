using System;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	public class GetPlayerDataVariable : PlayerDataVariableAction
	{
		[RequiredField]
		public FsmString VariableName;

		[UIHint(UIHint.Variable)]
		public FsmVar StoreValue;

		public override void Reset()
		{
			VariableName = null;
			StoreValue = null;
		}

		public override void OnEnter()
		{
			if (!VariableName.IsNone && !StoreValue.IsNone)
			{
				StoreValue.SetValue(PlayerData.instance.GetVariable(VariableName.Value, StoreValue.RealType));
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
			return StoreValue.RealType;
		}
	}
}
