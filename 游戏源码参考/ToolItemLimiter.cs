using System.Collections.Generic;
using GlobalSettings;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class ToolItemLimiter : MonoBehaviour
{
	private sealed class ToolLimiterTracker
	{
		public readonly LinkedList<ToolItemLimiter> ActiveLimiters = new LinkedList<ToolItemLimiter>();

		public void Add(ToolItemLimiter limiter)
		{
			Remove(limiter);
			limiter.activeNode = ActiveLimiters.AddLast(limiter);
		}

		public void Remove(ToolItemLimiter limiter)
		{
			if (limiter.activeNode != null)
			{
				ActiveLimiters.Remove(limiter);
				limiter.activeNode = null;
			}
		}
	}

	[SerializeField]
	private ToolItem representingTool;

	[Space]
	[SerializeField]
	private PlayMakerFSM targetFsm;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string breakEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmBool")]
	private string breakSetBool;

	[Space]
	public UnityEvent OnBreak;

	private float activateTime;

	private static int _lastThrowNum;

	private int throwNum;

	private static readonly HashSet<int> _tempInts = new HashSet<int>();

	private static readonly Dictionary<ToolItem, ToolLimiterTracker> _lookup = new Dictionary<ToolItem, ToolLimiterTracker>();

	private LinkedListNode<ToolItemLimiter> activeNode;

	public static void ClearStatic()
	{
		_tempInts.Clear();
		_lookup.Clear();
	}

	[UsedImplicitly]
	private bool? ValidateFsmEvent(string eventName)
	{
		return targetFsm.IsEventValid(eventName, isRequired: false);
	}

	[UsedImplicitly]
	private bool? ValidateFsmBool(string boolName)
	{
		if (!targetFsm || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return targetFsm.FsmVariables.FindFsmBool(boolName) != null;
	}

	private void OnEnable()
	{
		throwNum = _lastThrowNum;
		RecordSpawn();
	}

	private void OnDisable()
	{
		RemoveNode();
	}

	private void RecordSpawn()
	{
		if (!(representingTool == null))
		{
			if (!_lookup.TryGetValue(representingTool, out var value))
			{
				value = (_lookup[representingTool] = new ToolLimiterTracker());
			}
			value.Add(this);
		}
	}

	private void RemoveNode()
	{
		if (!(representingTool == null) && _lookup.TryGetValue(representingTool, out var value))
		{
			value.Remove(this);
		}
	}

	private void Break()
	{
		BreakInternal();
		if (representingTool == null || !_lookup.TryGetValue(representingTool, out var value))
		{
			return;
		}
		int num = throwNum;
		RemoveNode();
		while (value.ActiveLimiters.Count > 0)
		{
			LinkedListNode<ToolItemLimiter> first = value.ActiveLimiters.First;
			if (first.Value.throwNum == num)
			{
				first.Value.BreakInternal();
				value.ActiveLimiters.RemoveFirst();
				continue;
			}
			break;
		}
	}

	private void BreakInternal()
	{
		OnBreak.Invoke();
		if (!targetFsm)
		{
			return;
		}
		targetFsm.SendEvent(breakEvent);
		if (!string.IsNullOrEmpty(breakSetBool))
		{
			FsmBool fsmBool = targetFsm.FsmVariables.FindFsmBool(breakSetBool);
			if (fsmBool != null)
			{
				fsmBool.Value = true;
			}
		}
	}

	public static void ReportToolUsed(ToolItem thrownTool)
	{
		_lastThrowNum++;
		int num = ((thrownTool.Usage.UseAltForQuickSling && Gameplay.QuickSlingTool.IsEquipped) ? thrownTool.Usage.MaxActiveAlt : thrownTool.Usage.MaxActive);
		if (num <= 0 || !_lookup.TryGetValue(thrownTool, out var value))
		{
			return;
		}
		if (value.ActiveLimiters.Count > 0)
		{
			_tempInts.Add(value.ActiveLimiters.First.Value.throwNum);
			int num2 = value.ActiveLimiters.Last.Value.throwNum;
			_tempInts.Add(num2);
			if (_tempInts.Count >= 2)
			{
				LinkedListNode<ToolItemLimiter> next = value.ActiveLimiters.First.Next;
				while (next != null && next.Value.throwNum != num2)
				{
					_tempInts.Add(next.Value.throwNum);
					next = next.Next;
				}
			}
		}
		int count = _tempInts.Count;
		while (count-- >= num && value.ActiveLimiters.First != null)
		{
			value.ActiveLimiters.First.Value.Break();
		}
		_tempInts.Clear();
	}
}
