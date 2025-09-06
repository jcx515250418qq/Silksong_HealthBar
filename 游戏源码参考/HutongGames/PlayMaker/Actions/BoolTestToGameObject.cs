namespace HutongGames.PlayMaker.Actions
{
	public class BoolTestToGameObject : FsmStateAction
	{
		public FsmBool Test;

		public FsmBool ExpectedValue;

		public FsmGameObject TrueGameObject;

		public FsmGameObject FalseGameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreResult;

		public bool EveryFrame;

		public override void Reset()
		{
			Test = null;
			ExpectedValue = true;
			TrueGameObject = null;
			FalseGameObject = null;
			StoreResult = null;
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
			StoreResult.Value = ((Test.Value == ExpectedValue.Value) ? TrueGameObject.Value : FalseGameObject.Value);
		}
	}
}
