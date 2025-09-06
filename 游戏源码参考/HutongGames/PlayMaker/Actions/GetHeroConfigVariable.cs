using System;
using System.Text;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	public class GetHeroConfigVariable : FsmStateAction
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
			HeroController instance = HeroController.instance;
			if (!VariableName.IsNone && !StoreValue.IsNone)
			{
				StoreValue.SetValue(instance.Config.GetVariable(VariableName.Value, StoreValue.RealType));
			}
			Finish();
		}

		public string GetVariableName()
		{
			return VariableName.Value;
		}

		public Type GetVariableType()
		{
			return StoreValue.RealType;
		}

		public override string ErrorCheck()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (string.IsNullOrEmpty(GetVariableName()))
			{
				stringBuilder.AppendLine("Variable Name must be specified!");
			}
			if (GetVariableType() == null)
			{
				stringBuilder.AppendLine("Variable type must be specified!");
			}
			else if (!VariableExtensions.VariableExists<HeroControllerConfig>(GetVariableName(), GetVariableType()))
			{
				stringBuilder.AppendLine("Variable of correct type could not be found in HeroControllerConfig");
			}
			return stringBuilder.ToString();
		}
	}
}
