using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class RandomVector2 : FsmStateAction
	{
		public FsmVector2 Start;

		public FsmVector2 End;

		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreResult;

		public bool EveryFrame;

		public override void Reset()
		{
			Start = null;
			End = null;
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
			Vector2 value = Start.Value;
			Vector2 value2 = End.Value;
			StoreResult.Value = new Vector2(Random.Range(value.x, value2.x), Random.Range(value.y, value2.y));
		}
	}
}
