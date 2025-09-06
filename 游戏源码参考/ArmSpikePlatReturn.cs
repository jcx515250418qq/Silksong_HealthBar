using UnityEngine;

public class ArmSpikePlatReturn : ArmSpikePlat, IBinarySwitchable
{
	[Header("Return")]
	[SerializeField]
	private float returnDelay;

	private float returnDelayLeft;

	private bool willReturn;

	private bool binaryState;

	private bool queueChangeState;

	private void Update()
	{
		if (returnDelayLeft > 0f)
		{
			returnDelayLeft -= Time.deltaTime;
			if (returnDelayLeft <= 0f)
			{
				DoRotate(landStartDelay, 0f - base.PreviousDirection, doLandEffect: false);
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		returnDelayLeft = 0f;
	}

	protected override void OnEnd()
	{
		base.OnEnd();
		if (willReturn)
		{
			willReturn = false;
		}
		else
		{
			returnDelayLeft = returnDelay;
			willReturn = true;
		}
		if (queueChangeState)
		{
			queueChangeState = false;
			ChangeDefaultState();
		}
	}

	public void SwitchBinaryState(bool value)
	{
		bool flag = binaryState;
		binaryState = value;
		if (binaryState != flag)
		{
			if (returnDelayLeft > 0f)
			{
				returnDelayLeft = 0f;
				willReturn = false;
			}
			else if (base.IsRotating)
			{
				queueChangeState = true;
			}
			else
			{
				ChangeDefaultState();
			}
		}
	}

	private void ChangeDefaultState()
	{
		willReturn = true;
		DoRotate(0f, base.PreviousDirection, doLandEffect: false);
	}
}
