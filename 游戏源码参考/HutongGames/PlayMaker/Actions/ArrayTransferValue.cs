using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[NoActionTargets]
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Transfer a value from one array to another, basically a copy/cut paste action on steroids.")]
	public class ArrayTransferValue : FsmStateAction
	{
		public enum ArrayTransferType
		{
			Copy = 0,
			Cut = 1,
			nullify = 2
		}

		public enum ArrayPasteType
		{
			AsFirstItem = 0,
			AsLastItem = 1,
			InsertAtSameIndex = 2,
			ReplaceAtSameIndex = 3
		}

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable source.")]
		public FsmArray arraySource;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable target.")]
		public FsmArray arrayTarget;

		[MatchFieldType("array")]
		[Tooltip("The index to transfer.")]
		public FsmInt indexToTransfer;

		[ActionSection("Transfer Options")]
		[ObjectType(typeof(ArrayTransferType))]
		[Tooltip("Copy Options.")]
		public FsmEnum copyType;

		[ObjectType(typeof(ArrayPasteType))]
		[Tooltip("Paste Options")]
		public FsmEnum pasteType;

		[ActionSection("Result")]
		[Tooltip("Event sent if the array source does not contains that element.")]
		public FsmEvent indexOutOfRange;

		public override void Reset()
		{
			arraySource = null;
			arrayTarget = null;
			indexToTransfer = null;
			copyType = ArrayTransferType.Copy;
			pasteType = ArrayPasteType.AsLastItem;
		}

		public override void OnEnter()
		{
			DoTransferValue();
			Finish();
		}

		private void DoTransferValue()
		{
			if (arraySource.IsNone || arrayTarget.IsNone)
			{
				return;
			}
			int value = indexToTransfer.Value;
			if (value < 0 || value >= arraySource.Length)
			{
				base.Fsm.Event(indexOutOfRange);
				return;
			}
			object obj = arraySource.Values[value];
			if ((ArrayTransferType)(object)copyType.Value == ArrayTransferType.Cut)
			{
				List<object> list = new List<object>(arraySource.Values);
				list.RemoveAt(value);
				arraySource.Values = list.ToArray();
			}
			else if ((ArrayTransferType)(object)copyType.Value == ArrayTransferType.nullify)
			{
				arraySource.Values.SetValue(null, value);
			}
			if ((ArrayPasteType)(object)pasteType.Value == ArrayPasteType.AsFirstItem)
			{
				List<object> list2 = new List<object>(arrayTarget.Values);
				list2.Insert(0, obj);
				arrayTarget.Values = list2.ToArray();
			}
			else if ((ArrayPasteType)(object)pasteType.Value == ArrayPasteType.AsLastItem)
			{
				arrayTarget.Resize(arrayTarget.Length + 1);
				arrayTarget.Set(arrayTarget.Length - 1, obj);
			}
			else if ((ArrayPasteType)(object)pasteType.Value == ArrayPasteType.InsertAtSameIndex)
			{
				if (value >= arrayTarget.Length)
				{
					base.Fsm.Event(indexOutOfRange);
				}
				List<object> list3 = new List<object>(arrayTarget.Values);
				list3.Insert(value, obj);
				arrayTarget.Values = list3.ToArray();
			}
			else if ((ArrayPasteType)(object)pasteType.Value == ArrayPasteType.ReplaceAtSameIndex)
			{
				if (value >= arrayTarget.Length)
				{
					base.Fsm.Event(indexOutOfRange);
				}
				else
				{
					arrayTarget.Set(value, obj);
				}
			}
		}
	}
}
