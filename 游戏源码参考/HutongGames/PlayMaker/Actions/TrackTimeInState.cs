using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	public class TrackTimeInState : FsmStateAction
	{
		private double enterTime;

		public override void OnEnter()
		{
			enterTime = Time.timeAsDouble;
		}

		public override void OnExit()
		{
			_ = Time.timeAsDouble;
			_ = enterTime;
		}
	}
}
