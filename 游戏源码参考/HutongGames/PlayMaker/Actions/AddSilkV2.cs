using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public sealed class AddSilkV2 : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt amount;

		public FsmBool playHeroEffect;

		public FsmBool silent;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			amount = new FsmInt();
			playHeroEffect = new FsmBool();
			silent = null;
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (!(gameObject != null))
			{
				return;
			}
			HeroController component = gameObject.GetComponent<HeroController>();
			if (component != null)
			{
				if (silent.Value)
				{
					component.SuppressRefillSound(2);
				}
				component.AddSilk(amount.Value, playHeroEffect.Value);
			}
			Finish();
		}
	}
}
