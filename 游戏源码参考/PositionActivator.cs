using System;
using UnityEngine;
using UnityEngine.Events;

public class PositionActivator : MonoBehaviour
{
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}

	[SerializeField]
	private Transform relativeTo;

	[SerializeField]
	private float positionY;

	[Space]
	public UnityBoolEvent IsNowAbove;

	public UnityEvent IsJustAbove;

	public UnityBoolEvent IsNowNotAbove;

	public UnityEvent IsJustNotAbove;

	private bool wasAbove;

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		float? y = GetRelativePosition().y;
		Gizmos.DrawWireSphere(position.Where(null, y, null), 0.25f);
	}

	private void OnEnable()
	{
		Evaluate(force: true);
	}

	private void Update()
	{
		Evaluate(force: false);
	}

	private void Evaluate(bool force)
	{
		Vector2 relativePosition = GetRelativePosition();
		bool flag = base.transform.position.y > relativePosition.y;
		if (flag != wasAbove || force)
		{
			IsNowAbove.Invoke(flag);
			IsNowNotAbove.Invoke(!flag);
			if (flag)
			{
				IsJustAbove.Invoke();
			}
			else
			{
				IsJustNotAbove.Invoke();
			}
			wasAbove = flag;
		}
	}

	private Vector2 GetRelativePosition()
	{
		Vector2 vector = new Vector2(0f, positionY);
		if (!relativeTo)
		{
			return vector;
		}
		return relativeTo.TransformPoint(vector);
	}
}
