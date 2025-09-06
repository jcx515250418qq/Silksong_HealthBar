namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class GetMiddleObjectX : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject objectA;

		[RequiredField]
		public FsmGameObject objectB;

		[RequiredField]
		public FsmGameObject objectC;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeMiddleObject;

		public bool everyFrame;

		public override void Reset()
		{
			objectA = null;
			objectB = null;
			objectC = null;
			storeMiddleObject = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetMiddle();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetMiddle();
		}

		private void DoGetMiddle()
		{
			float x = objectA.Value.transform.position.x;
			float x2 = objectB.Value.transform.position.x;
			float x3 = objectC.Value.transform.position.x;
			if ((x <= x2 && x >= x3) || (x >= x2 && x <= x3))
			{
				storeMiddleObject.Value = objectA.Value;
			}
			else if ((x2 <= x && x2 >= x3) || (x2 >= x && x2 <= x3))
			{
				storeMiddleObject.Value = objectB.Value;
			}
			else
			{
				storeMiddleObject.Value = objectC.Value;
			}
		}
	}
}
