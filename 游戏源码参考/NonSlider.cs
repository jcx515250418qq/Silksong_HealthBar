using System.Collections.Generic;
using UnityEngine;

public class NonSlider : MonoBehaviour
{
	[SerializeField]
	private bool isActive = true;

	private static readonly Dictionary<GameObject, List<NonSlider>> NON_SLIDERS = new Dictionary<GameObject, List<NonSlider>>();

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
		}
	}

	private void Awake()
	{
		if (!NON_SLIDERS.TryGetValue(base.gameObject, out var value))
		{
			value = new List<NonSlider>();
			NON_SLIDERS.Add(base.gameObject, value);
		}
		value.Add(this);
	}

	private void OnDestroy()
	{
		if (NON_SLIDERS.TryGetValue(base.gameObject, out var value) && value.Remove(this) && value.Count == 0)
		{
			NON_SLIDERS.Remove(base.gameObject);
		}
	}

	public static bool IsNonSlider(Collider2D collider2D)
	{
		return NON_SLIDERS.ContainsKey(collider2D.gameObject);
	}

	public static bool IsNonSlider(GameObject gameObject)
	{
		return NON_SLIDERS.ContainsKey(gameObject);
	}

	public static bool TryGetNonSlider(Collider2D collider2D, out NonSlider nonSlider)
	{
		return TryGetNonSlider(collider2D.gameObject, out nonSlider);
	}

	public static bool TryGetNonSlider(GameObject gameObject, out NonSlider nonSlider)
	{
		if (NON_SLIDERS.TryGetValue(gameObject, out var value) && value.Count > 0)
		{
			nonSlider = value[0];
			return true;
		}
		nonSlider = null;
		return false;
	}
}
