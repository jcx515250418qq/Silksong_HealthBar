namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Check if an ArrayList Proxy component exists.")]
	public class ArrayListExists : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Result")]
		[Tooltip("Store in a bool wether it exists or not")]
		[UIHint(UIHint.Variable)]
		public FsmBool doesExists;

		[Tooltip("Event sent if this arrayList exists ")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doesExistsEvent;

		[Tooltip("Event sent if this arrayList does not exists")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doesNotExistsEvent;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			doesExists = null;
			doesExistsEvent = null;
			doesNotExistsEvent = null;
		}

		public override void OnEnter()
		{
			bool flag = GetArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value, silent: true) != null;
			doesExists.Value = flag;
			if (flag)
			{
				base.Fsm.Event(doesExistsEvent);
			}
			else
			{
				base.Fsm.Event(doesNotExistsEvent);
			}
			Finish();
		}
	}
}
