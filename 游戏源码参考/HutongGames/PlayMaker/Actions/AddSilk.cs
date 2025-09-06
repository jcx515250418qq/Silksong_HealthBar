using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class AddSilk : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt amount;

		public FsmBool playHeroEffect;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			amount = new FsmInt();
			playHeroEffect = new FsmBool();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				HeroController component = gameObject.GetComponent<HeroController>();
				if (component != null)
				{
					component.AddSilk(amount.Value, playHeroEffect.Value);
				}
				Finish();
			}
		}
	}
}
