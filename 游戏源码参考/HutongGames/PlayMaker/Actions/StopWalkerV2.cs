namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Used for stopping Walker V2 (NOT a version 2 for controlling original Walker)")]
	public class StopWalkerV2 : FSMUtility.GetComponentFsmStateAction<WalkerV2>
	{
		protected override void DoAction(WalkerV2 walker)
		{
			walker.StopWalking();
		}
	}
}
