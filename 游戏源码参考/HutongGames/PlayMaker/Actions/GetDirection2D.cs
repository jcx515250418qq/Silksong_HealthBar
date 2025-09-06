using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetDirection2D : FsmStateAction
	{
		public FsmOwnerDefault From;

		public FsmGameObject To;

		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreVector;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreX;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreY;

		public bool EveryFrame;

		public override void Reset()
		{
			From = null;
			To = null;
			StoreVector = null;
			StoreX = null;
			StoreY = null;
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
			GameObject safe = From.GetSafe(this);
			if ((bool)safe && (bool)To.Value)
			{
				Transform transform = safe.transform;
				Vector2 vector = To.Value.transform.position - transform.position;
				StoreVector.Value = vector.normalized;
				StoreX.Value = Mathf.Sign(vector.x);
				StoreY.Value = Mathf.Sign(vector.y);
			}
		}
	}
}
