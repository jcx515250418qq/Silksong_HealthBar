using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetRecoilFreeze : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmBool freeze;

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if ((bool)gameObject)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if ((bool)component)
				{
					component.FreezeInPlace = freeze.Value;
					if (freeze.Value)
					{
						component.CancelRecoil();
					}
				}
			}
			Finish();
		}
	}
}
