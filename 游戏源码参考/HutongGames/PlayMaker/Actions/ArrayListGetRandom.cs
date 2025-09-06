using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Gets a random item from a PlayMaker ArrayList Proxy component")]
	public class ArrayListGetRandom : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Result")]
		[Tooltip("The random item data picked from the array")]
		[UIHint(UIHint.Variable)]
		public FsmVar randomItem;

		[Tooltip("The random item index picked from the array")]
		[UIHint(UIHint.Variable)]
		public FsmInt randomIndex;

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
		public FsmEvent failureEvent;

		public override void Reset()
		{
			gameObject = null;
			failureEvent = null;
			randomItem = null;
			randomIndex = null;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				GetRandomItem();
			}
			Finish();
		}

		public void GetRandomItem()
		{
			if (isProxyValid())
			{
				int num = UnityEngine.Random.Range(0, proxy.arrayList.Count);
				object obj = null;
				try
				{
					obj = proxy.arrayList[num];
				}
				catch (Exception ex)
				{
					Debug.LogWarning(ex.Message);
					base.Fsm.Event(failureEvent);
					return;
				}
				randomIndex.Value = num;
				if (!PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, randomItem, obj))
				{
					Debug.LogWarning("ApplyValueToFsmVar failed");
					base.Fsm.Event(failureEvent);
				}
			}
		}
	}
}
