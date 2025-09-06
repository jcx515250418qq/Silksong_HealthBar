namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Takes a PlayMaker HashTable Proxy component snapshot, use action HashTableRevertToSnapShot was used. A Snapshot is taken by default at the beginning for the prefill data")]
	public class HashTableTakeSnapShot : HashTableActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
		}

		public override void OnEnter()
		{
			if (SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				DoHashTableTakeSnapShot();
			}
			Finish();
		}

		public void DoHashTableTakeSnapShot()
		{
			if (isProxyValid())
			{
				proxy.TakeSnapShot();
			}
		}
	}
}
