namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataInt : FsmStateAction
	{
		[RequiredField]
		public FsmString intName;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;

		public override void Reset()
		{
			intName = null;
			storeValue = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!(instance == null))
			{
				storeValue.Value = instance.GetPlayerDataInt(intName.Value);
				Finish();
			}
		}
	}
}
