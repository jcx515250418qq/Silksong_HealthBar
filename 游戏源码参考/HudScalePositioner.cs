using System.Collections.Generic;
using UnityEngine;

public sealed class HudScalePositioner : MonoBehaviour
{
	[SerializeField]
	private Vector3 reducedPosition;

	[SerializeField]
	private Vector3 largePosition;

	private static HashSet<HudScalePositioner> _activeObjs = new HashSet<HudScalePositioner>();

	private static bool _isReduced;

	public static bool IsReduced
	{
		get
		{
			return _isReduced;
		}
		set
		{
			_isReduced = value;
			foreach (HudScalePositioner activeObj in _activeObjs)
			{
				activeObj.UpdatePosition();
			}
		}
	}

	private void OnEnable()
	{
		UpdatePosition();
		_activeObjs.Add(this);
	}

	private void OnDisable()
	{
		_activeObjs.Remove(this);
	}

	private void UpdatePosition()
	{
		if (IsReduced)
		{
			base.transform.localPosition = reducedPosition;
		}
		else
		{
			base.transform.localPosition = largePosition;
		}
	}
}
