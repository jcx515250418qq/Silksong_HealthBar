namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class IncrementPlayerDataInt : FsmStateAction
	{
		[RequiredField]
		public FsmString intName;

		public override void Reset()
		{
			intName = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!(instance == null))
			{
				instance.IncrementPlayerDataInt(intName.Value);
				Finish();
			}
		}
	}
}
