using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DebrisPiece : MonoBehaviour
{
	[SerializeField]
	private bool resetOnDisable;

	private bool didLaunch;

	private bool didSpin;

	[Header("'set_z' Functionality")]
	[SerializeField]
	private bool forceZ;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("forceZ", true, false, false)]
	private float forcedZ;

	[Header("'spin_self' Functionality")]
	[SerializeField]
	private bool randomStartRotation;

	[SerializeField]
	private float zRandomRadius;

	[SerializeField]
	private float spinFactor;

	private Rigidbody2D body;

	protected void Reset()
	{
		resetOnDisable = true;
		forceZ = true;
		forcedZ = 0.015f;
		randomStartRotation = false;
		zRandomRadius = 0.000999f;
		spinFactor = 10f;
	}

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		if (body == null)
		{
			Debug.LogErrorFormat(this, "Missing Rigidbody2D on {0}", base.name);
		}
	}

	protected void OnEnable()
	{
		if (!didLaunch)
		{
			Launch();
		}
	}

	protected void OnDisable()
	{
		if (resetOnDisable)
		{
			didLaunch = false;
			didSpin = false;
		}
	}

	private void Launch()
	{
		didLaunch = true;
		Transform transform = base.transform;
		if (forceZ)
		{
			Vector3 position = transform.position;
			position.z = forcedZ;
			transform.position = position;
		}
		if (randomStartRotation)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.z = UnityEngine.Random.Range(0f, 360f);
			transform.localEulerAngles = localEulerAngles;
		}
	}

	protected void FixedUpdate()
	{
		if (!didSpin)
		{
			Spin();
		}
	}

	private void Spin()
	{
		didSpin = true;
		if (!(Math.Abs(spinFactor) <= Mathf.Epsilon))
		{
			if (Math.Abs(zRandomRadius) > Mathf.Epsilon)
			{
				Vector3 position = base.transform.position;
				position.z += UnityEngine.Random.Range(0f - zRandomRadius, zRandomRadius);
				base.transform.position = position;
			}
			body.AddTorque((0f - body.linearVelocity.x) * spinFactor);
		}
	}
}
