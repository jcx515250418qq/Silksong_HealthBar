using System;
using System.Text;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class PlayerDataVariableAction : FsmStateAction
	{
		public abstract bool GetShouldErrorCheck();

		public abstract string GetVariableName();

		public abstract Type GetVariableType();

		public override string ErrorCheck()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (GetShouldErrorCheck() && string.IsNullOrEmpty(GetVariableName()))
			{
				stringBuilder.AppendLine("Variable Name must be specified!");
			}
			if (GetVariableType() == null)
			{
				stringBuilder.AppendLine("Variable type must be specified!");
			}
			else if (GetShouldErrorCheck() && !VariableExtensions.VariableExists<PlayerData>(GetVariableName(), GetVariableType()))
			{
				stringBuilder.AppendLine("Variable of correct type could not be found in PlayerData");
			}
			return stringBuilder.ToString();
		}
	}
}
