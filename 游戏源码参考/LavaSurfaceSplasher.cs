using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class LavaSurfaceSplasher : MonoBehaviour
{
	[SerializeField]
	private bool recycleOnSplash;

	public UnityEvent onSplash;

	private static Dictionary<GameObject, LavaSurfaceSplasher> splashers = new Dictionary<GameObject, LavaSurfaceSplasher>();

	private void OnEnable()
	{
		splashers[base.gameObject] = this;
	}

	private void OnDisable()
	{
		splashers.Remove(base.gameObject);
	}

	public static bool TryGetSplasher(GameObject go, out LavaSurfaceSplasher splasher)
	{
		return splashers.TryGetValue(go, out splasher);
	}

	public static bool TrySplash(GameObject go)
	{
		if (TryGetSplasher(go, out var splasher))
		{
			splasher.DoSplash();
			return true;
		}
		return false;
	}

	public void DoSplash()
	{
		onSplash?.Invoke();
		if (recycleOnSplash)
		{
			base.gameObject.Recycle();
		}
	}
}
