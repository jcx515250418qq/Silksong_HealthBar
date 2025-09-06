using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class OnDamagedHeroEvent : FsmStateAction
	{
		[CheckForComponent(typeof(DamageHero))]
		public FsmOwnerDefault Target;

		public FsmEvent SendEvent;

		private DamageHero subscribed;

		public override void Reset()
		{
			Target = null;
			SendEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			subscribed = safe.GetComponent<DamageHero>();
			subscribed.HeroDamaged += OnDamagedHero;
		}

		public override void OnExit()
		{
			if (!(subscribed == null))
			{
				subscribed.HeroDamaged -= OnDamagedHero;
				subscribed = null;
			}
		}

		private void OnDamagedHero()
		{
			subscribed.HeroDamaged -= OnDamagedHero;
			subscribed = null;
			base.Fsm.Event(SendEvent);
			Finish();
		}
	}
}
