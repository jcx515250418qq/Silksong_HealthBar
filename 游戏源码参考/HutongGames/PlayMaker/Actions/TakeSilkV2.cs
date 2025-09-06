using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class TakeSilkV2 : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault Target;

		public FsmInt Amount;

		[ObjectType(typeof(SilkSpool.SilkTakeSource))]
		public FsmEnum TakeSource;

		public override void Reset()
		{
			Target = null;
			Amount = null;
			TakeSource = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				HeroController component = safe.GetComponent<HeroController>();
				if (component != null)
				{
					component.TakeSilk(Amount.Value, (SilkSpool.SilkTakeSource)(object)TakeSource.Value);
				}
			}
			Finish();
		}
	}
}
