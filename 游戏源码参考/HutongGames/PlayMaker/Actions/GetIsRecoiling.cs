using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetIsRecoiling : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmBool storeIsRecoiling;

		public bool everyframe;

		public override void OnEnter()
		{
			DoGetRecoiling();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetRecoiling();
		}

		public void DoGetRecoiling()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if ((bool)gameObject)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				storeIsRecoiling.Value = component.GetIsRecoiling();
			}
		}
	}
}
