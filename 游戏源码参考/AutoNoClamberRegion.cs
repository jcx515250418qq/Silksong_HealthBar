using UnityEngine;

public class AutoNoClamberRegion : NoClamberRegion
{
	private Vector2 previousPosition;

	private bool isMoving;

	private bool isInsideState;

	private bool isInsideRegion;

	private void FixedUpdate()
	{
		Vector2 vector = base.transform.position;
		isMoving = (vector - previousPosition).magnitude > Mathf.Epsilon;
		if (isMoving && isInsideState)
		{
			if (!isInsideRegion)
			{
				NoClamberRegion.InsideRegions.Add(this);
				isInsideRegion = true;
			}
		}
		else if (isInsideRegion)
		{
			NoClamberRegion.InsideRegions.Remove(this);
			isInsideRegion = false;
		}
		previousPosition = vector;
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		isInsideState = isInside;
	}
}
