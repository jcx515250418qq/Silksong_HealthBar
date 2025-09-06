using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class TakeSilk : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt amount;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			amount = new FsmInt();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				HeroController component = gameObject.GetComponent<HeroController>();
				if (component != null)
				{
					component.TakeSilk(amount.Value);
				}
				Finish();
			}
		}
	}
}
