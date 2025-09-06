using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Shuffle elements from an ArrayList Proxy component")]
	public class ArrayListShuffle : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to shuffle")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy from ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("Optional start Index for the shuffling. Leave it to 0 for no effect")]
		public FsmInt startIndex;

		[Tooltip("Optional range for the shuffling, starting at the start index if greater than 0. Leave it to 0 for no effect, that is will shuffle the whole array")]
		public FsmInt shufflingRange;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			startIndex = 0;
			shufflingRange = 0;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				DoArrayListShuffle(proxy.arrayList);
			}
			Finish();
		}

		public void DoArrayListShuffle(ArrayList source)
		{
			if (isProxyValid())
			{
				int num = 0;
				int num2 = proxy.arrayList.Count - 1;
				if (startIndex.Value > 0)
				{
					num = Mathf.Min(startIndex.Value, num2);
				}
				if (shufflingRange.Value > 0)
				{
					num2 = Mathf.Min(proxy.arrayList.Count - 1, num + shufflingRange.Value);
				}
				Debug.Log(num);
				Debug.Log(num2);
				for (int num3 = num2; num3 > num; num3--)
				{
					int index = Random.Range(num, num3 + 1);
					object value = proxy.arrayList[num3];
					proxy.arrayList[num3] = proxy.arrayList[index];
					proxy.arrayList[index] = value;
				}
			}
		}
	}
}
