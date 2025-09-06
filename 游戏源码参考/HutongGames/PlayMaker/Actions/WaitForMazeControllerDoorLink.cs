using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	public class WaitForMazeControllerDoorLink : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreCorrectDoor;

		public FsmEvent DoorsLinkedEvent;

		private MazeController mazeController;

		public override void Reset()
		{
			StoreCorrectDoor = null;
			DoorsLinkedEvent = null;
		}

		public override void OnEnter()
		{
			mazeController = MazeController.NewestInstance;
			if (!(mazeController == null))
			{
				if (mazeController.IsDoorLinkComplete)
				{
					OnDoorsLinked();
				}
				else
				{
					mazeController.DoorsLinked += OnDoorsLinked;
				}
			}
		}

		public override void OnExit()
		{
			if ((bool)mazeController)
			{
				mazeController.DoorsLinked -= OnDoorsLinked;
				mazeController = null;
			}
		}

		private void OnDoorsLinked()
		{
			TransitionPoint transitionPoint = mazeController.EnumerateCorrectDoors().FirstOrDefault();
			StoreCorrectDoor.Value = ((transitionPoint != null) ? transitionPoint.gameObject : null);
			mazeController.DoorsLinked -= OnDoorsLinked;
			mazeController = null;
			base.Fsm.Event(DoorsLinkedEvent);
			Finish();
		}
	}
}
