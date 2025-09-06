using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RadialLayoutUI : TransformLayout
{
	[SerializeField]
	private bool ignoreInactive;

	[SerializeField]
	private float scale = 1f;

	[SerializeField]
	private float radius = 1f;

	[SerializeField]
	private bool elementOffset;

	[SerializeField]
	private float splitX;

	[SerializeField]
	private float splitY;

	[SerializeField]
	private bool counterClockwise;

	[SerializeField]
	private bool rotateElements;

	private readonly List<Transform> elements = new List<Transform>();

	public bool ElementOffset
	{
		get
		{
			return elementOffset;
		}
		set
		{
			elementOffset = value;
			UpdatePositions();
		}
	}

	public override void UpdatePositions()
	{
		float num = radius * scale;
		float num2 = splitX * scale;
		float num3 = splitY * scale;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (!ignoreInactive || child.gameObject.activeSelf)
			{
				elements.Add(child);
			}
		}
		float num4 = (elementOffset ? (360f / (float)elements.Count / 2f) : 0f);
		int num5 = 0;
		foreach (Transform element in elements)
		{
			if (element.gameObject.activeSelf || !ignoreInactive)
			{
				float num6 = (float)(num5 + 1) / (float)elements.Count * 360f - num4;
				if (counterClockwise)
				{
					num6 = 360f - num6;
				}
				if (rotateElements)
				{
					element.SetLocalRotation2D(360f - num6);
				}
				num6 *= MathF.PI / 180f;
				Vector3 localPosition = new Vector3(num * Mathf.Sin(num6), num * Mathf.Cos(num6), 0f);
				localPosition.x += ((localPosition.x > 0f) ? num2 : (0f - num2));
				localPosition.y += ((localPosition.y > 0f) ? num3 : (0f - num3));
				element.localPosition = localPosition;
				num5++;
			}
		}
		elements.Clear();
	}
}
