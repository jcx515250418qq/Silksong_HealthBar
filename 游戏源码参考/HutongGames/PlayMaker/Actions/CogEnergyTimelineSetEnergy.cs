using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CogEnergyTimelineSetEnergy : FsmStateAction
	{
		[CheckForComponent(typeof(CogEnergyTimeline))]
		public FsmOwnerDefault Target;

		public FsmFloat Energy;

		public FsmBool Animate;

		public bool EveryFrame;

		private CogEnergyTimeline timeline;

		public override void Reset()
		{
			Target = null;
			Energy = null;
			Animate = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			timeline = safe.GetComponent<CogEnergyTimeline>();
			if (!timeline)
			{
				Finish();
				return;
			}
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			timeline.SetEnergy(Energy.Value, Animate.Value);
		}
	}
}
