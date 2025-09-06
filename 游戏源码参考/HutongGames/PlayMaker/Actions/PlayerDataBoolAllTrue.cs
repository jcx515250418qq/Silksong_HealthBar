namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class PlayerDataBoolAllTrue : FsmStateAction
	{
		[RequiredField]
		public FsmString[] stringVariables;

		[Tooltip("Event to send if all the Bool variables are True.")]
		public FsmEvent trueEvent;

		[Tooltip("Event to send if not all the bool variables are true.")]
		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		private bool boolCheck;

		public override void Reset()
		{
			stringVariables = null;
			trueEvent = null;
			falseEvent = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			DoAllTrue();
		}

		private void DoAllTrue()
		{
			GameManager instance = GameManager.instance;
			if (instance == null || stringVariables.Length == 0)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < stringVariables.Length; i++)
			{
				if (!instance.GetPlayerDataBool(stringVariables[i].Value))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				base.Fsm.Event(trueEvent);
			}
			else
			{
				base.Fsm.Event(falseEvent);
			}
			storeResult.Value = flag;
		}
	}
}
