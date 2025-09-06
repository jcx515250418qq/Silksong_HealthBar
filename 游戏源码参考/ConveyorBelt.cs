using System;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
	[SerializeField]
	private float speed;

	[SerializeField]
	private bool vertical;

	private float speedMultiplier = 1f;

	private HeroController hc;

	private ConveyorMovementHero hcConveyorMove;

	public float SpeedMultiplier
	{
		get
		{
			return speedMultiplier;
		}
		set
		{
			speedMultiplier = value;
			UpdateHero();
		}
	}

	public event Action<HeroController> CapturedHero;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		float c_xSpeed = speed * speedMultiplier;
		if ((bool)collision.gameObject.GetComponent<ConveyorMovement>())
		{
			collision.gameObject.GetComponent<ConveyorMovement>().StartConveyorMove(c_xSpeed, 0f);
		}
		if ((bool)collision.gameObject.GetComponent<DropCrystal>())
		{
			collision.gameObject.GetComponent<DropCrystal>().StartConveyorMove(c_xSpeed, 0f);
		}
		HeroController component = collision.gameObject.GetComponent<HeroController>();
		if ((bool)component)
		{
			hc = component;
			hcConveyorMove = hc.GetComponent<ConveyorMovementHero>();
			UpdateHero();
			if (this.CapturedHero != null)
			{
				this.CapturedHero(hc);
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if ((bool)collision.gameObject.GetComponent<ConveyorMovement>())
		{
			collision.gameObject.GetComponent<ConveyorMovement>().StopConveyorMove();
		}
		if ((bool)collision.gameObject.GetComponent<DropCrystal>())
		{
			collision.gameObject.GetComponent<DropCrystal>().StopConveyorMove();
		}
		HeroController component = collision.gameObject.GetComponent<HeroController>();
		if (component != null && component == hc)
		{
			hcConveyorMove.StopConveyorMove();
			hc.cState.onConveyor = false;
			hc.cState.onConveyorV = false;
			hc = null;
			hcConveyorMove = null;
			if (this.CapturedHero != null)
			{
				this.CapturedHero(null);
			}
		}
	}

	private void UpdateHero()
	{
		if ((bool)hc)
		{
			float num = speed * speedMultiplier;
			if (vertical)
			{
				hcConveyorMove.StartConveyorMove(0f, num);
				hc.cState.onConveyorV = true;
			}
			else
			{
				hc.SetConveyorSpeed(num);
				hc.cState.onConveyor = true;
			}
		}
	}
}
