using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NonTinter : MonoBehaviour
{
	[Serializable]
	private class BlockedStates
	{
		public TintFlag blockedTypes = TintFlag.All;

		public bool ShouldTint(TintFlag blockType)
		{
			if (blockedTypes == TintFlag.None)
			{
				return true;
			}
			return (blockType & blockedTypes) == 0;
		}

		public BlockedStates()
			: this(TintFlag.None)
		{
		}

		public BlockedStates(TintFlag blockedTypes)
		{
			this.blockedTypes = blockedTypes;
		}
	}

	[Flags]
	public enum TintFlag
	{
		None = 0,
		CorpseLand = 1,
		All = -1
	}

	[SerializeField]
	private BlockedStates blockedStates = new BlockedStates(TintFlag.All);

	[SerializeField]
	private bool includeChildren;

	private static Dictionary<GameObject, BlockedStates> lookup = new Dictionary<GameObject, BlockedStates>();

	private void Awake()
	{
		lookup[base.gameObject] = blockedStates;
		if (includeChildren)
		{
			tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>(includeInactive: true);
			foreach (tk2dSprite tk2dSprite2 in componentsInChildren)
			{
				lookup[tk2dSprite2.gameObject] = blockedStates;
			}
		}
	}

	public bool ShouldTint(TintFlag blockType)
	{
		return blockedStates.ShouldTint(blockType);
	}

	public static void ClearNonTinters()
	{
		lookup.Clear();
	}

	public static bool CanTint(GameObject gameObject, TintFlag source)
	{
		if (!lookup.TryGetValue(gameObject, out var value))
		{
			NonTinter componentInParent = gameObject.GetComponentInParent<NonTinter>(includeInactive: true);
			value = ((!(componentInParent != null) || (!componentInParent.includeChildren && !(componentInParent.gameObject == gameObject))) ? new BlockedStates(TintFlag.None) : componentInParent.blockedStates);
			lookup[gameObject] = value;
		}
		return value.ShouldTint(source);
	}
}
