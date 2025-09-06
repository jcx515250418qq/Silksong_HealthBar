using System;
using UnityEngine;

public class WaitForSecondsInterruptable : CustomYieldInstruction
{
	private readonly float seconds;

	private double endTime;

	private readonly Func<bool> endCondition;

	private readonly bool isRealtime;

	public override bool keepWaiting
	{
		get
		{
			if ((isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble) < endTime)
			{
				return !endCondition();
			}
			return false;
		}
	}

	public WaitForSecondsInterruptable(float seconds, Func<bool> endCondition, bool isRealtime = false)
	{
		this.isRealtime = isRealtime;
		this.seconds = seconds;
		this.endCondition = endCondition;
		ResetTimer();
	}

	public void ResetTimer()
	{
		double num = (isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble);
		endTime = num + (double)seconds;
	}
}
