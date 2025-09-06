using System;
using UnityEngine;

public class PositionConditions : MonoBehaviour
{
	[Serializable]
	private class Position
	{
		public PlayerDataTest Condition;

		public Vector2 Offset;

		public Vector3 Scale = Vector3.one;
	}

	[SerializeField]
	private Position[] positionsOrdered;

	private bool hasStarted;

	private Vector3 initialLocalPos;

	private void OnDrawGizmosSelected()
	{
		if (positionsOrdered != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Position[] array = positionsOrdered;
			for (int i = 0; i < array.Length; i++)
			{
				Gizmos.DrawWireSphere(array[i].Offset, 0.2f);
			}
		}
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			Evaluate();
		}
	}

	private void Start()
	{
		Evaluate();
	}

	public void Evaluate()
	{
		Transform transform = base.transform;
		if (!hasStarted)
		{
			initialLocalPos = transform.localPosition;
		}
		hasStarted = true;
		Vector2 position = initialLocalPos;
		Vector3 localScale = Vector3.one;
		Position[] array = positionsOrdered;
		foreach (Position position2 in array)
		{
			if (position2.Condition.IsFulfilled)
			{
				position = (Vector2)initialLocalPos + position2.Offset;
				localScale = position2.Scale;
				break;
			}
		}
		transform.SetLocalPosition2D(position);
		transform.localScale = localScale;
	}
}
