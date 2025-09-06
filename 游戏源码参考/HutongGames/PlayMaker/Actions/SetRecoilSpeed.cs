using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetRecoilSpeed : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmFloat newRecoilSpeed;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			newRecoilSpeed = new FsmFloat();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if (component != null)
				{
					component.SetRecoilSpeed(newRecoilSpeed.Value);
				}
			}
			Finish();
		}
	}
}
