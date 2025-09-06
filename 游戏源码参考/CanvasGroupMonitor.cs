using System;
using UnityEngine;
using UnityEngine.Events;

public class CanvasGroupMonitor : MonoBehaviour
{
	[Serializable]
	public class UnityEventFloat : UnityEvent<float>
	{
	}

	[SerializeField]
	private CanvasGroup group;

	[Space]
	public UnityEventFloat OnAlphaChanged;

	private float previousAlpha = -1f;

	public CanvasGroup Group
	{
		get
		{
			return group;
		}
		set
		{
			group = value;
		}
	}

	private void OnEnable()
	{
		previousAlpha = group.alpha;
		OnAlphaChanged.Invoke(previousAlpha);
	}

	private void Update()
	{
		if ((bool)group && OnAlphaChanged != null && Math.Abs(group.alpha - previousAlpha) > Mathf.Epsilon)
		{
			previousAlpha = group.alpha;
			OnAlphaChanged.Invoke(previousAlpha);
		}
	}
}
