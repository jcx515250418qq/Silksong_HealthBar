namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataBool : FsmStateAction
	{
		[RequiredField]
		public FsmString boolName;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		public override void Reset()
		{
			boolName = null;
			storeValue = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!(instance == null))
			{
				storeValue.Value = instance.GetPlayerDataBool(boolName.Value);
				Finish();
			}
		}
	}
}
