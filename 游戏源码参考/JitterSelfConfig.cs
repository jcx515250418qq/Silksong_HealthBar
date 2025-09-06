using System;
using TeamCherry.SharedUtils;
using UnityEngine;

[Serializable]
public struct JitterSelfConfig
{
	[Tooltip("How often to move per second. Set to 0 for every frame.")]
	public float Frequency;

	public Vector3 AmountMin;

	public Vector3 AmountMax;

	[ModifiableProperty]
	[Conditional("IsInEditMode", true, true, false)]
	public bool UseCameraRenderHooks;

	public MinMaxFloat Delay;

	private bool IsInEditMode()
	{
		return !Application.isPlaying;
	}
}
