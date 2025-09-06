using System.Collections.Generic;
using UnityEngine;

public abstract class MutableCollection<T> : Mutable where T : IMutable
{
	[SerializeField]
	private List<T> mutables = new List<T>();

	protected virtual void Awake()
	{
		mutables.RemoveAll((T o) => o == null);
	}

	public override void OnMuteStateChanged(bool muted)
	{
		foreach (T mutable in mutables)
		{
			mutable.SetMute(muted);
		}
	}
}
