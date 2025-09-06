using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CheckPlayerDataTimeLimit : PlayerDataVariableAction
	{
		[RequiredField]
		public FsmString VariableName;

		public FsmEvent AboveEvent;

		public FsmEvent BelowEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreBool;

		public override void Reset()
		{
			VariableName = null;
			AboveEvent = null;
			BelowEvent = null;
			StoreBool = null;
		}

		public override void OnEnter()
		{
			float variable = PlayerData.instance.GetVariable<float>(VariableName.Value);
			bool flag = Time.time >= variable;
			StoreBool.Value = flag;
			base.Fsm.Event(flag ? AboveEvent : BelowEvent);
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
