using System;
using TMProOld;
using UnityEngine;

[ExecuteInEditMode]
public class TextMeshProLineAnchor : MonoBehaviour
{
	private enum Axis
	{
		Vertical = 0,
		Horizontal = 1
	}

	[SerializeField]
	private TextMeshPro text;

	[SerializeField]
	private Axis axis;

	[SerializeField]
	private float offsetDirection = 1f;

	[SerializeField]
	private float offset;

	private float previous;

	private float previousOffset;

	private void OnValidate()
	{
		if (Math.Abs(offset - previousOffset) > 0.001f)
		{
			previousOffset = offset;
			DoLayout(force: true);
		}
	}

	private void LateUpdate()
	{
		DoLayout(force: false);
	}

	private void DoLayout(bool force)
	{
		if (!text)
		{
			return;
		}
		switch (axis)
		{
		case Axis.Vertical:
		{
			float preferredHeight = text.preferredHeight;
			if (force || Math.Abs(preferredHeight - previous) > 0.001f)
			{
				previous = preferredHeight;
				base.transform.SetPositionY(text.transform.position.y - preferredHeight * offsetDirection + offset);
			}
			break;
		}
		case Axis.Horizontal:
		{
			float preferredWidth = text.preferredWidth;
			if (force || Math.Abs(preferredWidth - previous) > 0.001f)
			{
				previous = preferredWidth;
				base.transform.SetPositionX(text.transform.position.y - preferredWidth * offsetDirection + offset);
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
