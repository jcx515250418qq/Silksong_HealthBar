namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Sets all element to to a given value of a PlayMaker ArrayList Proxy component")]
	public class ArrayListResetValues : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("The value to reset all the arrayList with")]
		public FsmVar resetValue;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			resetValue = null;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				ResetArrayList();
			}
			Finish();
		}

		public void ResetArrayList()
		{
			if (isProxyValid())
			{
				object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, resetValue);
				for (int i = 0; i < proxy.arrayList.Count; i++)
				{
					proxy.arrayList[i] = valueFromFsmVar;
				}
			}
		}
	}
}
