using System;
using UnityEngine;

public class SetPosConditional : MonoBehaviour
{
	[Serializable]
	private class ConditionPos
	{
		public Vector2 Position;

		public PlayerDataTest Condition;
	}

	[SerializeField]
	private ConditionPos[] orderedConditions;

	private Vector2 initialPos;

	private void Awake()
	{
		initialPos = base.transform.localPosition;
	}

	private void OnEnable()
	{
		bool flag = false;
		ConditionPos[] array = orderedConditions;
		foreach (ConditionPos conditionPos in array)
		{
			if (conditionPos.Condition.IsFulfilled)
			{
				base.transform.SetLocalPosition2D(conditionPos.Position);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			base.transform.SetLocalPosition2D(initialPos);
		}
	}
}
