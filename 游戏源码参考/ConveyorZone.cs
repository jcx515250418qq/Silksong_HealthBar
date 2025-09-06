using UnityEngine;

public class ConveyorZone : MonoBehaviour
{
	public float speed;

	public bool vertical;

	private bool activated = true;

	private ConveyorMovement conveyorMovement;

	private HeroController hc;

	private bool hasEntered;

	private void Start()
	{
		if ((bool)HeroController.instance)
		{
			activated = false;
			HeroController.HeroInPosition temp = null;
			temp = delegate
			{
				activated = true;
				HeroController.instance.heroInPosition -= temp;
			};
			HeroController.instance.heroInPosition += temp;
		}
	}

	private void OnDisable()
	{
		if (hasEntered)
		{
			if ((bool)conveyorMovement)
			{
				conveyorMovement.StopConveyorMove();
			}
			if ((bool)hc)
			{
				hc.GetComponent<ConveyorMovementHero>().StopConveyorMove();
				hc.cState.inConveyorZone = false;
				hc.cState.onConveyorV = false;
			}
			hasEntered = false;
			conveyorMovement = null;
			hc = null;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!activated)
		{
			return;
		}
		hasEntered = true;
		conveyorMovement = collision.GetComponent<ConveyorMovement>();
		if ((bool)conveyorMovement)
		{
			conveyorMovement.StartConveyorMove(speed, 0f);
		}
		hc = collision.GetComponent<HeroController>();
		if ((bool)hc)
		{
			if (vertical)
			{
				hc.GetComponent<ConveyorMovementHero>().StartConveyorMove(0f, speed);
				hc.cState.onConveyorV = true;
			}
			else
			{
				hc.SetConveyorSpeed(speed);
				hc.cState.inConveyorZone = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (activated)
		{
			hasEntered = false;
			ConveyorMovement component = collision.GetComponent<ConveyorMovement>();
			if ((bool)component)
			{
				component.StopConveyorMove();
			}
			HeroController component2 = collision.GetComponent<HeroController>();
			if ((bool)component2)
			{
				component2.GetComponent<ConveyorMovementHero>().StopConveyorMove();
				component2.cState.inConveyorZone = false;
				component2.cState.onConveyorV = false;
			}
		}
	}

	public void SetSpeed(float speed_new)
	{
		speed = speed_new;
	}

	public void SetActivated(bool activated_new)
	{
		activated = activated_new;
	}
}
