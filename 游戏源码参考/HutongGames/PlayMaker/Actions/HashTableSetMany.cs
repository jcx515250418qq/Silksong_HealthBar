namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Set key/value pairs to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy)")]
	public class HashTableSetMany : HashTableActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Data")]
		[CompoundArray("Count", "Key", "Value")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key values for that hash set")]
		public FsmString[] keys;

		[Tooltip("The variable to set.")]
		public FsmVar[] variables;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			keys = null;
			variables = null;
		}

		public override void OnEnter()
		{
			if (SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				SetHashTable();
			}
			Finish();
		}

		public void SetHashTable()
		{
			if (isProxyValid())
			{
				for (int i = 0; i < keys.Length; i++)
				{
					proxy.hashTable[keys[i].Value] = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, variables[i]);
				}
			}
		}
	}
}
