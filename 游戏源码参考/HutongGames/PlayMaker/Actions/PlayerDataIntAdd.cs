namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class PlayerDataIntAdd : FsmStateAction
	{
		[RequiredField]
		public FsmString intName;

		[RequiredField]
		public FsmInt amount;

		public override void Reset()
		{
			intName = null;
			amount = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!(instance == null))
			{
				instance.IntAdd(intName.Value, amount.Value);
				Finish();
			}
		}
	}
}
