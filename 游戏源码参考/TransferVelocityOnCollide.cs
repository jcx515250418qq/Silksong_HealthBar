using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class TransferVelocityOnCollide : MonoBehaviour
{
	private enum Types
	{
		Source = 0,
		Target = 1
	}

	[SerializeField]
	private Types type;

	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsSource", true, true, false)]
	private float minMagnitude;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsSource", false, true, false)]
	private TransferVelocityOnCollide linkedSource;

	[Space]
	public UnityEvent OnBounce;

	[Space]
	[SerializeField]
	private float wallBounceMagnitude;

	[SerializeField]
	private float wallRayDistance;

	private Vector2 lastVelocity;

	private int cooldownTicks;

	private bool didLastRayHit;

	private BoxCollider2D box;

	[UsedImplicitly]
	private bool IsSource()
	{
		return type == Types.Source;
	}

	private void Awake()
	{
		box = GetComponent<BoxCollider2D>();
	}

	private void FixedUpdate()
	{
		cooldownTicks--;
		if ((bool)body)
		{
			bool flag = DetectTerrain();
			if (flag && !didLastRayHit)
			{
				Vector2 linearVelocity = -lastVelocity.normalized * wallBounceMagnitude;
				body.linearVelocity = linearVelocity;
				OnBounce.Invoke();
			}
			lastVelocity = body.linearVelocity;
			didLastRayHit = flag;
		}
	}

	private bool DetectTerrain()
	{
		if (Mathf.Abs(wallBounceMagnitude) <= Mathf.Epsilon)
		{
			return false;
		}
		if (Mathf.Abs(lastVelocity.x) <= Mathf.Epsilon)
		{
			return false;
		}
		float distance = wallRayDistance - -0.1f;
		Bounds bounds = box.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		Vector2 direction;
		Vector2 origin;
		Vector2 origin2;
		if (lastVelocity.x > 0f)
		{
			direction = Vector2.right;
			origin = new Vector2(max.x + -0.1f, max.y + 0.001f);
			origin2 = new Vector2(max.x + -0.1f, min.y - 0.001f);
		}
		else
		{
			direction = Vector2.left;
			origin = new Vector2(min.x - -0.1f, max.y + 0.001f);
			origin2 = new Vector2(min.x - -0.1f, min.y - 0.001f);
		}
		RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, direction, distance, 256);
		if ((bool)raycastHit2D.collider && !raycastHit2D.collider.GetComponent<TransferVelocityOnCollide>())
		{
			return true;
		}
		RaycastHit2D raycastHit2D2 = Helper.Raycast2D(origin2, direction, distance, 256);
		if ((bool)raycastHit2D2.collider && !raycastHit2D2.collider.GetComponent<TransferVelocityOnCollide>())
		{
			return true;
		}
		return false;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (type != 0 || cooldownTicks > 0)
		{
			return;
		}
		TransferVelocityOnCollide component = collision.gameObject.GetComponent<TransferVelocityOnCollide>();
		if ((bool)component && component.type == Types.Target && (!body || !(lastVelocity.magnitude <= Mathf.Epsilon)))
		{
			Vector2 vector;
			if ((bool)body)
			{
				vector = lastVelocity.normalized;
				body.linearVelocity = Vector2.zero;
			}
			else
			{
				Vector2 vector2 = component.lastVelocity;
				vector = -vector2.normalized;
			}
			Vector2 linearVelocity = vector * minMagnitude;
			component.body.linearVelocity = linearVelocity;
			if ((bool)component.linkedSource)
			{
				component.linkedSource.cooldownTicks = 2;
			}
			component.OnBounce.Invoke();
		}
	}
}
