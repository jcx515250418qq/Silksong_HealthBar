namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	public class PlayerdataIntCompare : FsmStateAction
	{
		[RequiredField]
		public FsmString playerdataInt;

		[RequiredField]
		public FsmInt compareTo;

		[Tooltip("Event sent if Int 1 equals Int 2")]
		public FsmEvent equal;

		[Tooltip("Event sent if Int 1 is less than Int 2")]
		public FsmEvent lessThan;

		[Tooltip("Event sent if Int 1 is greater than Int 2")]
		public FsmEvent greaterThan;

		public override void Reset()
		{
			playerdataInt = null;
			compareTo = 0;
			equal = null;
			lessThan = null;
			greaterThan = null;
		}

		public override void OnEnter()
		{
			DoIntCompare();
			Finish();
		}

		private void DoIntCompare()
		{
			GameManager instance = GameManager.instance;
			if (!(instance == null))
			{
				int @int = instance.playerData.GetInt(playerdataInt.Value);
				if (@int == compareTo.Value)
				{
					base.Fsm.Event(equal);
				}
				else if (@int < compareTo.Value)
				{
					base.Fsm.Event(lessThan);
				}
				else if (@int > compareTo.Value)
				{
					base.Fsm.Event(greaterThan);
				}
			}
		}

		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(equal) && FsmEvent.IsNullOrEmpty(lessThan) && FsmEvent.IsNullOrEmpty(greaterThan))
			{
				return "Action sends no events!";
			}
			return "";
		}
	}
}
