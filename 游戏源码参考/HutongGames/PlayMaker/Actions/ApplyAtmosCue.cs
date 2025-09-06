using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[ActionTarget(typeof(AtmosCue), "atmosCue", false)]
	[Tooltip("Plays atmos cues.")]
	public class ApplyAtmosCue : FsmStateAction
	{
		[Tooltip("Atmos cue to play.")]
		[ObjectType(typeof(AtmosCue))]
		public FsmObject atmosCue;

		[Tooltip("Transition duration.")]
		public FsmFloat transitionTime;

		public override void Reset()
		{
			atmosCue = null;
			transitionTime = 0f;
		}

		public override void OnEnter()
		{
			AtmosCue atmosCue = this.atmosCue.Value as AtmosCue;
			GameManager instance = GameManager.instance;
			if (!(atmosCue == null))
			{
				if (instance == null)
				{
					Debug.LogErrorFormat(base.Owner, "Failed to play atmos cue, because the game manager is not ready");
				}
				else
				{
					instance.AudioManager.ApplyAtmosCue(atmosCue, transitionTime.Value);
				}
			}
			Finish();
		}
	}
}
