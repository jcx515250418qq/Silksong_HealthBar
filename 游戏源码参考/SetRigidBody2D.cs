using System;
using UnityEngine;

public sealed class SetRigidBody2D : MonoBehaviour
{
	[Serializable]
	[Flags]
	private enum SetEvent
	{
		None = 0,
		Start = 1,
		OnEnable = 2,
		OnDisable = 4,
		All = -1
	}

	[SerializeField]
	private Rigidbody2D rigidbody2D;

	[Space]
	[SerializeField]
	private SetEvent setVelocity = SetEvent.All;

	[SerializeField]
	private Vector2 velocity;

	[Space]
	[SerializeField]
	private SetEvent setAngularVelocity = SetEvent.All;

	[SerializeField]
	private float angularVelocity;

	private bool hasRb2d;

	private void Awake()
	{
		FindRigidBody();
	}

	private void Start()
	{
		SetState(SetEvent.Start);
	}

	private void OnValidate()
	{
		FindRigidBody();
	}

	private void OnEnable()
	{
		SetState(SetEvent.OnEnable);
	}

	private void OnDisable()
	{
		SetState(SetEvent.OnDisable);
	}

	private void FindRigidBody()
	{
		hasRb2d = rigidbody2D;
		if (!hasRb2d)
		{
			rigidbody2D = GetComponent<Rigidbody2D>();
			hasRb2d = rigidbody2D;
		}
	}

	private void SetState(SetEvent state)
	{
		if (hasRb2d)
		{
			if ((setVelocity & state) != 0)
			{
				rigidbody2D.linearVelocity = velocity;
			}
			if ((setAngularVelocity & state) != 0)
			{
				rigidbody2D.angularVelocity = angularVelocity;
			}
		}
	}
}
