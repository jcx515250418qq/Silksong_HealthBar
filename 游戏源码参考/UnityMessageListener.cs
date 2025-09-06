using System;
using UnityEngine;

[ExecuteInEditMode]
public class UnityMessageListener : MonoBehaviour
{
	public bool ExecuteInEditMode;

	private bool CanExecute
	{
		get
		{
			if (!Application.isPlaying)
			{
				return ExecuteInEditMode;
			}
			return true;
		}
	}

	public event Action Enabled;

	public event Action Disabled;

	public event Action TransformParentChanged;

	private void OnEnable()
	{
		if (CanExecute)
		{
			this.Enabled?.Invoke();
		}
	}

	private void OnDisable()
	{
		if (CanExecute)
		{
			this.Disabled?.Invoke();
		}
	}

	private void OnTransformParentChanged()
	{
		if (CanExecute)
		{
			this.TransformParentChanged?.Invoke();
		}
	}
}
