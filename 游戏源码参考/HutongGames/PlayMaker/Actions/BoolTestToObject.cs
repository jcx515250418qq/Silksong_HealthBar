namespace HutongGames.PlayMaker.Actions
{
	public class BoolTestToObject : FsmStateAction
	{
		public FsmBool Test;

		public FsmBool ExpectedValue;

		public FsmObject TrueObject;

		public FsmObject FalseObject;

		[UIHint(UIHint.Variable)]
		public FsmVar StoreResult;

		public bool EveryFrame;

		public override void Reset()
		{
			Test = new FsmBool
			{
				UseVariable = true
			};
			ExpectedValue = true;
			TrueObject = null;
			FalseObject = null;
			StoreResult = null;
			EveryFrame = false;
		}

		public override string ErrorCheck()
		{
			TrueObject.ObjectType = StoreResult.ObjectType;
			FalseObject.ObjectType = StoreResult.ObjectType;
			return base.ErrorCheck();
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
			StoreResult.SetValue((Test.Value == ExpectedValue.Value) ? TrueObject.Value : FalseObject.Value);
		}
	}
}
