using System;
using UnityEngine;

public class TinkRigidbodyForce : MonoBehaviour
{
	[SerializeField]
	private TinkEffect tinkEffect;

	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private Vector2 horizontalHitForce;

	[SerializeField]
	private Vector2 upHitForce;

	private void Awake()
	{
		tinkEffect.HitInDirection += OnHitInDirection;
	}

	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		switch (direction)
		{
		case HitInstance.HitDirection.Left:
			body.linearVelocity = new Vector2(0f - horizontalHitForce.x, horizontalHitForce.y);
			break;
		case HitInstance.HitDirection.Right:
			body.linearVelocity = horizontalHitForce;
			break;
		case HitInstance.HitDirection.Up:
			body.linearVelocity = upHitForce;
			break;
		case HitInstance.HitDirection.Down:
		{
			float num = body.transform.position.x - source.transform.position.x;
			body.linearVelocity = ((num > 0f) ? horizontalHitForce : new Vector2(0f - horizontalHitForce.x, horizontalHitForce.y));
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("direction", direction, null);
		}
	}
}
