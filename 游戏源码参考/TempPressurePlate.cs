using System;

public class TempPressurePlate : PressurePlateBase
{
	private bool isActivated;

	protected override bool CanDepress => !isActivated;

	public event Action PreActivated;

	public event Action Activated;

	protected override void PreActivate()
	{
		if (!isActivated)
		{
			this.PreActivated?.Invoke();
		}
	}

	protected override void Activate()
	{
		if (!isActivated)
		{
			isActivated = true;
			this.Activated?.Invoke();
		}
	}

	public void ActivateSilent()
	{
		if (!isActivated)
		{
			isActivated = true;
			StartDrop(force: true);
		}
	}

	protected override void Raised()
	{
		isActivated = false;
	}

	public void Deactivate()
	{
		Deactivate(0f);
	}

	public void DeactivateSilent()
	{
		if (isActivated)
		{
			isActivated = false;
			StartRaiseSilent(0f);
		}
	}

	public void Deactivate(float raiseDelay)
	{
		if (isActivated)
		{
			isActivated = false;
			StartRaise(raiseDelay);
		}
	}
}
