using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class AddTrackTrigger : FsmStateAction
	{
		public FsmOwnerDefault target;

		public override void Reset()
		{
			target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if ((bool)safe && !safe.GetComponent<TrackTriggerObjects>())
			{
				safe.AddComponent<TrackTriggerObjects>();
			}
			Finish();
		}
	}
}
