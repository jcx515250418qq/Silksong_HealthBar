using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Each time this action is called it gets the next item from a PlayMaker HashTable Proxy component. \nThis lets you quickly loop through all the children of an object to perform actions on them.\nNOTE: To get to specific item use HashTableGet instead.")]
	public class HashTableGetNext : HashTableActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
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
		public FsmString key;

		[UIHint(UIHint.Variable)]
		public FsmVar result;

		private ArrayList _keys;

		private int nextItemIndex;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			reset = null;
			startIndex = null;
			endIndex = null;
			loopEvent = null;
			finishedEvent = null;
			failureEvent = null;
			result = null;
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
				if (!SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
				{
					base.Fsm.Event(failureEvent);
					Finish();
				}
				_keys = new ArrayList(proxy.hashTable.Keys);
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
			if (nextItemIndex >= _keys.Count)
			{
				nextItemIndex = 0;
				base.Fsm.Event(finishedEvent);
				return;
			}
			GetItemAtIndex();
			if (nextItemIndex >= _keys.Count)
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
			if (isProxyValid())
			{
				object obj = null;
				try
				{
					obj = proxy.hashTable[_keys[nextItemIndex]];
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.Message);
					base.Fsm.Event(failureEvent);
					return;
				}
				key.Value = (string)_keys[nextItemIndex];
				PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, result, obj);
			}
		}
	}
}
