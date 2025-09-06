using TeamCherry.SharedUtils;
using UnityEngine;

public class ParentAngleEvent : EventBase
{
	[SerializeField]
	private Transform parent;

	[SerializeField]
	private MinMaxFloat angleRange;

	private bool wasInsideRange;

	public override string InspectorInfo => string.Format("From parent \"{0}\"", parent ? parent.name : "null");

	private void Update()
	{
		Vector2 vector = (Vector2)base.transform.position - (Vector2)parent.position;
		float value = Vector2.Angle(Vector2.right, vector.normalized);
		bool flag = angleRange.IsInRange(value);
		if (!wasInsideRange && flag)
		{
			CallReceivedEvent();
		}
		wasInsideRange = flag;
	}
}
