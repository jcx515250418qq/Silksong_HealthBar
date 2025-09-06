using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetHeroTalkWatchTarget : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[HideIf("HideEndedFacingForwardEvent")]
		public FsmEvent EndedFacingForwardEvent;

		public bool HideEndedFacingForwardEvent()
		{
			return Target.GetSafe(this);
		}

		public override void Reset()
		{
			Target = null;
			EndedFacingForwardEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				HeroTalkAnimation.SetWatchTarget(safe.transform, null);
				Finish();
			}
			else
			{
				HeroTalkAnimation.SetWatchTarget(null, End);
			}
		}

		private void End()
		{
			if (!base.Finished)
			{
				base.Fsm.Event(EndedFacingForwardEvent);
				Finish();
			}
		}
	}
}
