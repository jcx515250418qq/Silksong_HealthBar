using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Each time this action is called it gets the next item from a PlayMaker ArrayList Proxy component. \nThis lets you quickly loop through all the children of an object to perform actions on them.\nNOTE: To get to specific item use ArrayListGet instead.")]
	public class ArrayListGetNext : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("Set to true to force iterating from the first item. This variable will be set to false as it carries on iterating, force it back to true if you want to renter this action back to the first item.")]
		[UIHint(UIHint.Variable)]
		public FsmBool reset;

		[Tooltip("From where to start iteration, leave to 0 to start from the beginning")]
		public FsmInt startIndex;

		[Tooltip("When to end iteration, leave to 0 to iterate until the end")]
		public FsmInt endIndex;

		[Tooltip("Event to send to get the next item.")]
		public FsmEvent loopEvent;

		[Tooltip("Event to send when there are no more items.")]
		public FsmEvent finishedEvent;

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
		public FsmEvent failureEvent;

		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The current index.")]
		public FsmInt currentIndex;

		[UIHint(UIHint.Variable)]
		[Tooltip("The value for the current index.")]
		public FsmVar result;

		private int nextItemIndex;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			startIndex = null;
			endIndex = null;
			reset = null;
			loopEvent = null;
			finishedEvent = null;
			failureEvent = null;
			result = null;
			currentIndex = null;
		}

		public override void OnEnter()
		{
			if (reset.Value)
			{
				reset.Value = false;
				nextItemIndex = 0;
			}
			if (nextItemIndex == 0)
			{
				if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
				{
					base.Fsm.Event(failureEvent);
					Finish();
				}
				if (startIndex.Value > 0)
				{
					nextItemIndex = startIndex.Value;
				}
			}
			DoGetNextItem();
			Finish();
		}

		private void DoGetNextItem()
		{
			if (nextItemIndex >= proxy.arrayList.Count)
			{
				nextItemIndex = 0;
				base.Fsm.Event(finishedEvent);
				return;
			}
			GetItemAtIndex();
			if (nextItemIndex >= proxy.arrayList.Count)
			{
				nextItemIndex = 0;
				base.Fsm.Event(finishedEvent);
				return;
			}
			if (endIndex.Value > 0 && nextItemIndex >= endIndex.Value)
			{
				nextItemIndex = 0;
				base.Fsm.Event(finishedEvent);
				return;
			}
			nextItemIndex++;
			if (loopEvent != null)
			{
				base.Fsm.Event(loopEvent);
			}
		}

		public void GetItemAtIndex()
		{
			if (isProxyValid() && !result.IsNone)
			{
				object obj = null;
				currentIndex.Value = nextItemIndex;
				try
				{
					obj = proxy.arrayList[nextItemIndex];
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.Message);
					base.Fsm.Event(failureEvent);
					return;
				}
				PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, result, obj);
			}
		}
	}
}
