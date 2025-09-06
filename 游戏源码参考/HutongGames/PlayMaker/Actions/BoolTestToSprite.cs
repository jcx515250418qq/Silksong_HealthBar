using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class BoolTestToSprite : FsmStateAction
	{
		public FsmBool Test;

		public FsmBool ExpectedValue;

		[ObjectType(typeof(Sprite))]
		public FsmObject TrueSprite;

		[ObjectType(typeof(Sprite))]
		public FsmObject FalseSprite;

		[ObjectType(typeof(Sprite))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreResult;

		public bool EveryFrame;

		public override void Reset()
		{
			Test = new FsmBool
			{
				UseVariable = true
			};
			ExpectedValue = true;
			TrueSprite = null;
			FalseSprite = null;
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
			StoreResult.Value = ((Test.Value == ExpectedValue.Value) ? TrueSprite.Value : FalseSprite.Value);
		}
	}
}
