using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CrestReportUnlockProgress : FsmStateAction
	{
		public override void OnEnter()
		{
			Debug.LogError("DEPRECATED! Slot unlock XP system has been removed!");
			Finish();
		}
	}
}
