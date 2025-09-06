using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetPlayerDataTimeLimit : PlayerDataVariableAction
	{
		[RequiredField]
		public FsmString VariableName;

		[RequiredField]
		public FsmFloat Delay;

		public override void Reset()
		{
			VariableName = null;
			Delay = null;
		}

		public override void OnEnter()
		{
			PlayerData.instance.SetVariable(VariableName.Value, Time.time + Delay.Value);
			Finish();
		}

		public override bool GetShouldErrorCheck()
		{
			return true;
		}

		public override string GetVariableName()
		{
			return VariableName.Value;
		}

		public override Type GetVariableType()
		{
			return typeof(float);
		}
	}
}
