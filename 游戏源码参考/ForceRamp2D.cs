using TeamCherry.SharedUtils;
using UnityEngine;

public class ForceRamp2D : RampBase
{
	[Space]
	[SerializeField]
	private Vector2 force;

	[SerializeField]
	private float torque;

	[Space]
	[SerializeField]
	private bool requireStarted;

	private Vector2 addForce;

	private float addTorque;

	private Rigidbody2D body;

	private bool hasBody;

	private Vector2 Force
	{
		get
		{
			return force;
		}
		set
		{
			force = value;
		}
	}

	public float Torque
	{
		get
		{
			return torque;
		}
		set
		{
			torque = value;
		}
	}

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		hasBody = body != null;
	}

	private void FixedUpdate()
	{
		if ((!requireStarted || started) && hasBody)
		{
			if (addForce.sqrMagnitude > Mathf.Epsilon)
			{
				body.AddForce(addForce, ForceMode2D.Force);
			}
			if (Mathf.Abs(addTorque) > Mathf.Epsilon)
			{
				body.AddTorque(addTorque, ForceMode2D.Force);
			}
		}
	}

	protected override void UpdateValues(float multiplier)
	{
		addForce = force * multiplier;
		addTorque = torque * multiplier;
	}

	protected override void ResetValues()
	{
		addForce = Vector2.zero;
		addTorque = 0f;
	}
}
