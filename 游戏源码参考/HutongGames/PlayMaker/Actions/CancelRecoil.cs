using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class CancelRecoil : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if ((bool)gameObject)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if ((bool)component)
				{
					component.CancelRecoil();
				}
			}
			Finish();
		}
	}
}
