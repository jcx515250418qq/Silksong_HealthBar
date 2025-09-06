using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class BeginRecoil : FsmStateAction
	{
		public FsmOwnerDefault target;

		public FsmFloat attackDirection;

		public FsmInt attackType;

		public FsmFloat attackMagnitude;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			attackDirection = new FsmFloat();
			attackType = new FsmInt();
			attackMagnitude = new FsmFloat();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if (component != null)
				{
					component.RecoilByDirection(DirectionUtils.GetCardinalDirection(attackDirection.Value), attackMagnitude.Value);
				}
			}
			Finish();
		}
	}
}
