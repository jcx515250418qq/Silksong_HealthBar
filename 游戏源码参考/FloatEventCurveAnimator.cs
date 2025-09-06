using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class FloatEventCurveAnimator : FloatCurveAnimator
{
	[Serializable]
	public class UnityFloatEvent : UnityEvent<float>
	{
	}

	[SerializeField]
	private float value;

	public UnityFloatEvent OnValueChanged;

	protected override float Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			OnValueChanged.Invoke(value);
		}
	}
}
