using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert)]
	public class ConvertIntToEnum : FsmStateAction
	{
		[RequiredField]
		public FsmInt IntValue;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmEnum EnumVariable;

		public bool EveryFrame;

		public override void Reset()
		{
			IntValue = null;
			EnumVariable = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoConvertIntToEnum();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoConvertIntToEnum();
		}

		private void DoConvertIntToEnum()
		{
			EnumVariable.RawValue = Enum.ToObject(EnumVariable.EnumType, IntValue.Value);
		}
	}
}
