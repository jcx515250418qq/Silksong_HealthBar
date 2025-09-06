using UnityEngine;

public class ColliderExpandTo : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private BoxCollider2D expandCollider;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private BoxCollider2D toCollider;

	private void Reset()
	{
		expandCollider = GetComponent<BoxCollider2D>();
	}

	private void Awake()
	{
		Bounds bounds = expandCollider.bounds;
		Bounds bounds2 = toCollider.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		Vector3 min2 = bounds2.min;
		Vector3 max2 = bounds2.max;
		if (min2.x < min.x)
		{
			min.x = min2.x;
		}
		if (min2.y < min.y)
		{
			min.y = min2.y;
		}
		if (max2.x > max.x)
		{
			max.x = max2.x;
		}
		if (max2.y > max.y)
		{
			max.y = max2.y;
		}
		bounds.SetMinMax(min, max);
		Transform transform = expandCollider.transform;
		expandCollider.offset = transform.InverseTransformPoint(bounds.center);
		expandCollider.size = ((Vector2)transform.InverseTransformVector(bounds.size)).Abs();
	}
}
