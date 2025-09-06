using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Gets an item from a PlayMaker ArrayList Proxy component using a base index and a relative increment. This allows you to move to next or previous items granuraly")]
	public class ArrayListGetRelative : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("The index base to compute the item to get")]
		public FsmInt baseIndex;

		[Tooltip("The incremental value from the base index to get the value from. Overshooting the range will loop back on the list.")]
		public FsmInt increment;

		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmVar result;

		[Tooltip("The index of the result")]
		[UIHint(UIHint.Variable)]
		public FsmInt resultIndex;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			baseIndex = null;
			increment = null;
			result = null;
			resultIndex = null;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				GetItemAtIncrement();
			}
			Finish();
		}

		public void GetItemAtIncrement()
		{
			if (isProxyValid())
			{
				object obj = null;
				if (baseIndex.Value + increment.Value >= 0)
				{
					resultIndex.Value = (baseIndex.Value + increment.Value) % proxy.arrayList.Count;
				}
				else
				{
					resultIndex.Value = proxy.arrayList.Count - Mathf.Abs(baseIndex.Value + increment.Value) % proxy.arrayList.Count;
				}
				obj = proxy.arrayList[resultIndex.Value];
				PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, result, obj);
			}
		}
	}
}
