using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	[Tooltip("Check and respond to the amount of objects in a Trigger that has TrackTriggerObjects attached to the same object.")]
	public class CheckTrackTriggerCount : FsmStateAction
	{
		public FsmOwnerDefault target;

		public FsmInt count;

		[ObjectType(typeof(global::Extensions.IntTest))]
		public FsmEnum test;

		public bool everyFrame;

		[Space]
		public FsmEvent successEvent;

		private TrackTriggerObjects track;

		public override void Reset()
		{
			target = null;
			count = null;
			test = null;
			everyFrame = true;
			successEvent = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if ((bool)safe)
			{
				track = safe.GetComponent<TrackTriggerObjects>();
				if ((bool)track)
				{
					if (!CheckCount())
					{
						if (everyFrame)
						{
							return;
						}
					}
					else
					{
						base.Fsm.Event(successEvent);
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
			if (everyFrame && CheckCount())
			{
				base.Fsm.Event(successEvent);
			}
		}

		public bool CheckCount()
		{
			if ((bool)track)
			{
				return track.InsideCount.Test((global::Extensions.IntTest)(object)test.Value, count.Value);
			}
			return false;
		}
	}
}
