using UnityEngine;

public class ContinuousTrigger : MonoBehaviour
{
	[SerializeField]
	private BoxCollider2D childCollider;

	private Vector2 previousPos;

	private Collider2D selfCollider;

	private float selfColliderSize;

	private void Awake()
	{
		selfCollider = GetComponent<Collider2D>();
		selfColliderSize = selfCollider.bounds.size.y;
		childCollider.enabled = false;
	}

	private void OnEnable()
	{
		previousPos = base.transform.position;
		UpdateChildCollider();
	}

	private void FixedUpdate()
	{
		UpdateChildCollider();
		previousPos = base.transform.position;
	}

	private void UpdateChildCollider()
	{
		Vector2 vector = (Vector2)base.transform.position - previousPos;
		float magnitude = vector.magnitude;
		if (magnitude <= Mathf.Epsilon)
		{
			childCollider.enabled = false;
			return;
		}
		float num = 1f / Mathf.Abs(base.transform.lossyScale.y);
		float num2 = magnitude * num;
		childCollider.enabled = true;
		childCollider.size = new Vector2(num2, selfColliderSize * num);
		childCollider.offset = new Vector2(num2 / 2f, 0f);
		childCollider.transform.SetRotation2D(vector.normalized.DirectionToAngle());
	}
}
