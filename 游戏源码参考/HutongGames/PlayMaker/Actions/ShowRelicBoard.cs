using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ShowRelicBoard : FsmStateAction
	{
		[CheckForComponent(typeof(RelicBoardOwner))]
		[RequiredField]
		public FsmOwnerDefault Target;

		public FsmEvent ClosedEvent;

		private CollectableRelicBoard board;

		public override void Reset()
		{
			Target = null;
			ClosedEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				RelicBoardOwner component = safe.GetComponent<RelicBoardOwner>();
				if ((bool)component)
				{
					board = component.RelicBoard;
					if ((bool)board)
					{
						board.BoardClosed += OnBoardClosed;
						board.OpenBoard(component);
						return;
					}
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if ((bool)board)
			{
				board.CloseBoard();
				board.BoardClosed -= OnBoardClosed;
				board = null;
			}
		}

		private void OnBoardClosed()
		{
			if ((bool)board)
			{
				board.BoardClosed -= OnBoardClosed;
				board = null;
			}
			base.Fsm.Event(ClosedEvent);
		}
	}
}
