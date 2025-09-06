using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Copy elements from one PlayMaker ArrayList Proxy component to another")]
	public class ArrayListCopyTo : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to copy from")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy from ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to copy to")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObjectTarget;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy to ( necessary if several component coexists on the same GameObject")]
		public FsmString referenceTarget;

		[Tooltip("Optional start index to copy from the source, if not set, starts from the beginning")]
		public FsmInt startIndex;

		[Tooltip("Optional amount of elements to copy, If not set, will copy all from start index.")]
		public FsmInt count;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			gameObjectTarget = null;
			referenceTarget = null;
			startIndex = new FsmInt
			{
				UseVariable = true
			};
			count = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				DoArrayListCopyTo(proxy.arrayList);
			}
			Finish();
		}

		public void DoArrayListCopyTo(ArrayList source)
		{
			if (!isProxyValid() || !SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObjectTarget), referenceTarget.Value) || !isProxyValid())
			{
				return;
			}
			int value = startIndex.Value;
			int num = source.Count;
			int value2 = source.Count;
			if (!count.IsNone)
			{
				value2 = count.Value;
			}
			if (value < 0 || value >= source.Count)
			{
				LogError("start index out of range");
				return;
			}
			if (count.Value < 0)
			{
				LogError("count can not be negative");
				return;
			}
			num = Mathf.Min(value + value2, source.Count);
			for (int i = value; i < num; i++)
			{
				proxy.arrayList.Add(source[i]);
			}
		}
	}
}
