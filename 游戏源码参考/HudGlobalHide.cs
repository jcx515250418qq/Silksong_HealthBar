using System.Collections.Generic;
using UnityEngine;

public class HudGlobalHide : MonoBehaviour
{
	[SerializeField]
	private Vector2 reducedPos;

	[SerializeField]
	private Vector2 reducedScale;

	private static readonly HashSet<HudGlobalHide> _activeObjs = new HashSet<HudGlobalHide>();

	private static bool _isHidden;

	private static bool _isReduced;

	public static bool IsHidden
	{
		get
		{
			return _isHidden;
		}
		set
		{
			_isHidden = value;
			foreach (HudGlobalHide activeObj in _activeObjs)
			{
				activeObj.UpdateLocation();
			}
		}
	}

	public static bool IsReduced
	{
		get
		{
			return _isReduced;
		}
		set
		{
			_isReduced = value;
			foreach (HudGlobalHide activeObj in _activeObjs)
			{
				activeObj.UpdateLocation();
			}
		}
	}

	private void OnEnable()
	{
		_activeObjs.Add(this);
		UpdateLocation();
	}

	private void OnDisable()
	{
		_activeObjs.Remove(this);
		if (_activeObjs.Count == 0)
		{
			_isHidden = false;
		}
	}

	private void UpdateLocation()
	{
		Transform transform = base.transform;
		if (_isHidden)
		{
			transform.localPosition = new Vector3(0f, -200f, 0f);
		}
		else if (_isReduced)
		{
			transform.localPosition = reducedPos;
			transform.localScale = reducedScale.ToVector3(1f);
		}
		else
		{
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
		}
	}
}
