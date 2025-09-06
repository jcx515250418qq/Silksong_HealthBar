using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CheckHeroPerformanceRegionV2 : CheckHeroPerformanceRegionBase
	{
		public FsmOwnerDefault Target;

		public FsmFloat Radius;

		[HideIf("IsOnlyOnEnter")]
		public FsmFloat MinReactDelay;

		[HideIf("IsOnlyOnEnter")]
		public FsmFloat MaxReactDelay;

		public FsmEvent None;

		public FsmEvent ActiveInner;

		public FsmEvent ActiveOuter;

		public FsmBool IgnoreNeedolinRange;

		public FsmBool UseActiveBool;

		[UIHint(UIHint.Variable)]
		public FsmBool ActiveBool;

		[ObjectType(typeof(HeroPerformanceRegion.AffectedState))]
		[UIHint(UIHint.Variable)]
		public FsmEnum StoreState;

		public bool EveryFrame;

		private readonly FsmEvent[] currentEvents = new FsmEvent[2];

		private Transform transform;

		private HeroPerformanceRegion.AffectedState storedState;

		protected override bool IsActive
		{
			get
			{
				if (!ActiveBool.Value)
				{
					return !UseActiveBool.Value;
				}
				return true;
			}
		}

		protected override Transform TargetTransform => transform;

		protected override float TargetRadius => Radius.Value;

		protected override float NewDelay => new MinMaxFloat(MinReactDelay.Value, MaxReactDelay.Value).GetRandomValue();

		protected override bool UseNeedolinRange => !IgnoreNeedolinRange.Value;

		protected override bool IsNoiseResponder
		{
			get
			{
				if (ActiveOuter == null)
				{
					return !StoreState.IsNone;
				}
				return true;
			}
		}

		public bool IsOnlyOnEnter()
		{
			return !EveryFrame;
		}

		public override void Reset()
		{
			Target = null;
			Radius = null;
			MinReactDelay = null;
			MaxReactDelay = null;
			StoreState = null;
			None = null;
			ActiveInner = null;
			ActiveOuter = null;
			IgnoreNeedolinRange = null;
			UseActiveBool = false;
			ActiveBool = null;
			EveryFrame = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			transform = (safe ? safe.transform : null);
			StoreState.Value = HeroPerformanceRegion.AffectedState.None;
			storedState = HeroPerformanceRegion.AffectedState.None;
			base.OnEnter();
			if (!safe)
			{
				Finish();
				return;
			}
			DoAction(EveryFrame);
			if (!EveryFrame)
			{
				Finish();
			}
		}

		protected override void OnAffectedState(HeroPerformanceRegion.AffectedState affectedState)
		{
			switch (affectedState)
			{
			case HeroPerformanceRegion.AffectedState.ActiveInner:
				currentEvents[0] = ActiveInner;
				currentEvents[1] = ActiveOuter;
				break;
			case HeroPerformanceRegion.AffectedState.ActiveOuter:
				currentEvents[0] = ActiveOuter;
				currentEvents[1] = null;
				break;
			default:
				currentEvents[0] = None;
				currentEvents[1] = null;
				break;
			}
		}

		protected override void SendEvents(HeroPerformanceRegion.AffectedState affectedState)
		{
			if (storedState != affectedState)
			{
				StoreState.Value = (storedState = affectedState);
			}
			FsmEvent[] array = currentEvents;
			foreach (FsmEvent fsmEvent in array)
			{
				if (fsmEvent != null)
				{
					base.Fsm.Event(fsmEvent);
				}
			}
		}
	}
}
