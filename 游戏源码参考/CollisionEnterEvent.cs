using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEnterEvent : MonoBehaviour
{
	public enum Direction
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}

	public delegate void DirectionalCollisionEvent(Direction direction, Collision2D collision);

	public delegate void CollisionEvent(Collision2D collision);

	public bool checkDirection;

	public bool ignoreTriggers;

	public PhysLayers otherLayer = PhysLayers.TERRAIN;

	[Space]
	public UnityEvent OnCollisionEntered;

	public UnityEvent OnCollisionExited;

	private Collider2D col2d;

	private const float RAYCAST_LENGTH = 0.08f;

	private List<Vector2> topRays = new List<Vector2>(3);

	private List<Vector2> rightRays = new List<Vector2>(3);

	private List<Vector2> bottomRays = new List<Vector2>(3);

	private List<Vector2> leftRays = new List<Vector2>(3);

	public bool DoCollisionStay { get; set; }

	public event DirectionalCollisionEvent CollisionEnteredDirectional;

	public event CollisionEvent CollisionEntered;

	public event CollisionEvent CollisionExited;

	private void Awake()
	{
		col2d = GetComponent<Collider2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.CollisionEntered != null)
		{
			this.CollisionEntered(collision);
		}
		HandleCollision(collision);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (DoCollisionStay)
		{
			HandleCollision(collision);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (this.CollisionExited != null)
		{
			this.CollisionExited(collision);
		}
		if (OnCollisionExited != null)
		{
			OnCollisionExited.Invoke();
		}
	}

	private void HandleCollision(Collision2D collision)
	{
		if (OnCollisionEntered != null)
		{
			OnCollisionEntered.Invoke();
		}
		if (checkDirection)
		{
			CheckTouching((int)otherLayer, collision);
		}
	}

	private void CheckTouching(LayerMask layer, Collision2D collision)
	{
		topRays.Clear();
		topRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.max.y));
		topRays.Add(new Vector2(col2d.bounds.center.x, col2d.bounds.max.y));
		topRays.Add(col2d.bounds.max);
		rightRays.Clear();
		rightRays.Add(col2d.bounds.max);
		rightRays.Add(new Vector2(col2d.bounds.max.x, col2d.bounds.center.y));
		rightRays.Add(new Vector2(col2d.bounds.max.x, col2d.bounds.min.y));
		bottomRays.Clear();
		bottomRays.Add(new Vector2(col2d.bounds.max.x, col2d.bounds.min.y));
		bottomRays.Add(new Vector2(col2d.bounds.center.x, col2d.bounds.min.y));
		bottomRays.Add(col2d.bounds.min);
		leftRays.Clear();
		leftRays.Add(col2d.bounds.min);
		leftRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.center.y));
		leftRays.Add(new Vector2(col2d.bounds.min.x, col2d.bounds.max.y));
		foreach (Vector2 topRay in topRays)
		{
			RaycastHit2D raycastHit2D = Helper.Raycast2D((Vector3)topRay, Vector2.up, 0.08f, 1 << (int)layer);
			if (raycastHit2D.collider != null && (!ignoreTriggers || !raycastHit2D.collider.isTrigger))
			{
				if (this.CollisionEnteredDirectional != null)
				{
					this.CollisionEnteredDirectional(Direction.Top, collision);
				}
				break;
			}
		}
		foreach (Vector2 rightRay in rightRays)
		{
			RaycastHit2D raycastHit2D2 = Helper.Raycast2D((Vector3)rightRay, Vector2.right, 0.08f, 1 << (int)layer);
			if (raycastHit2D2.collider != null && (!ignoreTriggers || !raycastHit2D2.collider.isTrigger))
			{
				if (this.CollisionEnteredDirectional != null)
				{
					this.CollisionEnteredDirectional(Direction.Right, collision);
				}
				break;
			}
		}
		foreach (Vector2 bottomRay in bottomRays)
		{
			RaycastHit2D raycastHit2D3 = Helper.Raycast2D((Vector3)bottomRay, -Vector2.up, 0.08f, 1 << (int)layer);
			if (raycastHit2D3.collider != null && (!ignoreTriggers || !raycastHit2D3.collider.isTrigger))
			{
				if (this.CollisionEnteredDirectional != null)
				{
					this.CollisionEnteredDirectional(Direction.Bottom, collision);
				}
				break;
			}
		}
		foreach (Vector2 leftRay in leftRays)
		{
			RaycastHit2D raycastHit2D4 = Helper.Raycast2D((Vector3)leftRay, -Vector2.right, 0.08f, 1 << (int)layer);
			if (raycastHit2D4.collider != null && (!ignoreTriggers || !raycastHit2D4.collider.isTrigger))
			{
				if (this.CollisionEnteredDirectional != null)
				{
					this.CollisionEnteredDirectional(Direction.Left, collision);
				}
				break;
			}
		}
	}
}
