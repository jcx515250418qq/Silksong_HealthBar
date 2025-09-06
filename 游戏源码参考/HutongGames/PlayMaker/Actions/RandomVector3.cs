using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class RandomVector3 : FsmStateAction
	{
		public FsmVector3 Start;

		public FsmVector3 End;

		[UIHint(UIHint.Variable)]
		public FsmVector3 StoreResult;

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
			Vector3 value = Start.Value;
			Vector3 value2 = End.Value;
			StoreResult.Value = new Vector3(Random.Range(value.x, value2.x), Random.Range(value.y, value2.y), Random.Range(value.z, value2.z));
		}
	}
}
