using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

public class ReceivedDamageProxy : MonoBehaviour, IHitResponder
{
	[SerializeField]
	private bool dontReportHit;

	private readonly HashSet<ReceivedDamageBase> handlers = new HashSet<ReceivedDamageBase>();

	private readonly List<ReceivedDamageBase> hitting = new List<ReceivedDamageBase>();

	private int hittingIndex;

	private bool isHitting;

	private bool dirty;

	public void AddHandler(ReceivedDamageBase handler)
	{
		handlers.Add(handler);
	}

	public void RemoveHandler(ReceivedDamageBase handler)
	{
		if (handlers.Remove(handler))
		{
			dirty = true;
		}
	}

	private void OnDisable()
	{
		hitting.Clear();
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (isHitting)
		{
			return IHitResponder.Response.None;
		}
		if (handlers.Count <= 0)
		{
			return IHitResponder.Response.None;
		}
		isHitting = true;
		hitting.AddRange(handlers);
		dirty = false;
		bool flag = false;
		for (int i = 0; i < hitting.Count; i++)
		{
			ReceivedDamageBase receivedDamageBase = hitting[i];
			if ((!dirty || handlers.Contains(receivedDamageBase)) && receivedDamageBase.RespondToHit(damageInstance))
			{
				flag = true;
			}
		}
		hitting.Clear();
		isHitting = false;
		return (flag && !dontReportHit) ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}
}
