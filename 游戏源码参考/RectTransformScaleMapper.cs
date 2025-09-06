using System;
using UnityEngine;

public class RectTransformScaleMapper : MonoBehaviour
{
	[SerializeField]
	private RectTransform source;

	[SerializeField]
	private bool overrideInitialSourceRectSize;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideInitialSourceRectSize", true, false, false)]
	private Vector2 initialSourceRectSize;

	[Space]
	[SerializeField]
	private Transform target;

	[SerializeField]
	private bool overrideInitialTargetScale;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideInitialTargetScale", true, false, false)]
	private Vector2 initialTargetScale;

	private Vector2 previousSize;

	private void Start()
	{
		if (!overrideInitialSourceRectSize)
		{
			initialSourceRectSize = source.rect.size;
		}
		if (!overrideInitialTargetScale)
		{
			initialTargetScale = target.localScale;
		}
	}

	private void LateUpdate()
	{
		Vector2 size = source.rect.size;
		if (!(Math.Abs(size.x - previousSize.x) < 0.001f) || !(Math.Abs(size.y - previousSize.y) < 0.001f))
		{
			previousSize = size;
			Vector2 vector = size.DivideElements(initialSourceRectSize);
			Vector3 localScale = target.localScale;
			localScale.x = initialTargetScale.x * vector.x;
			localScale.y = initialTargetScale.y * vector.y;
			target.localScale = localScale;
		}
	}
}
