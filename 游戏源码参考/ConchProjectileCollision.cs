using System.Collections.Generic;
using UnityEngine;

public sealed class ConchProjectileCollision : MonoBehaviour
{
	[SerializeField]
	private PlayMakerFSM control;

	[SerializeField]
	private Rigidbody2D rigidbody2D;

	[SerializeField]
	private Collider2D collider2D;

	[SerializeField]
	private ProjectileVelocityManager projectileVelocityManager;

	[SerializeField]
	private Vector2 direction;

	[SerializeField]
	private int layer = 8;

	private bool isActive;

	public const float RAYCAST_LENGTH = 0.15f;

	private Collider2D previousCollider;

	private Vector2 previousNormal;

	private List<Vector2> topRays = new List<Vector2>();

	private List<Vector2> rightRays = new List<Vector2>();

	private List<Vector2> bottomRays = new List<Vector2>();

	private List<Vector2> leftRays = new List<Vector2>();

	private int layerMask;

	private void Awake()
	{
		layerMask = 1 << layer;
	}

	private void OnDisable()
	{
		previousCollider = null;
		previousNormal = Vector2.zero;
	}

	private void OnValidate()
	{
		if (projectileVelocityManager == null)
		{
			projectileVelocityManager = GetComponent<ProjectileVelocityManager>();
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (isActive && other.gameObject.layer == layer)
		{
			CheckCollision(checkHit: true);
		}
	}

	private void OnCollisionStay2D(Collision2D other)
	{
		if (isActive && other.gameObject.layer == layer)
		{
			CheckCollision(checkHit: true);
		}
	}

	private void CheckCollision(bool checkHit = false)
	{
		Bounds bounds = collider2D.bounds;
		Vector3 max = bounds.max;
		Vector3 min = bounds.min;
		Vector3 center = bounds.center;
		bool didHit = false;
		if (direction.y > 0f)
		{
			CheckTop();
			if (direction.x > 0f)
			{
				CheckRight();
			}
			else
			{
				CheckLeft();
			}
		}
		else
		{
			CheckBottom();
			if (direction.x > 0f)
			{
				CheckRight();
			}
			else
			{
				CheckLeft();
			}
		}
		Vector2 linearVelocity = rigidbody2D.linearVelocity;
		if (linearVelocity.x == 0f || linearVelocity.y == 0f)
		{
			projectileVelocityManager.DesiredVelocity = projectileVelocityManager.DesiredVelocity;
		}
		void CheckBottom()
		{
			bottomRays.Clear();
			bottomRays.Add(new Vector2(max.x, min.y));
			bottomRays.Add(new Vector2(center.x, min.y));
			bottomRays.Add(min);
			for (int l = 0; l < 3; l++)
			{
				RaycastHit2D raycastHit2D4 = Helper.Raycast2D(bottomRays[l], -Vector2.up, 0.15f, layerMask);
				Collider2D collider4 = raycastHit2D4.collider;
				if (collider4 != null && (!(previousNormal == raycastHit2D4.normal) || !(collider4 == previousCollider)))
				{
					control.SendEvent("FLOOR");
					previousNormal = raycastHit2D4.normal;
					previousCollider = collider4;
					didHit = true;
					break;
				}
			}
		}
		void CheckLeft()
		{
			leftRays.Clear();
			leftRays.Add(min);
			leftRays.Add(new Vector2(min.x, center.y));
			leftRays.Add(new Vector2(min.x, max.y));
			for (int k = 0; k < 3; k++)
			{
				RaycastHit2D raycastHit2D3 = Helper.Raycast2D(leftRays[k], -Vector2.right, 0.15f, layerMask);
				Collider2D collider3 = raycastHit2D3.collider;
				if (collider3 != null && (!(previousNormal == raycastHit2D3.normal) || !(collider3 == previousCollider)))
				{
					control.SendEvent("WALL L");
					previousNormal = raycastHit2D3.normal;
					previousCollider = collider3;
					didHit = true;
					break;
				}
			}
		}
		void CheckRight()
		{
			rightRays.Clear();
			rightRays.Add(max);
			rightRays.Add(new Vector2(max.x, center.y));
			rightRays.Add(new Vector2(max.x, min.y));
			for (int j = 0; j < 3; j++)
			{
				RaycastHit2D raycastHit2D2 = Helper.Raycast2D(rightRays[j], Vector2.right, 0.15f, layerMask);
				Collider2D collider2 = raycastHit2D2.collider;
				if (collider2 != null && (!(previousNormal == raycastHit2D2.normal) || !(collider2 == previousCollider)))
				{
					control.SendEvent("WALL R");
					previousNormal = raycastHit2D2.normal;
					previousCollider = collider2;
					didHit = true;
					break;
				}
			}
		}
		void CheckTop()
		{
			topRays.Clear();
			topRays.Add(new Vector2(min.x, max.y));
			topRays.Add(new Vector2(center.x, max.y));
			topRays.Add(max);
			for (int i = 0; i < 3; i++)
			{
				RaycastHit2D raycastHit2D = Helper.Raycast2D(topRays[i], Vector2.up, 0.15f, layerMask);
				Collider2D collider = raycastHit2D.collider;
				if (!(collider == null) && (!(previousNormal == raycastHit2D.normal) || !(collider == previousCollider)))
				{
					control.SendEvent("ROOF");
					previousNormal = raycastHit2D.normal;
					previousCollider = collider;
					didHit = true;
					break;
				}
			}
		}
	}

	public void SetDirection(Vector2 direction)
	{
		previousNormal = Vector2.zero;
		previousCollider = null;
		this.direction = direction;
		isActive = true;
	}

	public void StateExited()
	{
		isActive = false;
	}
}
