using System;
using UnityEngine;

public sealed class VolumeModifier
{
	private float volume;

	public bool IsValid { get; private set; }

	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			float b = Mathf.Clamp01(value);
			if (!Mathf.Approximately(volume, b))
			{
				volume = b;
				this.OnValueChanged?.Invoke();
			}
		}
	}

	public event Action OnValueChanged;

	public VolumeModifier(float initial)
	{
		volume = Mathf.Clamp01(initial);
		IsValid = true;
	}

	public void SetInvalid()
	{
		IsValid = false;
	}
}
