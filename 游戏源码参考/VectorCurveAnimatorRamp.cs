using TeamCherry.SharedUtils;
using UnityEngine;

public class VectorCurveAnimatorRamp : RampBase
{
	[Space]
	[SerializeField]
	private VectorCurveAnimator[] curveAnimators;

	[SerializeField]
	private bool getChildren;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("getChildren", true, false, false)]
	private Transform parentOverride;

	private void Awake()
	{
		Transform transform = (parentOverride ? parentOverride : base.transform);
		if (getChildren)
		{
			curveAnimators = transform.GetComponentsInChildren<VectorCurveAnimator>();
		}
	}

	protected override void UpdateValues(float multiplier)
	{
		VectorCurveAnimator[] array = curveAnimators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpeedMultiplier = multiplier;
		}
	}

	protected override void ResetValues()
	{
		VectorCurveAnimator[] array = curveAnimators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpeedMultiplier = 1f;
		}
	}
}
