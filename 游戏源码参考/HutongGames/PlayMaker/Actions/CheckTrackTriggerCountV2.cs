using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class CheckTrackTriggerCountV2 : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmInt Count;

		[ObjectType(typeof(global::Extensions.IntTest))]
		public FsmEnum Test;

		public bool EveryFrame;

		[Space]
		[UIHint(UIHint.Variable)]
		public FsmBool SetBool;

		public FsmEvent SuccessEvent;

		public FsmEvent FailEvent;

		private TrackTriggerObjects track;

		public override void Reset()
		{
			Target = null;
			Count = null;
			Test = null;
			EveryFrame = true;
			SetBool = null;
			SuccessEvent = null;
			FailEvent = null;
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
					UpdateState(CheckCount());
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
			UpdateState(CheckCount());
		}

		public bool CheckCount()
		{
			if ((bool)track)
			{
				return track.InsideCount.Test((global::Extensions.IntTest)(object)Test.Value, Count.Value);
			}
			return false;
		}

		private void UpdateState(bool value)
		{
			base.Fsm.Event(value ? SuccessEvent : FailEvent);
			SetBool.Value = value;
		}
	}
}
