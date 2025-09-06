using System.Collections.Generic;
using UnityEngine;

public class CogCorpseItem : MonoBehaviour
{
	private Collider2D col;

	private bool wasTrigger;

	private BreakWhenNotMoving breaker;

	[SerializeField]
	private bool trackDisable = true;

	private HashSet<CogCorpseReaction> insideReactions = new HashSet<CogCorpseReaction>();

	public bool IsBroken
	{
		get
		{
			if ((bool)breaker)
			{
				return breaker.IsBroken;
			}
			return false;
		}
	}

	public bool TrackDisable => trackDisable;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
		if ((bool)col)
		{
			wasTrigger = col.isTrigger;
		}
		breaker = GetComponent<BreakWhenNotMoving>();
	}

	private void OnDisable()
	{
		if (!trackDisable)
		{
			return;
		}
		foreach (CogCorpseReaction insideReaction in insideReactions)
		{
			insideReaction.RemoveCorpse(base.transform);
		}
		insideReactions.Clear();
	}

	public void EnteredCogs()
	{
		if ((bool)col)
		{
			col.isTrigger = true;
		}
	}

	public void ExitedCogs()
	{
		if ((bool)col)
		{
			col.isTrigger = wasTrigger;
		}
		if ((bool)breaker)
		{
			breaker.Break();
		}
	}

	public void AddTrackedRegion(CogCorpseReaction cogReaction)
	{
		if (trackDisable)
		{
			insideReactions.Add(cogReaction);
		}
	}

	public void RemoveTrackedRegion(CogCorpseReaction cogReaction)
	{
		if (trackDisable)
		{
			insideReactions.Remove(cogReaction);
		}
	}
}
