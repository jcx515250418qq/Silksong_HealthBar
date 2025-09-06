using System;
using UnityEngine;

public class TrackTriggerObjectsLineOfSight : TrackTriggerObjects
{
	private enum LineOfSightChecks
	{
		Self = 0,
		Parent = 1
	}

	[SerializeField]
	private LineOfSightChecks lineOfSightCheck;

	protected override bool IsCounted(GameObject obj)
	{
		Transform transform = base.transform;
		Transform transform2;
		switch (lineOfSightCheck)
		{
		case LineOfSightChecks.Self:
			transform2 = transform;
			break;
		case LineOfSightChecks.Parent:
		{
			Transform parent = transform.parent;
			transform2 = (parent ? parent : transform);
			break;
		}
		default:
			throw new NotImplementedException();
		}
		if (!transform2)
		{
			return false;
		}
		Vector2 vector = transform2.position;
		Vector2 vector2 = (Vector2)obj.transform.position - vector;
		return !Helper.IsRayHittingNoTriggers(vector, vector2.normalized, vector2.magnitude, 256);
	}
}
