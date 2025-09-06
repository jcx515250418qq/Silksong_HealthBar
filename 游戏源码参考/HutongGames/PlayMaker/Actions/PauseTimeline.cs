using UnityEngine;
using UnityEngine.Playables;

namespace HutongGames.PlayMaker.Actions
{
	public class PauseTimeline : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				safe.GetComponent<PlayableDirector>().Pause();
			}
			Finish();
		}
	}
}
