using System;
using UnityEngine;

public class ConveyorMovementHero : MonoBehaviour
{
	[SerializeField]
	private bool gravityOff;

	private float xSpeed;

	private float ySpeed;

	private bool onConveyor;

	private HeroController heroCon;

	private void Start()
	{
		heroCon = GetComponent<HeroController>();
	}

	public void StartConveyorMove(float cXSpeed, float cYSpeed)
	{
		onConveyor = true;
		xSpeed = cXSpeed;
		ySpeed = cYSpeed;
	}

	public void StopConveyorMove()
	{
		onConveyor = false;
		if (gravityOff)
		{
			if (!heroCon.cState.superDashing)
			{
				heroCon.AffectedByGravity(gravityApplies: true);
			}
			gravityOff = false;
		}
	}

	private void LateUpdate()
	{
		if (!onConveyor)
		{
			return;
		}
		if (Math.Abs(ySpeed) > 0.001f && (heroCon.cState.wallSliding || heroCon.cState.superDashOnWall))
		{
			if (heroCon.cState.superDashOnWall)
			{
				GetComponent<Rigidbody2D>().linearVelocity = new Vector3(0f, 0f);
			}
			GetComponent<Rigidbody2D>().linearVelocity = new Vector2(GetComponent<Rigidbody2D>().linearVelocity.x, ySpeed);
			if (!gravityOff)
			{
				heroCon.AffectedByGravity(gravityApplies: false);
				gravityOff = true;
			}
		}
		else if (gravityOff && !heroCon.cState.superDashing)
		{
			heroCon.AffectedByGravity(gravityApplies: true);
			gravityOff = false;
		}
	}
}
