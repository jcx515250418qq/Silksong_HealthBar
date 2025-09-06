using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class PositionActivatorRange : MonoBehaviour
{
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}

	[SerializeField]
	private Transform relativeTo;

	[SerializeField]
	private MinMaxFloat range;

	[Space]
	public UnityBoolEvent IsNowInside;

	public UnityEvent IsJustInside;

	public UnityBoolEvent IsNowOutside;

	public UnityEvent IsJustOutside;

	private bool wasInside;

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		float? y = GetRelativeYPosition(range.Start);
		Gizmos.DrawWireSphere(position.Where(null, y, null), 0.25f);
		y = GetRelativeYPosition(range.End);
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
		bool flag = new MinMaxFloat(GetRelativeYPosition(range.Start), GetRelativeYPosition(range.End)).IsInRange(base.transform.position.y);
		if (flag != wasInside || force)
		{
			IsNowInside.Invoke(flag);
			IsNowOutside.Invoke(!flag);
			if (flag)
			{
				IsJustInside.Invoke();
			}
			else
			{
				IsJustOutside.Invoke();
			}
			wasInside = flag;
		}
	}

	private float GetRelativeYPosition(float fromPos)
	{
		Vector2 vector = new Vector2(0f, fromPos);
		Vector2 obj = (relativeTo ? ((Vector2)relativeTo.TransformPoint(vector)) : vector);
		return obj.y;
	}
}
