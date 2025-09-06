using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetPreviousRecoilAngle : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeRecoilAngle;

		public bool everyframe;

		public override void OnEnter()
		{
			DoGetAngle();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetAngle();
		}

		public void DoGetAngle()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if ((bool)gameObject)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				storeRecoilAngle.Value = component.GetPreviousRecoilAngle();
			}
		}
	}
}
