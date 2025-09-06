namespace HutongGames.PlayMaker.Actions
{
	public class BoolKeepValue : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmBool MonitorBool;

		[UIHint(UIHint.Variable)]
		public FsmBool KeepBool;

		public FsmBool TargetValue;

		public bool EveryFrame;

		public override void Reset()
		{
			MonitorBool = null;
			KeepBool = null;
			TargetValue = null;
			EveryFrame = true;
		}

		public override void OnEnter()
		{
			CheckValues();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			CheckValues();
		}

		private void CheckValues()
		{
			if (MonitorBool.Value == TargetValue.Value)
			{
				KeepBool.Value = TargetValue.Value;
				Finish();
			}
		}
	}
}
