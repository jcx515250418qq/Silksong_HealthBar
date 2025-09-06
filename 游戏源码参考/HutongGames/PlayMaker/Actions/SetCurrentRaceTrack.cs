using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetCurrentRaceTrack : FsmStateAction
	{
		[CheckForComponent(typeof(SplineRunner))]
		public FsmOwnerDefault Runner;

		[CheckForComponent(typeof(SprintRaceController))]
		public FsmGameObject Track;

		[Space]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(SavedItem))]
		public FsmObject GetReward;

		public override void Reset()
		{
			Runner = null;
			Track = null;
			GetReward = null;
		}

		public override void OnEnter()
		{
			SplineRunner component = Runner.GetSafe(this).GetComponent<SplineRunner>();
			SprintRaceController sprintRaceController;
			if ((bool)Track.Value)
			{
				sprintRaceController = Track.Value.GetComponent<SprintRaceController>();
				sprintRaceController.transform.parent.gameObject.SetActive(value: true);
			}
			else
			{
				sprintRaceController = null;
			}
			component.SetRaceController(sprintRaceController);
			GetReward.Value = ((sprintRaceController != null) ? sprintRaceController.Reward : null);
			Finish();
		}
	}
}
