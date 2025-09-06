namespace HutongGames.PlayMaker.Actions
{
	public class StringToUpper : FsmStateAction
	{
		[RequiredField]
		public FsmString Source;

		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmString Destination;

		public bool EveryFrame;

		public override void Reset()
		{
			Source = null;
			Destination = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			Destination.Value = Source.Value.ToUpper();
		}
	}
}
