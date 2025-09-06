namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Checks whether a player bool is true and another is false. Sends event.")]
	public class PlayerDataBoolTrueAndFalse : FsmStateAction
	{
		[RequiredField]
		public FsmString trueBool;

		[RequiredField]
		public FsmString falseBool;

		[Tooltip("Event to send if conditions met.")]
		public FsmEvent isTrue;

		[Tooltip("Event to send if conditions not met.")]
		public FsmEvent isFalse;

		public override void Reset()
		{
			trueBool = null;
			falseBool = null;
			isTrue = null;
			isFalse = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!(instance == null))
			{
				if (instance.GetPlayerDataBool(trueBool.Value) && !instance.GetPlayerDataBool(falseBool.Value))
				{
					base.Fsm.Event(isTrue);
				}
				else
				{
					base.Fsm.Event(isFalse);
				}
				Finish();
			}
		}
	}
}
