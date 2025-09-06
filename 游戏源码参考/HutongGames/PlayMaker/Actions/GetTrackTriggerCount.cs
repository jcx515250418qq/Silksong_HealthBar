using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetTrackTriggerCount : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreCount;

		public bool EveryFrame;

		private TrackTriggerObjects track;

		public override void Reset()
		{
			Target = null;
			StoreCount = null;
			EveryFrame = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				track = safe.GetComponent<TrackTriggerObjects>();
				if ((bool)track)
				{
					StoreCount.Value = track.InsideCount;
					if (EveryFrame)
					{
						return;
					}
				}
				else
				{
					Debug.LogError("Target GameObject does not have a TrackTriggerObjects component attached!", base.Owner);
				}
			}
			Finish();
		}

		public override void OnFixedUpdate()
		{
			StoreCount.Value = track.InsideCount;
		}
	}
}
