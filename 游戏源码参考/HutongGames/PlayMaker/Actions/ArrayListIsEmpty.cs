namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Check if an ArrayList Proxy component is empty.")]
	public class ArrayListIsEmpty : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Result")]
		[Tooltip("Store in a bool wether it is empty or not")]
		[UIHint(UIHint.Variable)]
		public FsmBool isEmpty;

		[Tooltip("Event sent if this arrayList is empty ")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isEmptyEvent;

		[Tooltip("Event sent if this arrayList is not empty")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isNotEmptyEvent;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			isEmpty = null;
			isNotEmptyEvent = null;
			isEmptyEvent = null;
		}

		public override void OnEnter()
		{
			bool flag = GetArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value, silent: true).arrayList.Count == 0;
			isEmpty.Value = flag;
			if (flag)
			{
				base.Fsm.Event(isEmptyEvent);
			}
			else
			{
				base.Fsm.Event(isNotEmptyEvent);
			}
			Finish();
		}
	}
}
