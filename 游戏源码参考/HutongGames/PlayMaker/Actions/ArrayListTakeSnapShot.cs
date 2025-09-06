namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Takes a PlayMaker ArrayList Proxy component snapshot, use action ArrayListRevertToSnapShot was used. A Snapshot is taken by default at the beginning for the prefill data")]
	public class ArrayListTakeSnapShot : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				DoArrayListTakeSnapShot();
			}
			Finish();
		}

		public void DoArrayListTakeSnapShot()
		{
			if (isProxyValid())
			{
				proxy.TakeSnapShot();
			}
		}
	}
}
