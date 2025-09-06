using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetRecoilSpeed : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmFloat storeRecoilSpeed;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			storeRecoilSpeed = new FsmFloat();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if (component != null)
				{
					storeRecoilSpeed.Value = component.RecoilSpeedBase;
				}
			}
			Finish();
		}
	}
}
