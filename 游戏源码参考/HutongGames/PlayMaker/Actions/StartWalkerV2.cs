namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Used for starting Walker V2 (NOT a version 2 for controlling original Walker)")]
	public class StartWalkerV2 : FSMUtility.GetComponentFsmStateAction<WalkerV2>
	{
		public bool stopOnExit;

		protected override void DoAction(WalkerV2 walker)
		{
			walker.StartWalking();
		}

		public override void OnExit()
		{
			if (stopOnExit)
			{
				WalkerV2 component = base.Owner.GetComponent<WalkerV2>();
				if (component != null)
				{
					component.StopWalking();
				}
			}
		}
	}
}
