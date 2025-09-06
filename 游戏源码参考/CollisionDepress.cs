using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class CollisionDepress : DebugDrawColliderRuntimeAdder
{
	[Serializable]
	public class Rotator
	{
		public Transform Target;

		public float Offset;

		[NonSerialized]
		public float InitialRotation;
	}

	[SerializeField]
	private LayerMask layerMask;

	[SerializeField]
	private Transform childToMove;

	[SerializeField]
	private float depressDistance;

	[SerializeField]
	private bool useTrigger;

	[Space]
	[SerializeField]
	private UnityEvent onDepress;

	[SerializeField]
	private UnityEvent onRise;

	[Space]
	[SerializeField]
	private Rotator[] rotators;

	private Vector3 initialPosition;

	private int depressReturnStepsLeft;

	private readonly List<Collider2D> collided = new List<Collider2D>();

	private bool isDepressed;

	private bool isDisabled;

	private BoxCollider2D collider;

	protected override void Awake()
	{
		base.Awake();
		collider = GetComponent<BoxCollider2D>();
		if ((bool)childToMove)
		{
			initialPosition = childToMove.localPosition;
		}
	}

	private void OnEnable()
	{
		Rotator[] array = rotators;
		foreach (Rotator rotator in array)
		{
			if ((bool)rotator.Target)
			{
				rotator.InitialRotation = rotator.Target.localEulerAngles.z;
			}
		}
	}

	private void OnDisable()
	{
		if (isDepressed)
		{
			SetNotDepressed();
		}
		collided.Clear();
	}

	private void FixedUpdate()
	{
		if (!isDepressed && collided.Count > 0)
		{
			foreach (Collider2D item in collided)
			{
				if (CanColliderDepress(item))
				{
					SetDepressed();
				}
			}
		}
		if (depressReturnStepsLeft > 0)
		{
			depressReturnStepsLeft--;
			if (depressReturnStepsLeft <= 0)
			{
				SetNotDepressed();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!useTrigger)
		{
			HandleEnter(collision.collider);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (!useTrigger)
		{
			HandleExit(collision.collider);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (useTrigger)
		{
			HandleEnter(other);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (useTrigger)
		{
			HandleExit(other);
		}
	}

	private void HandleEnter(Collider2D other)
	{
		if (((1 << other.gameObject.layer) & (int)layerMask) == 0)
		{
			return;
		}
		collided.AddIfNotPresent(other);
		if (CanColliderDepress(other))
		{
			depressReturnStepsLeft = 0;
			if (!isDepressed)
			{
				SetDepressed();
			}
		}
	}

	private void HandleExit(Collider2D other)
	{
		if (collided.Count != 0)
		{
			collided.Remove(other);
			if (isDepressed && collided.Count == 0)
			{
				depressReturnStepsLeft = 5;
			}
		}
	}

	private bool CanColliderDepress(Collider2D otherCollider)
	{
		if (isDisabled)
		{
			return false;
		}
		if (useTrigger)
		{
			return true;
		}
		float y = collider.bounds.max.y;
		return otherCollider.bounds.min.y >= y;
	}

	private void SetDepressed()
	{
		isDepressed = true;
		if ((bool)childToMove)
		{
			childToMove.localPosition = initialPosition + new Vector3(0f, 0f - depressDistance, 0f);
		}
		onDepress.Invoke();
		Rotator[] array = rotators;
		foreach (Rotator rotator in array)
		{
			if ((bool)rotator.Target)
			{
				rotator.Target.SetLocalRotation2D(rotator.InitialRotation + rotator.Offset);
			}
		}
	}

	private void SetNotDepressed()
	{
		isDepressed = false;
		if ((bool)childToMove)
		{
			childToMove.localPosition = initialPosition;
		}
		depressReturnStepsLeft = 0;
		onRise.Invoke();
		Rotator[] array = rotators;
		foreach (Rotator rotator in array)
		{
			if ((bool)rotator.Target)
			{
				rotator.Target.SetLocalRotation2D(rotator.InitialRotation);
			}
		}
	}

	public void SetActive(bool value)
	{
		if (value)
		{
			isDisabled = false;
			if ((bool)childToMove)
			{
				childToMove.localPosition = initialPosition;
			}
			if (!isDepressed && collided.Count > 0)
			{
				SetDepressed();
			}
		}
		else
		{
			isDisabled = true;
			if (isDepressed)
			{
				SetNotDepressed();
			}
			if ((bool)childToMove)
			{
				childToMove.localPosition = initialPosition + new Vector3(0f, 0f - depressDistance, 0f);
			}
		}
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}
}
