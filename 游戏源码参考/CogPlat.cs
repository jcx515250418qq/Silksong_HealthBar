using System;
using UnityEngine;
using UnityEngine.Events;

public class CogPlat : MonoBehaviour
{
	[Serializable]
	public class UnityEventBool : UnityEvent<bool>
	{
	}

	[SerializeField]
	private Animator animator;

	private int droppedBool = Animator.StringToHash("Is Dropped");

	private int depressedBool = Animator.StringToHash("Is Depressed");

	[SerializeField]
	[Range(0f, 1f)]
	private float endPoint = 1f;

	[Space]
	[SerializeField]
	private Transform platform;

	[SerializeField]
	private float droppedZ;

	[SerializeField]
	private float raisedZ;

	[SerializeField]
	private Collider2D platformCollider;

	[SerializeField]
	private CollisionEnterEvent depressedCollision;

	[Space]
	[SerializeField]
	private Transform centreMarker;

	[SerializeField]
	private bool movingClockWise;

	[Space]
	public UnityEventBool OnRotationStart;

	private int collisionsEntered;

	private bool landedOnTop;

	private bool hasEnded;

	private void OnEnable()
	{
		if ((bool)depressedCollision)
		{
			depressedCollision.CollisionEntered += OnCollisionEntered;
			depressedCollision.CollisionExited += OnCollisionExited;
		}
	}

	private void OnDisable()
	{
		if ((bool)depressedCollision)
		{
			depressedCollision.CollisionEntered -= OnCollisionEntered;
			depressedCollision.CollisionExited -= OnCollisionExited;
		}
	}

	private void OnCollisionEntered(Collision2D collision)
	{
		collisionsEntered++;
		if (!landedOnTop)
		{
			Collider2D otherCollider = collision.otherCollider;
			if (collision.collider.bounds.min.y >= otherCollider.bounds.max.y)
			{
				landedOnTop = true;
				SetAnimatorBool(depressedBool, value: true);
			}
		}
	}

	private void OnCollisionExited(Collision2D collision)
	{
		collisionsEntered--;
		if (collisionsEntered == 0)
		{
			SetAnimatorBool(depressedBool, value: false);
			landedOnTop = false;
		}
	}

	private void SetAnimatorBool(int hash, bool value)
	{
		if ((bool)animator)
		{
			animator.SetBool(hash, value);
		}
	}

	public void StartRotation()
	{
		if (base.isActiveAndEnabled)
		{
			hasEnded = false;
			SetAnimatorBool(droppedBool, value: true);
			OnPlatDropped();
			Vector2 vector = (centreMarker ? ((Vector2)centreMarker.position) : Vector2.zero);
			float x = vector.x;
			float x2 = base.transform.position.x;
			bool flag = base.transform.position.y > vector.y;
			bool arg = ((x2 == x) ? ((flag && movingClockWise) || (!flag && !movingClockWise)) : ((!(x2 > x)) ? movingClockWise : (!movingClockWise)));
			OnRotationStart.Invoke(arg);
		}
	}

	public void UpdateRotation(float time)
	{
		if (time > endPoint)
		{
			EndRotation();
		}
	}

	public void EndRotation()
	{
		if (base.isActiveAndEnabled && !hasEnded)
		{
			hasEnded = true;
			SetAnimatorBool(droppedBool, value: false);
		}
	}

	public void OnPlatDropped()
	{
		if ((bool)platform)
		{
			platform.SetLocalPositionZ(droppedZ);
		}
		if ((bool)platformCollider)
		{
			platformCollider.enabled = false;
		}
	}

	public void OnPlatRaised()
	{
		if ((bool)platform)
		{
			platform.SetLocalPositionZ(raisedZ);
		}
		if ((bool)platformCollider)
		{
			platformCollider.enabled = true;
		}
	}
}
