using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Black Thread Attack", fileName = "New Attack")]
public class BlackThreadAttack : ScriptableObject
{
	private enum RangeShape
	{
		Rect = 0,
		Circle = 1
	}

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject prefab;

	[SerializeField]
	private float duration;

	[SerializeField]
	private RangeShape rangeShape;

	[SerializeField]
	private Vector2 attackRangeSize;

	[SerializeField]
	private bool counterRotate;

	public GameObject Prefab => prefab;

	public float Duration => duration;

	public bool CounterRotate => counterRotate;

	private void OnValidate()
	{
		if (attackRangeSize.x < 0f)
		{
			attackRangeSize.x = 0f;
		}
		if (attackRangeSize.y < 0f)
		{
			attackRangeSize.y = 0f;
		}
	}

	public void DrawAttackRangeGizmos(Vector3 pos)
	{
		Gizmos.color = new Color(0.3f, 0.1f, 0.1f, 1f);
		switch (rangeShape)
		{
		case RangeShape.Rect:
		{
			Vector2 vector = attackRangeSize;
			if (Mathf.Abs(vector.x) <= Mathf.Epsilon)
			{
				vector.x = 1000f;
			}
			if (Mathf.Abs(vector.y) <= Mathf.Epsilon)
			{
				vector.y = 1000f;
			}
			Gizmos.DrawWireCube(pos, vector);
			break;
		}
		case RangeShape.Circle:
		{
			float num = Mathf.Max(attackRangeSize.x, attackRangeSize.y);
			if (Mathf.Abs(num) > Mathf.Epsilon)
			{
				Gizmos.DrawWireSphere(pos, num);
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public bool IsInRange(Vector2 point, Vector2 refPoint)
	{
		switch (rangeShape)
		{
		case RangeShape.Rect:
		{
			Vector2 vector = attackRangeSize * 0.5f;
			Vector2 vector2 = refPoint - vector;
			Vector2 vector3 = refPoint + vector;
			if (Mathf.Abs(attackRangeSize.x) > Mathf.Epsilon && (point.x < vector2.x || point.x > vector3.x))
			{
				return false;
			}
			if (Mathf.Abs(attackRangeSize.y) > Mathf.Epsilon && (point.y < vector2.y || point.y > vector3.y))
			{
				return false;
			}
			return true;
		}
		case RangeShape.Circle:
		{
			float num = Mathf.Max(attackRangeSize.x, attackRangeSize.y);
			if (Mathf.Abs(num) <= Mathf.Epsilon)
			{
				return false;
			}
			return Vector2.SqrMagnitude(point - refPoint) <= num * num;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
