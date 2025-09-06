using System;

namespace HutongGames.PlayMaker.Actions
{
	public class EnumBitmaskSwitch : FsmStateAction
	{
		[Serializable]
		public class SwitchItem
		{
			[MatchFieldType("EnumVariable")]
			public FsmEnum[] Values;

			public FsmEvent SendEvent;
		}

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmEnum EnumVariable;

		public SwitchItem[] Switches;

		public bool EveryFrame;

		public override void Reset()
		{
			EnumVariable = null;
			Switches = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoEnumSwitch();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoEnumSwitch();
		}

		private void DoEnumSwitch()
		{
			if (EnumVariable.IsNone)
			{
				return;
			}
			int num = (int)EnumVariable.RawValue;
			SwitchItem[] switches = Switches;
			foreach (SwitchItem switchItem in switches)
			{
				int num2 = 0;
				FsmEnum[] values = switchItem.Values;
				foreach (FsmEnum fsmEnum in values)
				{
					num2 |= (int)fsmEnum.RawValue;
				}
				if ((num & num2) == num2)
				{
					base.Fsm.Event(switchItem.SendEvent);
					break;
				}
			}
		}
	}
}
